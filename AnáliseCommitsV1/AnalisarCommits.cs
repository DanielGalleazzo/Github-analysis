using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AnáliseCommitsV1
{
    public class AnaliseDoGPT
    {
        public static async Task<string> AnalisarCommitsComGPT(string commitsTexto)
        {
            string apiKey = ""; 

           

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4o-mini",   // requisição HTTP
                messages = new object[]
                {
                    new { role = "system", content = "Você é um assistente que analisa commits de código." }, // aqui voce vai colocar o prompt
                    new { role = "user", content = $"Analise os commits abaixo e me dê um resumo:{commitsTexto}" }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var resposta = doc.RootElement
                              .GetProperty("choices")[0]
                              .GetProperty("message")
                              .GetProperty("content")
                              .GetString();

            return resposta;
        }
    }
}
