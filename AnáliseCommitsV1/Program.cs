using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using OfficeOpenXml;
using AnáliseCommitsV1;

class Program
{
    static async Task Main()
    {
        var usuario = "Galleazzo";
        var repositorio = "Casamento-Julia-Paulo-BackEnd";
        var api_key = "coloque aqui a sua chave api";
        var data1 = "2025-09-13T00:00:00Z";
        var data2 = "2025-09-27T23:59:59Z";
        int contagem = 0;

        string caminhoArquivo = @"C:\Users\danie\Desktop\AnáliseCommitsV1\GitHub_análise_v1.xlsx";
        string caminhoTxt = Path.ChangeExtension(caminhoArquivo, ".txt");

        ExcelPackage.License.SetNonCommercialPersonal("DanielGalleazzo");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", api_key);

        var url = $"https://api.github.com/repos/{usuario}/{repositorio}/commits?author={usuario}&since={data1}&until={data2}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Commits");

        worksheet.Cells[1, 1].Value = "Autor";
        worksheet.Cells[1, 2].Value = "Data";
        worksheet.Cells[1, 3].Value = "Mensagem";

        int linha = 2;
        var commitsLista = new List<(DateTime Data, string Autor, string Mensagem)>();

        foreach (var commit in doc.RootElement.EnumerateArray())
        {
            var commitObj = commit.GetProperty("commit");
            var autor = commitObj.GetProperty("author").GetProperty("name").GetString();
            var data = commitObj.GetProperty("author").GetProperty("date").GetDateTime();
            var mensagem = commitObj.GetProperty("message").GetString();

            Console.WriteLine("Autor: " + autor);
            Console.WriteLine("Data: " + data);
            Console.WriteLine("Mensagem: " + mensagem);
            Console.WriteLine("");

            worksheet.Cells[linha, 1].Value = autor;
            worksheet.Cells[linha, 2].Value = data.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[linha, 3].Value = mensagem;

            worksheet.Cells["A1:C1"].Style.Font.Bold = true;
            worksheet.Cells["A1:C1"].Style.Font.Italic = true;

            commitsLista.Add((data, autor, mensagem));
            linha++;
            contagem++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        Console.WriteLine("Você deseja salvar essas informações numa planilha ? (sim/não)");
        string resposta = Console.ReadLine();

        if (resposta?.ToLower() == "sim")
        {
            var arquivo = new FileInfo(caminhoArquivo);
            package.SaveAs(arquivo);
        }

        Console.WriteLine("Você quer salvar esses dados num arquivo txt ? (sim/não)");
        string resposta1 = Console.ReadLine();
        if (resposta1?.ToLower() == "sim")
        {
            using (StreamWriter writer = new StreamWriter(caminhoTxt))
            {
                foreach (var item in commitsLista)
                {
                    writer.WriteLine($"Autor: {item.Autor}");
                    writer.WriteLine($"Data: {item.Data:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Mensagem: {item.Mensagem}");
                    writer.WriteLine("---");
                }
            }
        }

        var commitsPorDia = commitsLista
           .GroupBy(c => c.Data.Date)
           .Select(g => new { Data = g.Key, Count = g.Count() })
           .OrderBy(g => g.Data);

        Console.WriteLine("Quantidade de commits no período: " + contagem);

        Console.WriteLine("");
        Console.WriteLine("Gráfico representando a quantidade de commits por dia: ");
        int maxBarras = 50;
        int maxCommits = commitsPorDia.Max(x => x.Count);
        foreach (var item in commitsPorDia)
        {
            int barrasCount = (int)Math.Ceiling((double)item.Count / maxCommits * maxBarras);
            string barras = new string('-', barrasCount);
            Console.WriteLine($"{item.Data:yyyy-MM-dd}: {barras} ({item.Count})");
        }


        if (commitsLista.Count > 0)
        {
            Console.WriteLine("");
            Console.WriteLine("Deseja que o GPT faça uma análise resumida dos commits? (sim/não)");
            string respostaGPT = Console.ReadLine();
            if (respostaGPT?.ToLower() == "sim")
            {
                string textoCommits = TextoCommit.GerarTextoCommits(commitsLista);
                string analise = await AnaliseDoGPT.AnalisarCommitsComGPT(textoCommits);
                Console.WriteLine("");
                Console.WriteLine("=== Análise do GPT ===");
                Console.WriteLine(analise);
            }
        }
    }
}