using RPSLS.Application.Interfaces;
using System.Text.Json;

namespace RPSLS.Infrastructure.Services
{
    public class RandomNumberService : IRandomNumberService
    {
        private readonly HttpClient _httpClient;
        private const string RandomApi = "https://codechallenge.boohma.com/random";

        public RandomNumberService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetRandomNumberAsync()
        {
            var response = await _httpClient.GetAsync(RandomApi);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            return doc.RootElement.GetProperty("random_number").GetInt32();
        }
    }
}
