using MealPlannerAPI.MealPlans;
using MealPlannerAPI.MealPlans.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Guids;
using Volo.Abp.Mapperly;

namespace MealPlannerAPI.Mappings.Recipes;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MealPlanToMealPlanDtoMapper : MapperBase<MealPlan, MealPlanDto>
{
    [MapperIgnoreTarget(nameof(MealPlanDto.Days))]
    public override partial MealPlanDto Map(MealPlan source);
    [MapperIgnoreTarget(nameof(MealPlanDto.Days))]
    public override partial void Map(MealPlan source, MealPlanDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMealPlanDtoToMealPlanMapper : MapperBase<CreateUpdateMealPlanDto, MealPlan>
{
    private readonly IGuidGenerator _guidGenerator;

    public CreateUpdateMealPlanDtoToMealPlanMapper(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    [ObjectFactory]
    private MealPlan CreateMealPlan()
        => new MealPlan(_guidGenerator.Create(), Guid.Empty, DateTime.UtcNow);

    [MapperIgnoreTarget(nameof(MealPlan.Id))]
    [MapperIgnoreTarget(nameof(MealPlan.UserId))]
    [MapperIgnoreTarget(nameof(MealPlan.Entries))]
    [MapperIgnoreTarget(nameof(MealPlan.CreationTime))]
    [MapperIgnoreTarget(nameof(MealPlan.CreatorId))]
    [MapperIgnoreTarget(nameof(MealPlan.LastModificationTime))]
    [MapperIgnoreTarget(nameof(MealPlan.LastModifierId))]
    [MapperIgnoreTarget(nameof(MealPlan.IsDeleted))]
    [MapperIgnoreTarget(nameof(MealPlan.DeletionTime))]
    [MapperIgnoreTarget(nameof(MealPlan.DeleterId))]
    [MapperIgnoreTarget(nameof(MealPlan.ConcurrencyStamp))]
    public override partial MealPlan Map(CreateUpdateMealPlanDto source);

    [MapperIgnoreTarget(nameof(MealPlan.Id))]
    [MapperIgnoreTarget(nameof(MealPlan.UserId))]
    [MapperIgnoreTarget(nameof(MealPlan.Entries))]
    [MapperIgnoreTarget(nameof(MealPlan.CreationTime))]
    [MapperIgnoreTarget(nameof(MealPlan.CreatorId))]
    [MapperIgnoreTarget(nameof(MealPlan.LastModificationTime))]
    [MapperIgnoreTarget(nameof(MealPlan.LastModifierId))]
    [MapperIgnoreTarget(nameof(MealPlan.IsDeleted))]
    [MapperIgnoreTarget(nameof(MealPlan.DeletionTime))]
    [MapperIgnoreTarget(nameof(MealPlan.DeleterId))]
    [MapperIgnoreTarget(nameof(MealPlan.ConcurrencyStamp))]
    public override partial void Map(CreateUpdateMealPlanDto source, MealPlan destination);

}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]

public partial class MealPlanEntryToMealPlanEntryDtoMapper : MapperBase<MealPlanEntry, MealPlanEntryDto>
{

    [MapperIgnoreTarget(nameof(MealPlanEntryDto.RecipeName))]
    public override partial MealPlanEntryDto Map(MealPlanEntry source);
    [MapperIgnoreTarget(nameof(MealPlanEntryDto.RecipeName))]
    public override partial void Map(MealPlanEntry source, MealPlanEntryDto destination);

    public List<MealPlanEntryDto> MapList(ICollection<MealPlanEntry> source)
    {
        return source.Select(Map).ToList();
    }

    public List<MealPlanDayDto> GroupIntoDays(ICollection<MealPlanEntry> entries)
    {
        return entries
             .GroupBy(e => e.DayOfWeek)
            .OrderBy(g => g.Key == DayOfWeek.Sunday ? 7 : (int)g.Key)
            .Select(g => new MealPlanDayDto
            {
                DayOfWeek = g.Key,
                Meals = g.OrderBy(e => e.MealType).Select(Map).ToList()
            })
            .ToList();
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class CreateUpdateMealPlanEntryDtoToMealPlanEntryMapper : MapperBase<CreateUpdateMealPlanEntryDto, MealPlanEntry>
    {
        private readonly IGuidGenerator _guidGenerator;

        public CreateUpdateMealPlanEntryDtoToMealPlanEntryMapper(IGuidGenerator guidGenerator)
        {
            _guidGenerator = guidGenerator;
        }

        [ObjectFactory]
        private MealPlanEntry CreateMealPlanEntry()
            => new MealPlanEntry(id: _guidGenerator.Create(),
                                 mealPlanId: Guid.Empty,
                                 dayOfWeek: default,
                                 mealName: string.Empty,
                                 mealType: default,
                                 recipeName: null,
                                 recipeId :null,
                                 scheduledTime:null);

        [MapperIgnoreTarget(nameof(MealPlanEntry.Id))]
        [MapperIgnoreTarget(nameof(MealPlanEntry.MealPlanId))]
        public override partial MealPlanEntry Map(CreateUpdateMealPlanEntryDto source);

        [MapperIgnoreTarget(nameof(MealPlanEntry.Id))]
        [MapperIgnoreTarget(nameof(MealPlanEntry.MealPlanId))]
        public override partial void Map(CreateUpdateMealPlanEntryDto source, MealPlanEntry destination);
    }
}