using System;
using System.Collections.Generic;
using Core.StatSystem;
using Items.Enums;
using UnityEngine;

namespace Items.Data
{
    [Serializable]
    public class StatChangingItemDescriptor : ItemDescriptor
    {
        [field: SerializeField] public float Level { get; private set; }
        [field: SerializeField] public List<StatModificator> Stats { get; private set; }

        public StatChangingItemDescriptor(ItemId itemId, ItemType itemType, Sprite sprite, ItemRarity itemRarity, float price, float level, List<StatModificator> stats) : 
            base(itemId, itemType, sprite, itemRarity, price)
        {
            Level = level;
            Stats = stats;
        }
    }
}