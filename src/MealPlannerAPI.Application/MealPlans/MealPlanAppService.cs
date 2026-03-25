using MealPlannerAPI.Enums;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.Mappings.Recipes;
using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.MealPlans.Services;
using MealPlannerAPI.Permissions;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using static MealPlannerAPI.Mappings.Recipes.MealPlanEntryToMealPlanEntryDtoMapper;

namespace MealPlannerAPI.MealPlans
{
    [RemoteService(false)]
    public class MealPlanAppService : CrudAppService<
        MealPlan,
        MealPlanDto,
        MealPlanDto,
        Guid,
        GetMealPlansInput,
        CreateUpdateMealPlanDto,
        CreateUpdateMealPlanDto>,
        IMealPlanAppService
    {

        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IRepository<MealPlanEntry, Guid> _mealPlanEntryRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly MealPlanToMealPlanDtoMapper _toMealPlanDtoMapper;
        private readonly MealPlanEntryToMealPlanEntryDtoMapper _toEntryDtoMapper;
        private readonly CreateUpdateMealPlanEntryDtoToMealPlanEntryMapper _toEntryMapper;
        private readonly MealPlanManager _mealPlanManager;
        private readonly IMealPlannerHubPublisher _hub;
        public MealPlanAppService(IMealPlanRepository mealPlanRepository,
                                  IRepository<MealPlanEntry, Guid> mealPlanEntryRepository,
                                  IRecipeRepository recipeRepository,
                                  IIdentityUserRepository identityUserRepository,
                                  MealPlanToMealPlanDtoMapper toMealPlanDtoMapper,
                                  MealPlanEntryToMealPlanEntryDtoMapper toEntryDtoMapper,
                                  CreateUpdateMealPlanEntryDtoToMealPlanEntryMapper toEntryMapper,
                                  MealPlanManager mealPlanManager,
                                  IMealPlannerHubPublisher hub) : base(mealPlanRepository)
        {
            _mealPlanRepository = mealPlanRepository;
            _mealPlanEntryRepository = mealPlanEntryRepository;
            _recipeRepository = recipeRepository;
            _identityUserRepository = identityUserRepository;
            _toMealPlanDtoMapper = toMealPlanDtoMapper;
            _toEntryDtoMapper = toEntryDtoMapper;
            _toEntryMapper = toEntryMapper;
            _mealPlanManager = mealPlanManager;
            _hub = hub;
            ConfigurePolicies();
        }
        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            CreatePolicyName = MealPlannerAPIPermissions.MealPlans.Create;
            UpdatePolicyName = MealPlannerAPIPermissions.MealPlans.Create;
            DeletePolicyName = MealPlannerAPIPermissions.MealPlans.Create;

        }

        [Authorize(MealPlannerAPIPermissions.MealPlans.Default)]
        public override async Task<MealPlanDto> GetAsync(Guid id)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(id);
            if (mealPlan == null)
            {
                throw new EntityNotFoundException(typeof(MealPlan), id);
            }



