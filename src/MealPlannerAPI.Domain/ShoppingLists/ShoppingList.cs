using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.ShoppingLists
{
    public class ShoppingList : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
        protected ShoppingList() { }
        public ShoppingList(Guid id, Guid userId, string name) : base(id)
        {
            UserId = userId;
            Name = name;
        }

        public ShoppingListItem AddItem(Guid id,
                                        string name,
                                        decimal quantity,
                                        bool isCompleted,
                                        string unit,
                                        ShoppingItemCategory category)
        {
            var item = new ShoppingListItem(id: id,
                                            shoppingListId: Id,
                                            name: name,
                                            isCompleted: isCompleted,
                                            quantity: quantity,
                                            unit: unit,
                                            category: category);
            Items.Add(item);
            return item;
        }

        public void UpdateItem(Guid itemId, string name, decimal quantity, string unit, ShoppingItemCategory category)
        {
            var item = FindItem(itemId);
            item.Name = name;
            item.Quantity = quantity;
            item.Unit = unit;
            item.Category = category;
        }

        public void RemoveItem(Guid itemId)
        {
            var item = FindItem(itemId);
            Items.Remove(item);
        }

        public void ToggleItem(Guid itemId)
        {
            var item = FindItem(itemId);
            item.IsCompleted = !item.IsCompleted;
        }

        public void MarkAllCompleted()
        {
            foreach (var item in Items)
                item.IsCompleted = true;
        }
        public void ReplaceItems(IEnumerable<(Guid Id, string Name, bool IsCompleted, decimal Quantity, string Unit, ShoppingItemCategory Category)> items)
        {
            Items.Clear();
            foreach (var i in items)
                AddItem(id: i.Id, name: i.Name, quantity: i.Quantity, unit: i.Unit, category: i.Category, isCompleted: i.IsCompleted);
        }
        private ShoppingListItem FindItem(Guid itemId)
        {
            return Items.FirstOrDefault(i => i.Id == itemId)
                ?? throw new EntityNotFoundException(typeof(ShoppingListItem), itemId);
        }
    }
}
