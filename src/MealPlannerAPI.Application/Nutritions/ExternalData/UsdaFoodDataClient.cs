// MealPlannerAPI.Nutritions.ExternalData.UsdaFoodDataClient
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
                var url = $"{BaseUrl}/foods/search" +
                          $"?query={Uri.EscapeDataString(query)}" +
                          $"&dataType=Foundation,SR%20Legacy,Branded" +
                          $"&pageSize={maxResults * 3}" +
                          $"&api_key={_apiKey}";

                var response = await _http.GetFromJsonAsync<UsdaSearchResponse>(url, cancellationToken);

                if (response?.Foods == null) return new List<UsdaFoodDataResultDTO>();

                return response.Foods
                    .Select(Map)
                    // Drop items with virtually 0 scores or data entirely
                    .Where(r => r.CompletenessScore > 0 || r.CaloriesPer100g > 0)
                    .OrderByDescending(r => r.CompletenessScore)
                    .Take(maxResults)
                    .ToList();
            }
            catch (Exception ex)
            {
                // This will print if you get Rate Limited (429)!
                _logger.LogWarning(ex, "USDA FoodData search failed for query: {Query}. Make sure you haven't exceeded your DEMO_KEY limits.", query);
                return new List<UsdaFoodDataResultDTO>();
            }
        }

        // ── Mapping ───────────────────────────────────────────────────────────────

        private static UsdaFoodDataResultDTO Map(UsdaFood food)
        {
            var nutrients = food.FoodNutrients ?? new List<UsdaNutrient>();

            // Improved parser checks both standard strings (FDC universal representations like "208") and integers like "1008".
            float GetSafeValue(string stringNumId, int intBackupId) => MathF.Round(
                nutrients.FirstOrDefault(n => n.NutrientNumber == stringNumId || n.NutrientId == intBackupId)?.Value ?? 0f, 1);

            // Energy="208"(str) or 1008(id), Protein="203", Carbs="205", Fat="204", Fiber="291"
            var calories = GetSafeValue("208", 1008);
            var protein = GetSafeValue("203", 1003);
            var carbs = GetSafeValue("205", 1005);
            var fat = GetSafeValue("204", 1004);
            var fiber = GetSafeValue("291", 1079);

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
            // FDC Native Integer
            [JsonPropertyName("nutrientId")]
            public int NutrientId { get; set; }

            // MUST INCLUDE: Standard string nutrientNumber mapping fixes "missing values" in basic veg
            [JsonPropertyName("nutrientNumber")]
            public string? NutrientNumber { get; set; }

            [JsonPropertyName("value")]
            public float Value { get; set; }
        }
    }
}