            //var dailyNutrition = mealPlanEntries
            //    .Where(e => e.Recipe.NutritionPerServing != null)
            //    .GroupBy(e => e.DayOfWeek)
            //    .ToDictionary(
            //        g => g.Key,
            //        g => new NutritionalInfoDto
            //        {
            //            Calories = g.Sum(e => e.Recipe.NutritionPerServing!.Calories),
            //            ProteinGrams = g.Sum(e => e.Recipe.NutritionPerServing!.ProteinGrams),
            //            CarbsGrams = g.Sum(e => e.Recipe.NutritionPerServing!.CarbsGrams),
            //            FatGrams = g.Sum(e => e.Recipe.NutritionPerServing!.FatGrams),
            //            FiberGrams = g.Sum(e => e.Recipe.NutritionPerServing!.FiberGrams),
            //        });
            return await MapToMealPlanDtoAsync(mealPlan);
        }

        [Authorize(MealPlannerAPIPermissions.MealPlans.Default)]
        public async Task<MealPlanDto> GetCurrentWeekAsync()
        {
            var plan = await _mealPlanManager.GetOrCreateMealPlanAsync(CurrentUser.GetId(), DateTime.UtcNow);

            return await MapToMealPlanDtoAsync(plan);
        }

        //[Authorize()]
        //[AllowAnonymous]
        public async override Task<PagedResultDto<MealPlanDto>> GetListAsync(GetMealPlansInput input)
        {
            var query = await _mealPlanRepository.GetQueryableAsync();

            // Safely assign nullable Id (will be null for anonymous requests)
            var currentUserId = CurrentUser.Id;

            // Update the LINQ expression to account for a possible null currentUserId
            query = query.Where(mp => mp.UserId == input.UserId ||
                                      (currentUserId.HasValue && mp.UserId == currentUserId.Value));

            if (input.WeekStartDate.HasValue)
                query = query.Where(mp => mp.WeekStartDate == input.WeekStartDate.Value);

            var totalCount = await AsyncExecuter.CountAsync(query);

            var mealPlans = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(mp => mp.WeekStartDate)
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount));

            var dtos = new List<MealPlanDto>();
            foreach (var mp in mealPlans)
                dtos.Add(await MapToMealPlanDtoAsync(mp));

            return new PagedResultDto<MealPlanDto>(totalCount, dtos);
        }
        [Authorize(MealPlannerAPIPermissions.MealPlans.Create)]
        public async Task<MealPlanEntryDto> SetEntryAsync(Guid mealPlanId, CreateUpdateMealPlanEntryDto input)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(mealPlanId);

            var entry = mealPlan.AddEntry(
                GuidGenerator.Create(),
                input.DayOfWeek, input.MealName, input.MealType, input.ScheduledTime, input.RecipeId);

            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);
            return await MapToEntryDtoAsync(entry);
        }
        [Authorize(MealPlannerAPIPermissions.MealPlans.Delete)]
        public async Task DeleteEntryAsync(Guid mealPlanId, Guid entryId)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(mealPlanId);
            if (mealPlan == null)
                throw new EntityNotFoundException(typeof(MealPlan), mealPlanId);

            mealPlan.RemoveEntry(entryId);
            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);
            var dto = await MapToMealPlanDtoAsync(mealPlan);
            await _hub.NotifyMealPlanUpdatedAsync(mealPlan.UserId, dto);
        }
        [Authorize(MealPlannerAPIPermissions.MealPlans.Create)]
        public async override Task<MealPlanDto> CreateAsync(CreateUpdateMealPlanDto input)
        {
            var mealPlan = new MealPlan(
            GuidGenerator.Create(),
            CurrentUser.GetId(),
            input.WeekStartDate);

            foreach (var e in input.Entries)
                mealPlan.AddEntry(GuidGenerator.Create(), e.DayOfWeek, e.MealName, e.MealType, e.ScheduledTime, e.RecipeId);

            await _mealPlanRepository.InsertAsync(mealPlan, autoSave: true);
            var dto = await MapToMealPlanDtoAsync(mealPlan);
            await _hub.NotifyMealPlanUpdatedAsync(CurrentUser.GetId(), dto);
            return dto;
        }
        [Authorize(MealPlannerAPIPermissions.MealPlans.Update)]
        public async override Task<MealPlanDto> UpdateAsync(Guid id, CreateUpdateMealPlanDto input)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(id);

            mealPlan.WeekStartDate = MealPlan.GetWeekStart(input.WeekStartDate);
            mealPlan.ReplaceEntries(
                input.Entries.Select(e => (
                    GuidGenerator.Create(),
                    e.DayOfWeek, e.MealName, e.MealType, e.ScheduledTime, e.RecipeId)));

            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);
            var dto = await MapToMealPlanDtoAsync(mealPlan);
            await _hub.NotifyMealPlanUpdatedAsync(mealPlan.UserId, dto);
            return dto;
        }
        [Authorize(MealPlannerAPIPermissions.MealPlans.Delete)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        // ── Auto-generate ─────────────────────────────────────────────────────────

        [Authorize(MealPlannerAPIPermissions.MealPlans.Create)]
        public async Task<MealPlanDto> AutoGenerateAsync(AutoGenerateMealPlanDto input)
        {
            var userId = CurrentUser.GetId();

            // ── 1. Resolve user preferences ───────────────────────────────────
            var cuisines = input.CuisinePreferences ?? new();
            var restrictions = input.DietaryRestrictions ?? new();

            if (cuisines.Count == 0 || restrictions.Count == 0)
            {
                var identity = await _identityUserRepository.FindAsync(userId);
                if (identity is UserProfile profile)
                {
                    if (cuisines.Count == 0)
                        cuisines = profile.GetCuisinePreferences();
                    if (restrictions.Count == 0)
                        restrictions = profile.GetDietaryRestrictions();
                }
            }

            // ── 2. Build candidate recipe pool ────────────────────────────────
            var recipeQuery = await _recipeRepository.GetQueryableAsync();

            // Filter by cuisine (if any preferences set)
            if (cuisines.Count > 0)
                recipeQuery = recipeQuery.Where(r => cuisines.Contains(r.Cuisine));

            // Filter by dietary tags (if any restrictions set)
            if (restrictions.Count > 0)
                recipeQuery = recipeQuery.Where(r =>
                    r.Tags != null && restrictions.Any(tag => r.Tags.Contains(tag)));

            // Filter by max total time
            if (input.MaxTotalTimeMinutes.HasValue)
                recipeQuery = recipeQuery.Where(r =>
                    r.CookingTimeMinutes + r.PrepTimeMinutes <= input.MaxTotalTimeMinutes.Value);

            // Filter by max difficulty
            if (input.MaxDifficulty.HasValue)
                recipeQuery = recipeQuery.Where(r =>
                    (int)r.Difficulty <= input.MaxDifficulty.Value);

            var candidates = await AsyncExecuter.ToListAsync(recipeQuery);

            // Fall back to ALL recipes when no matches
            if (candidates.Count == 0)
                candidates = await AsyncExecuter.ToListAsync(
                    await _recipeRepository.GetQueryableAsync());

            if (candidates.Count == 0)
                throw new BusinessException("MealPlan:NoRecipes")
                    .WithData("message", "No recipes found to build a plan.");

            // ── 3. Determine meal types per day ───────────────────────────────
            var mealTypes = (input.MealTypes?.Count > 0
                ? input.MealTypes.Select(m => (MealType)m).ToList()
                : new List<MealType> { MealType.Breakfast, MealType.Lunch, MealType.Dinner });

            // ── 4. Build the meal plan ─────────────────────────────────────────
            var weekStart = input.WeekStartDate.HasValue
                ? MealPlan.GetWeekStart(input.WeekStartDate.Value)
                : MealPlan.GetWeekStart(DateTime.UtcNow);

            var mealPlan = await _mealPlanManager.GetOrCreateMealPlanAsync(userId, weekStart);

            // Clear any existing entries for a clean generation
            mealPlan.ReplaceEntries(Enumerable.Empty<(Guid, DayOfWeek, string, MealType, string?, Guid?)>());

            var rng = new Random();
            var daysOfWeek = new[]
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
            };

            var scheduledTimes = new Dictionary<MealType, string>
            {
                [MealType.Breakfast] = "07:00",
                [MealType.Lunch] = "12:00",
                [MealType.Dinner] = "18:00",
                [MealType.Snack] = "15:00"
            };

            foreach (var day in daysOfWeek)
            {
                foreach (var mt in mealTypes)
                {
                    var recipe = candidates[rng.Next(candidates.Count)];
                    mealPlan.AddEntry(
                        GuidGenerator.Create(),
                        day,
                        mt.ToString(),
                        mt,
                        scheduledTimes.GetValueOrDefault(mt),
                        recipe.Id);
                }
            }

            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);

            var dto = await MapToMealPlanDtoAsync(mealPlan);
            await _hub.NotifyMealPlanUpdatedAsync(userId, dto);
            return dto;
        }
        private async Task<MealPlanDto> MapToMealPlanDtoAsync(MealPlan mealPlan)
        {
            var dto = _toMealPlanDtoMapper.Map(mealPlan);
            dto.Days = await BuildDaysWithRecipeNamesAsync(mealPlan.Entries);
            return dto;
        }

        private async Task<List<MealPlanDayDto>> BuildDaysWithRecipeNamesAsync(ICollection<MealPlanEntry> entries)
        {
            // Batch-load recipe names for all entries that have a RecipeId
            var recipeIds = entries
                .Where(e => e.RecipeId.HasValue)
                .Select(e => e.RecipeId!.Value)
                .Distinct()
                .ToList();

            var recipeNames = new Dictionary<Guid, string>();
            if (recipeIds.Any())
            {
                var query = await _recipeRepository.GetQueryableAsync();
                recipeNames = await AsyncExecuter.ToListAsync(
                    query.Where(r => recipeIds.Contains(r.Id))
                         .Select(r => new { r.Id, r.Name }))
                    .ContinueWith(t => t.Result.ToDictionary(r => r.Id, r => r.Name));
            }

            return entries
                .GroupBy(e => e.DayOfWeek)
                .OrderBy(g => g.Key == DayOfWeek.Sunday ? 7 : (int)g.Key)
                .Select(g => new MealPlanDayDto
                {
                    DayOfWeek = g.Key,
                    Meals = g.OrderBy(e => e.MealType)
                              .Select(e =>
                              {
                                  var entryDto = _toEntryDtoMapper.Map(e);
                                  entryDto.RecipeName = e.RecipeId.HasValue &&
                                      recipeNames.TryGetValue(e.RecipeId.Value, out var name)
                                      ? name
                                      : null;
                                  return entryDto;
                              })
                              .ToList()
                })
                .ToList();
        }

        private async Task<MealPlanEntryDto> MapToEntryDtoAsync(MealPlanEntry entry)
        {
            var dto = _toEntryDtoMapper.Map(entry);
            if (entry.RecipeId.HasValue)
            {
                var recipe = await _recipeRepository.FindAsync(entry.RecipeId.Value);
                dto.RecipeName = recipe?.Name;
            }
            return dto;
        }
    }
}
