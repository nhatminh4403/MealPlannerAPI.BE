using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.ShoppingLists.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Guids;
using Volo.Abp.Mapperly;

namespace MealPlannerAPI.Mappings.ShoppingLists;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ShoppingListToShoppingListDtoMapper
    : MapperBase<ShoppingList, ShoppingListDto>
{
    public override partial ShoppingListDto Map(ShoppingList source);

    public override partial void Map(ShoppingList source, ShoppingListDto destination);
}

// ── ShoppingItem → ShoppingItemDto ───────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ShoppingItemToShoppingItemDtoMapper
    : MapperBase<ShoppingListItem, ShoppingListItemDto>
{
    public override partial ShoppingListItemDto Map(ShoppingListItem source);

    public override partial void Map(ShoppingListItem source, ShoppingListItemDto destination);

    public List<ShoppingListItemDto> MapList(ICollection<ShoppingListItem> source)
        => source.Select(Map).ToList();
}

// ── CreateUpdateShoppingListDto → ShoppingList ────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateShoppingListDtoToShoppingListMapper
    : MapperBase<CreateUpdateShoppingListDto, ShoppingList>
{
    private readonly IGuidGenerator _guidGenerator;

    public CreateUpdateShoppingListDtoToShoppingListMapper(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    [ObjectFactory]
    private ShoppingList CreateShoppingList()
        => new ShoppingList(_guidGenerator.Create(), Guid.Empty, null);

    [MapperIgnoreTarget(nameof(ShoppingList.Id))]
    [MapperIgnoreTarget(nameof(ShoppingList.UserId))]
    [MapperIgnoreTarget(nameof(ShoppingList.Items))]
    [MapperIgnoreTarget(nameof(ShoppingList.CreationTime))]
    [MapperIgnoreTarget(nameof(ShoppingList.CreatorId))]
    [MapperIgnoreTarget(nameof(ShoppingList.LastModificationTime))]
    [MapperIgnoreTarget(nameof(ShoppingList.LastModifierId))]
    [MapperIgnoreTarget(nameof(ShoppingList.IsDeleted))]
    [MapperIgnoreTarget(nameof(ShoppingList.DeletionTime))]
    [MapperIgnoreTarget(nameof(ShoppingList.DeleterId))]
    public override partial ShoppingList Map(CreateUpdateShoppingListDto source);

    [MapperIgnoreTarget(nameof(ShoppingList.Id))]
    [MapperIgnoreTarget(nameof(ShoppingList.UserId))]
    [MapperIgnoreTarget(nameof(ShoppingList.Items))]
    [MapperIgnoreTarget(nameof(ShoppingList.CreationTime))]
    [MapperIgnoreTarget(nameof(ShoppingList.CreatorId))]
    [MapperIgnoreTarget(nameof(ShoppingList.LastModificationTime))]
    [MapperIgnoreTarget(nameof(ShoppingList.LastModifierId))]
    [MapperIgnoreTarget(nameof(ShoppingList.IsDeleted))]
    [MapperIgnoreTarget(nameof(ShoppingList.DeletionTime))]
    [MapperIgnoreTarget(nameof(ShoppingList.DeleterId))]
    public override partial void Map(CreateUpdateShoppingListDto source, ShoppingList destination);
}

// ── CreateUpdateShoppingItemDto → ShoppingItem ────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateShoppingItemDtoToShoppingItemMapper
    : MapperBase<CreateUpdateShoppingListItemDto, ShoppingListItem>
{
    private readonly IGuidGenerator _guidGenerator;

    public CreateUpdateShoppingItemDtoToShoppingItemMapper(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    [ObjectFactory]
    private ShoppingListItem CreateShoppingItem()
        => new ShoppingListItem(
            _guidGenerator.Create(),
            Guid.Empty,
            string.Empty,
            false,            // isCompleted (bool) - required by constructor
            0m,               // quantity (decimal)
            string.Empty,     // unit
            default(Enums.ShoppingItemCategory) // category
        );

    [MapperIgnoreTarget(nameof(ShoppingListItem.Id))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.ShoppingListId))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.CreationTime))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.CreatorId))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.LastModificationTime))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.LastModifierId))]
    public override partial ShoppingListItem Map(CreateUpdateShoppingListItemDto source);

    [MapperIgnoreTarget(nameof(ShoppingListItem.Id))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.ShoppingListId))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.CreationTime))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.CreatorId))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.LastModificationTime))]
    [MapperIgnoreTarget(nameof(ShoppingListItem.LastModifierId))]
    public override partial void Map(CreateUpdateShoppingListItemDto source, ShoppingListItem destination);
}


