using System.Collections.Generic;
using System.Linq;
using Core.Services.Updater;
using Items.Data;
using Items.Enums;
using Player;
using UnityEngine;

namespace Items
{
    public class DropGenerator
    {
        private PlayerEntityBehavior playerEntityBehavior;
        private List<ItemDescriptor> _itemDescriptors;
        private ItemsSystem _itemsSystem;

        public DropGenerator(List<ItemDescriptor> itemDescriptors, PlayerEntityBehavior playerEntityBehavior, ItemsSystem itemsSystem)
        {
            this.playerEntityBehavior = playerEntityBehavior;
            _itemDescriptors = itemDescriptors;
            _itemsSystem = itemsSystem;
            ProjectUpdater.Instance.UpdateCalled += Update;
        }

        public void DropRandomItem(ItemRarity rarity)
        {
            List<ItemDescriptor> items = _itemDescriptors.Where(item => item.ItemRarity == rarity).ToList();
            ItemDescriptor itemDescriptor = items[Random.Range(0, items.Count())];
            _itemsSystem.DropItem(itemDescriptor, playerEntityBehavior.transform.position + Vector3.one);
        }

        public ItemRarity GetDropRarity()
        {
            float chance = Random.Range(0, 100);
            return chance switch
            {
                <= 40 => ItemRarity.Trash,
                > 40 and <= 75 => ItemRarity.Common,
                > 75 and <= 90 => ItemRarity.Rare,
                > 90 and <= 97 => ItemRarity.Legendary,
                > 97 and <= 100 => ItemRarity.Epic,
                _ => ItemRarity.Trash
            };
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.G))
                DropRandomItem(GetDropRarity());
        }
    }
}