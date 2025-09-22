using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var usuario = "DanielGalleazzo";
        var repositorio = "CotacaoCripto";
        var api_key = "";
        var data1 = "2025-06-01T00:00:00Z";
        var data2 = "2025-06-30T23:59:59Z";
        int contagem = 0;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", api_key);

        var url = $"https://api.github.com/repos/{usuario}/{repositorio}/commits?author={usuario}&since={data1}&until={data2}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);
        foreach (var commit in doc.RootElement.EnumerateArray())
        {
            var message = commit.GetProperty("commit").GetProperty("message").GetString();
            Console.WriteLine("Mensagem do commit: " + message);
            contagem++;
           
        }
        Console.WriteLine("Quantidade de commits no período: " + contagem);
    }
}
