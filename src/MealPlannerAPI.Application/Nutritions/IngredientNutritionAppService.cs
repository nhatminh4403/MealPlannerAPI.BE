using MealPlannerAPI.Nutritions.Dtos;
using MealPlannerAPI.Nutritions.ExternalData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Guids;

namespace MealPlannerAPI.Nutritions
{
    [RemoteService(false)]
    public class IngredientNutritionAppService : MealPlannerAPIAppService, IIngredientNutritionAppService
    {
        private readonly IIngredientNutritionRepository _ingredientNutritionRepository;
        private readonly IUsdaFoodDataClient _usdaClient;
        private readonly IGuidGenerator _guidGenerator;

        public IngredientNutritionAppService(IIngredientNutritionRepository ingredientNutritionRepository,
                                             IUsdaFoodDataClient usdaClient,
                                             IGuidGenerator guidGenerator)
        {
            _ingredientNutritionRepository = ingredientNutritionRepository;
            _usdaClient = usdaClient;
            _guidGenerator = guidGenerator;
        }

        public async Task<IngredientNutritionDto> CreateAsync(CreateIngredientNutritionDto input)
        {
            var existing = await _ingredientNutritionRepository.FindByNameAsync(input.Name);
            if (existing != null)
                return MapToDto(existing);

            var entity = new IngredientNutrition(_guidGenerator.Create(),
                                                 input.Name,
                                                 input.CaloriesPer100g,
                                                 input.ProteinPer100g,
                                                 input.CarbsPer100g,
                                                 input.FatPer100g,
                                                 input.FiberPer100g);

            await _ingredientNutritionRepository.InsertAsync(entity, autoSave: true);
            return MapToDto(entity);
        }

        public async Task<PagedResultDto<IngredientNutritionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var totalCount = await _ingredientNutritionRepository.GetCountAsync();
            var items = await _ingredientNutritionRepository.GetPagedListAsync(input.SkipCount,
                                                                               input.MaxResultCount,
                                                                               input.Sorting ?? nameof(IngredientNutrition.Name));

            return new PagedResultDto<IngredientNutritionDto>(
                totalCount,
                items.Select(MapToDto).ToList()
                );
        }

        public async Task<IngredientNutritionSearchResultDto> SearchAsync(string query, bool includeExternal = false)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new IngredientNutritionSearchResultDto();

            var normalized = query.Trim().ToLowerInvariant();

            var dbMatches = await _ingredientNutritionRepository.GetListAsync(
                x => x.NormalizedName.Contains(normalized));

            var result = new IngredientNutritionSearchResultDto
            {
                DbResults = dbMatches
                    .OrderBy(x => x.NormalizedName.StartsWith(normalized) ? 0 : 1)
                    .Take(8)
                    .Select(MapToDto)
                    .ToList(),
            };

            // Only call USDA when user explicitly clicks "Search USDA"
            if (includeExternal)
            {
                var usdaResults = await _usdaClient.SearchAsync(query, maxResults: 6);

                var existingNames = result.DbResults
                    .Select(x => x.Name.ToLowerInvariant())
                    .ToHashSet();

                result.ExternalCandidates = usdaResults
                    .Where(r => !existingNames.Contains(r.Name.ToLowerInvariant()))
                    .Select(r => new ExternalFoodCandidateDto
                    {
                        Name = r.Name,
                        Brand = r.BrandOwner,
                        CaloriesPer100g = r.CaloriesPer100g,
                        ProteinPer100g = r.ProteinPer100g,
                        CarbsPer100g = r.CarbsPer100g,
                        FatPer100g = r.FatPer100g,
                        FiberPer100g = r.FiberPer100g,
                        CompletenessScore = r.CompletenessScore,
                        ExternalId = r.FdcId,
                        Source = "USDA",
                    })
                    .ToList();
            }

            return result;
        }
        private static IngredientNutritionDto MapToDto(IngredientNutrition e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            CaloriesPer100g = e.CaloriesPer100g,
            ProteinPer100g = e.ProteinPer100g,
            CarbsPer100g = e.CarbsPer100g,
            FatPer100g = e.FatPer100g,
            FiberPer100g = e.FiberPer100g,
            IsVerified = true,
        };
    }
}
