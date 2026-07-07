using System.Text;
using System.Text.Json;

namespace RagSaaS.Backend.Services
{
    public class GeminiService
    {
        private readonly IConfiguration _configuration;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateAnswerAsync(string question, string context)
        {
            string apiKey = _configuration["GeminiApiKey"];
            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";
            string prompt = $"Context: {context}\n\nQuestion:{question}\n\nAnswer based Only on the context provided above.";

            var payload = new
            {
                contents = new[] { new { parts = new[] {new { text = prompt}}}}
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to communicate with AI provider.");

            var responseString = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseString);

            return jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();
        }
    }
}
