using MealPlannerAPI.Mappings.Recipes;
using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.MealPlans.Services;
using MealPlannerAPI.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
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
        private readonly MealPlanToMealPlanDtoMapper _toMealPlanDtoMapper;
        private readonly MealPlanEntryToMealPlanEntryDtoMapper _toEntryDtoMapper;
        private readonly CreateUpdateMealPlanEntryDtoToMealPlanEntryMapper _toEntryMapper;
        private readonly MealPlanManager _mealPlanManager;
        public MealPlanAppService(IMealPlanRepository mealPlanRepository,
                                  IRepository<MealPlanEntry, Guid> mealPlanEntryRepository,
                                  IRecipeRepository recipeRepository,
                                  MealPlanToMealPlanDtoMapper toMealPlanDtoMapper,
                                  MealPlanEntryToMealPlanEntryDtoMapper toEntryDtoMapper,
                                  CreateUpdateMealPlanEntryDtoToMealPlanEntryMapper toEntryMapper,
                                  MealPlanManager mealPlanManager) : base(mealPlanRepository)
        {
            _mealPlanRepository = mealPlanRepository;
            _mealPlanEntryRepository = mealPlanEntryRepository;
            _recipeRepository = recipeRepository;
            _toMealPlanDtoMapper = toMealPlanDtoMapper;
            _toEntryDtoMapper = toEntryDtoMapper;
            _toEntryMapper = toEntryMapper;
            _mealPlanManager = mealPlanManager;
        }

        public override async Task<MealPlanDto> GetAsync(Guid id)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(id);
            if (mealPlan == null)
            {
                throw new EntityNotFoundException(typeof(MealPlan), id);
            }
            return await MapToMealPlanDtoAsync(mealPlan);
        }

        public async Task<MealPlanDto> GetCurrentWeekAsync()
        {
            var plan = await _mealPlanManager.GetOrCreateMealPlanAsync(CurrentUser.GetId(), DateTime.UtcNow);

            return await MapToMealPlanDtoAsync(plan);
        }

        public async override Task<PagedResultDto<MealPlanDto>> GetListAsync(GetMealPlansInput input)
        {
            var query = await _mealPlanRepository.GetQueryableAsync();

            query = query.Where(mp => mp.UserId == CurrentUser.GetId());

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

        public async Task<MealPlanEntryDto> SetEntryAsync(Guid mealPlanId, CreateUpdateMealPlanEntryDto input)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(mealPlanId);

            var entry = mealPlan.AddEntry(
                GuidGenerator.Create(),
                input.DayOfWeek, input.MealName, input.MealType, input.ScheduledTime, input.RecipeId);

            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);
            return await MapToEntryDtoAsync(entry);
        }

        public async Task DeleteEntryAsync(Guid mealPlanId, Guid entryId)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(mealPlanId);
            if (mealPlan == null)
                throw new EntityNotFoundException(typeof(MealPlan), mealPlanId);

            mealPlan.RemoveEntry(entryId);
            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);
        }

        public async override Task<MealPlanDto> CreateAsync(CreateUpdateMealPlanDto input)
        {
            var mealPlan = new MealPlan(
            GuidGenerator.Create(),
            CurrentUser.GetId(),
            input.WeekStartDate);

            foreach (var e in input.Entries)
                mealPlan.AddEntry(GuidGenerator.Create(), e.DayOfWeek, e.MealName, e.MealType, e.ScheduledTime, e.RecipeId);

            await _mealPlanRepository.InsertAsync(mealPlan, autoSave: true);
            return await MapToMealPlanDtoAsync(mealPlan);
        }

        public async override Task<MealPlanDto> UpdateAsync(Guid id, CreateUpdateMealPlanDto input)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(id);

            mealPlan.WeekStartDate = MealPlan.GetWeekStart(input.WeekStartDate);
            mealPlan.ReplaceEntries(
                input.Entries.Select(e => (
                    GuidGenerator.Create(),
                    e.DayOfWeek, e.MealName, e.MealType, e.ScheduledTime, e.RecipeId)));

            await _mealPlanRepository.UpdateAsync(mealPlan, autoSave: true);
            return await MapToMealPlanDtoAsync(mealPlan);
        }

        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
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
