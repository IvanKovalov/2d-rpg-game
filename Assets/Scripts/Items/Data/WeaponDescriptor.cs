using System;
using System.Collections.Generic;
using Core.StatSystem;
using Items.Enums;
using UnityEngine;

namespace Items.Data
{
    [Serializable]
    public class WeaponDescriptor : StatChangingItemDescriptor
    {
        [field: SerializeField] public  WeaponType WeaponType { get; private set; }
        
        public WeaponDescriptor(WeaponType weaponType, ItemId itemId, ItemType itemType, Sprite sprite, ItemRarity itemRarity, float price, float level, List<StatModificator> stats) : base(itemId, itemType, sprite, itemRarity, price, level, stats)
        {
            WeaponType = weaponType;
        }
    }
}