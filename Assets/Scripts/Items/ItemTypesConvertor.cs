using Items.Core;
using Items.Enums;

namespace Items
{
    public static class ItemTypesConverter
    {
        private const int MaxOneHandMeleeWeaponsIndex = 500;
        private const int MaxTwoHandsMeleeWeaponsIndex = 1000;
        private const int MaxArmorsIndex = 1200;
        private const int MaxShieldsIndex = 1700;
        private const int MaxPotionsIndex = 1800;

        public static ItemType GetItemType(this Item item) => item.Descriptor.ItemId.GetItemType();

        public static ItemType GetItemType(this ItemId itemId)
        {
            return (int)itemId switch
            {
                <= MaxOneHandMeleeWeaponsIndex => ItemType.Weapon,
                <= MaxArmorsIndex => ItemType.Armor,
                <= MaxShieldsIndex => ItemType.Shield,
                <= MaxPotionsIndex => ItemType.Potion,
                _ => ItemType.Misc
            };
        }

        public static bool IsItemTypeEqual(this ItemId itemId, ItemId otherItemId)
            => itemId.GetItemType() == otherItemId.GetItemType();
        
        public static bool IsItemTypeEqual(this ItemId itemId, ItemType itemType)
            => itemId.GetItemType() == itemType;

        public static bool IsWeapon(this Item item) =>
            item.GetItemType().IsWeapon();
        
        public static bool IsWeapon(this ItemId itemId) =>
            itemId.GetItemType().IsWeapon();
        public static bool IsWeapon(this ItemType itemType) =>
            itemType == ItemType.Weapon;
        public static EquipmentType GetItemInventorySlotType(this Item item)
        {
            switch (item.GetItemType())
            {
                case ItemType.Armor:
                    return EquipmentType.Armor;
                case ItemType.Shield:
                    return EquipmentType.LeftHand;
                case ItemType.Weapon:
                    return EquipmentType.RightHand;
                case ItemType.Misc:
                case ItemType.None:
                case ItemType.Potion:
                default:
                    return EquipmentType.None;
            }
        }

        public static bool IsEquipmentSlotEqual(this Item item, EquipmentType slotType) =>
            item.GetItemInventorySlotType() == slotType;

        public static bool IsEquipmentSlotEqual(this Item item, Item otherItem) =>
            item.GetItemInventorySlotType() == otherItem.GetItemInventorySlotType();
    }
}