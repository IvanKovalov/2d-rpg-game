using System;
using System.Collections.Generic;
using System.Linq;
using Items.Core;
using UnityEditor;
using UnityEngine;

namespace Items
{
    public class Inventory
    {
        public const int InventorySize = 28;
        private readonly EquipmentConditionChecker _equipmentFitter;
        private readonly Transform _player;
        public List<Item> BackPackItems { get; }
        public List<Equipment> Equipment { get; }

        public event Action BackPackChanged;
        public event Action EquipmentCnahged;
        public event Action<Item, Vector2> ItemDropped; 

        public Inventory(List<Item> backPackItems, List<Equipment> equipment, Transform player, EquipmentConditionChecker equipmentFitter)
        {
            /*_equipmentFitter = new EquipmentConditionChecker();
            _player = player;
            Equipment = equipment ?? new List<Equipment>();
           BackPackItems = new List<Item>();
           //BackPackItems = backPackItems;
            for (int i = 0; i < InventorySize; i++)
                BackPackItems.Add(null);
            
            Equipment = new List<Equipment>();
            _player = player;*/
            
            _equipmentFitter = equipmentFitter;
            BackPackItems = new List<Item>();
            for (var i = 0; i < InventorySize; i++)
                BackPackItems.Add(null);

            Equipment = new List<Equipment>();
            _player = player;
        }
        
        public bool TryAddToInventory(Item item)
        {
            if (item is Equipment equipment
                && Equipment.All(equip => equip.EquipmentType != equipment.EquipmentType)
                && TryEquip(equipment))
                return true;

            return TryAddToBackPack(item);
        }
        
        public bool TryEquip(Item item)
        {
            if(!(item is Equipment equipment))
                return false;

            if (!_equipmentFitter.IsEquipmentConditionFits(equipment, Equipment))
                return false;

            #region InventoryScreen
            if (!_equipmentFitter.TryReplaceEquipment(equipment, Equipment, out var oldEquipment))
                return false;
                
            if(oldEquipment != null)
                UnEquip(oldEquipment, false);

            if (BackPackItems.Contains(equipment))
            {
                var indexOfItem = BackPackItems.IndexOf(equipment);
                PlaceToBackPack(oldEquipment, indexOfItem);
            }
            else TryAddToBackPack(oldEquipment);
            #endregion
            
            Equipment.Add(equipment);
            equipment.Use();
            EquipmentCnahged?.Invoke();
            return true;
        }

        public void UseItem(Item item)
        {
            if (item is Potion potion)
            {
                potion.Use();
                if (potion.Amount <= 0)
                    RemoveItem(item, false);
                return;
            }

            if (item is not Equipment equipment)
                return;

            if (Equipment.Contains(equipment))
            {
                if(TryAddToBackPack(equipment))
                    UnEquip(equipment, true);
                
                return;
            }

            if (!TryEquip(equipment))
                return;

            BackPackItems.Remove(item);
            BackPackChanged?.Invoke();
        }
        
        public void RemoveItem(Item item, bool toWorld)
        {
            if(item is Equipment equipment && Equipment.Contains(equipment))
                UnEquip(equipment, false);
            else
                RemoveFromBackpack(item, true);
            
            if(toWorld)
                ItemDropped?.Invoke(item, _player.position);
        }

        public void MoveItemToPositionBackPack(Item item, int place)
        {
            if (item is Equipment equipment)
            {
                var backPackItem = BackPackItems[place];
                if (backPackItem != null)
                {
                    TryEquip(backPackItem);
                    return;
                }

                if (TryPlaceToBackPack(item, place))
                {
                    UnEquip(equipment);
                }
                return;
            }
            TryPlaceToBackPack(item, place);
        }

        private void UnEquip(Equipment equipment)
        {
            Equipment.Remove(equipment);
            equipment.Use();
            EquipmentCnahged?.Invoke();
        }

        private bool TryAddToBackPack(Item item)
        {
            if (BackPackItems.All(slot => slot != null))
                return false;

            var index = BackPackItems.IndexOf(null);
            PlaceToBackPack(item, index);
            return true;
        }
        
        private bool TryPlaceToBackPack(Item item, int index)
        {
            var oldItem = BackPackItems[index];
            if (BackPackItems.Contains(item))
            {
                var indexOfItem = BackPackItems.IndexOf(item);
                BackPackItems[indexOfItem] = oldItem;
            }
            else if (oldItem != null)
                return false;
            
            BackPackItems[index] = item;
            BackPackChanged?.Invoke();
            return true;
        }

        public void AddItemToBackPack(Item item)
        {
            var index = BackPackItems.FindIndex(element => element == null);
            BackPackItems[index] = item;
            BackPackChanged?.Invoke();
        }
        
        private void PlaceToBackPack(Item item, int index)
        {
            BackPackItems[index] = item;
            BackPackChanged?.Invoke();
        }

        public void RemoveFromBackpack(Item item, bool toWorld)
        {
            var index = BackPackItems.IndexOf(item);
            BackPackItems[index] = null;
            BackPackChanged?.Invoke();
            
            if (toWorld)
            {
                ItemDropped?.Invoke(item, _player.position);
            }
        }

        public void Equip(Equipment equipment)
        {
            Equipment.Add(equipment);
            EquipmentCnahged?.Invoke();
        }
        
        public void UnEquip(Equipment equipment, bool toWorld)
        {
            Equipment.Remove(equipment);
            equipment.Use();
            EquipmentCnahged?.Invoke();

            if (toWorld)
            {
                ItemDropped?.Invoke(equipment, _player.position);
            }
        }
    }
}