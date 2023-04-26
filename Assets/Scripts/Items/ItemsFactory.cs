using System;
using Core.StatSystem;
using Items.Core;
using Items.Data;
using Items.Enums;

namespace Items
{
    public class ItemsFactory
    {
        private readonly StatsController _statsController;
        public ItemsFactory(StatsController statsController) => _statsController = statsController;

        public Item CreateItem(ItemDescriptor descriptor)
        {
            switch (descriptor.Type)
            {
                case ItemType.Potion:
                    return new Potion(descriptor, _statsController);
                case ItemType.Armor:
                case ItemType.Shield:
                case ItemType.Weapon:
                    return new Equipment(descriptor, _statsController, GetEquipmentType(descriptor));
                default:
                    throw new NullReferenceException($"Item type {descriptor.Type}");
            }
        }

        public EquipmentType GetEquipmentType(ItemDescriptor descriptor)
        {
            switch (descriptor.Type)
            {
                case ItemType.Armor:
                    return EquipmentType.Armor;
                case ItemType.Shield:
                    return EquipmentType.LeftHand;
                case ItemType.Weapon:
                    var weaponDescriptor = descriptor as WeaponDescriptor;
                    switch (weaponDescriptor.WeaponType)
                    {
                        case WeaponType.Caster:
                        case WeaponType.OneHand:
                            return EquipmentType.RightHand;
                    }

                    throw new NullReferenceException("Weapon has wrong type");
                case ItemType.Misc:
                case ItemType.None:
                case ItemType.Potion:
                default:
                    return EquipmentType.None;

            }
        }
        
    }
}