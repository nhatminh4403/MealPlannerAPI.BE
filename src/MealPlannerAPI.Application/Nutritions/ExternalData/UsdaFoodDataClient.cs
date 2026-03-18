using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MealPlannerAPI.Nutritions.ExternalData
{
    public class UsdaFoodDataClient : IUsdaFoodDataClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        private readonly ILogger<UsdaFoodDataClient> _logger;
        private const string BaseUrl = "https://api.nal.usda.gov/fdc/v1";
        private const int NutrientEnergy = 1008; // kcal
        private const int NutrientProtein = 1003;
        private const int NutrientCarbs = 1005;
        private const int NutrientFat = 1004;
        private const int NutrientFiber = 1079;
        public UsdaFoodDataClient(HttpClient http,
                                  ILogger<UsdaFoodDataClient> logger,
                                  IConfiguration configuration)
        {
            _http = http;
            _logger = logger;
            _apiKey = configuration["Usda:ApiKey"] ?? "DEMO_KEY";

            _http.BaseAddress = new Uri(BaseUrl);
            _http.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<UsdaFoodDataResultDTO>> SearchAsync(
         string query,
         int maxResults = 5,
         CancellationToken cancellationToken = default)
        {
            try
            {
                // Prefer "Foundation" and "SR Legacy" data types —
                // these are raw ingredients with the most accurate per-100g values.
                // "Branded" foods are also included as fallback.
                var url = $"{BaseUrl}/foods/search" +
                          $"?query={Uri.EscapeDataString(query)}" +
                          $"&dataType=Foundation,SR%20Legacy,Branded" +
                          $"&pageSize={maxResults * 3}" + // fetch extra to filter below
                          $"&api_key={_apiKey}";

                var response = await _http.GetFromJsonAsync<UsdaSearchResponse>(
                    url, cancellationToken);

                if (response?.Foods == null) return [];

                return response.Foods
                    .Select(Map)
                    .Where(r => r.CaloriesPer100g > 0)
                    .OrderByDescending(r => r.CompletenessScore)
                    .Take(maxResults)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "USDA FoodData search failed for query: {Query}", query);
                return [];
            }
        }

        // ── Mapping ───────────────────────────────────────────────────────────────

        private static UsdaFoodDataResultDTO Map(UsdaFood food)
        {
            var nutrients = food.FoodNutrients ?? [];

            float Get(int id) => MathF.Round(
                nutrients.FirstOrDefault(n => n.NutrientId == id)?.Value ?? 0f, 1);

            var calories = Get(NutrientEnergy);
            var protein = Get(NutrientProtein);
            var carbs = Get(NutrientCarbs);
            var fat = Get(NutrientFat);
            var fiber = Get(NutrientFiber);

            // Score: each present macro = 20 pts, calories = 20 pts
            var score = (calories > 0 ? 20 : 0)
                      + (protein > 0 ? 20 : 0)
                      + (carbs > 0 ? 20 : 0)
                      + (fat > 0 ? 20 : 0)
                      + (fiber > 0 ? 20 : 0);

            return new UsdaFoodDataResultDTO
            {
                Name = food.Description?.Trim() ?? "Unknown",
                BrandOwner = string.IsNullOrWhiteSpace(food.BrandOwner) ? null : food.BrandOwner.Trim(),
                CaloriesPer100g = calories,
                ProteinPer100g = protein,
                CarbsPer100g = carbs,
                FatPer100g = fat,
                FiberPer100g = fiber,
                CompletenessScore = score,
                FdcId = food.FdcId?.ToString(),
            };
        }

        // ── USDA response shape ───────────────────────────────────────────────────

        private class UsdaSearchResponse
        {
            [JsonPropertyName("foods")]
            public List<UsdaFood>? Foods { get; set; }
        }

        private class UsdaFood
        {
            [JsonPropertyName("fdcId")]
            public int? FdcId { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }

            [JsonPropertyName("brandOwner")]
            public string? BrandOwner { get; set; }

            [JsonPropertyName("dataType")]
            public string? DataType { get; set; }

            [JsonPropertyName("foodNutrients")]
            public List<UsdaNutrient>? FoodNutrients { get; set; }
        }

        private class UsdaNutrient
        {
            [JsonPropertyName("nutrientId")]
            public int NutrientId { get; set; }

            [JsonPropertyName("value")]
            public float Value { get; set; }
        }
    }
}
