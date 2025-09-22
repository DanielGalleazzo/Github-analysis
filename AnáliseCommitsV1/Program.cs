using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;

class Program
{
    static async Task Main()
    {
        var usuario = "DanielGalleazzo";
        var repositorio = "CotacaoCripto";
        var api_key = "aaaa";
        var data1 = "2025-06-01T00:00:00Z";
        var data2 = "2025-06-30T23:59:59Z";
        int contagem = 0;

        string caminhoArquivo = @"C:\Users\danie\Desktop\AnáliseCommitsV1\GitHub_análise_v1.xlsx"; // caminho

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

        foreach (var commit in doc.RootElement.EnumerateArray())
        {
            var commitObj = commit.GetProperty("commit");
            var autor = commitObj.GetProperty("author").GetProperty("name").GetString();
            var data = commitObj.GetProperty("author").GetProperty("date").GetDateTime();
            var mensagem = commitObj.GetProperty("message").GetString();

            worksheet.Cells[linha, 1].Value = autor;
            worksheet.Cells[linha, 2].Value = data.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[linha, 3].Value = mensagem;

            linha++;
            contagem++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        var arquivo = new FileInfo(caminhoArquivo);
        package.SaveAs(arquivo);

        Console.WriteLine("Quantidade de commits no período: " + contagem);
        Console.WriteLine("Arquivo salvo em: " + caminhoArquivo);
    }
}
