using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var usuario = "DanielGalleazzo";
        var repositorio = "LeetCode";
        var api_key = ""; 
        var data1 = "2025-08-01T00:00:00Z";
        var  data2= "2025-08-31T23:59:59Z";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", api_key);

        var url = $"https://api.github.com/repos/{usuario}/{repositorio}/commits?author={usuario}&since={data1}&until={data2}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine(content); 
    }
}
