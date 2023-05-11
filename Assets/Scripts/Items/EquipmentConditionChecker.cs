using System;
using System.Collections.Generic;
using Items.Core;
using Items.Enums;

namespace Items
{
    public class EquipmentConditionChecker
    {
        public bool IsEquipmentConditionFits(Equipment equipment, List<Equipment> currentEquipment)
        {
            return true;
        }

        public bool TryReplaceEquipment(Equipment equipment, List<Equipment> currentEquipment,
            out Equipment oldEquipment)
        {
            oldEquipment = currentEquipment.Find(slot => slot.EquipmentType == equipment.EquipmentType);
            if (oldEquipment != null)
                return true;

            switch (equipment.EquipmentType)
            {
                case EquipmentType.LeftHand:
                case EquipmentType.RightHand:
                case EquipmentType.Armor:
                case EquipmentType.Boots:
                case EquipmentType.Helmet:
                    return true;
                case EquipmentType.None:
                default:
                    throw new NullReferenceException($"Equipment type of item {equipment.Descriptor}");
            }
        }
    }
}