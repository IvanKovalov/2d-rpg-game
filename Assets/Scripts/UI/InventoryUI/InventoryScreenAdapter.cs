using System.Collections.Generic;
using System.Linq;
using Items;
using Items.Core;
using Items.Data;
using Items.Enums;
using UI.Core;
using UI.InventoryUI.Element;
using UnityEngine;

namespace UI.InventoryUI
{
    public class InventoryScreenAdapter: ScreenController<InventoryScreenView>
    {
        private readonly Inventory _inventory;
        private readonly List<RarityDescriptor> _rarityDescriptors;
        private readonly Dictionary<ItemSlot, Item> _backPackSlots;
        private readonly Dictionary<EquipmentSlot, Equipment> _equipmentSlots;
        private readonly EquipmentConditionChecker _equipmentConditionChecker;

        private readonly Sprite _emptyBackgroundSprite;
        

        public InventoryScreenAdapter(InventoryScreenView view, Inventory inventory, List<RarityDescriptor> rarityDescriptors) : base(view)
        {
            _inventory = inventory;

            _rarityDescriptors = rarityDescriptors;
            _emptyBackgroundSprite =
                _rarityDescriptors.Find(descriptor => descriptor.ItemRarity == ItemRarity.None).Sprite;
            _equipmentSlots = new Dictionary<EquipmentSlot, Equipment>();
            _backPackSlots = new Dictionary<ItemSlot, Item>();
            _equipmentConditionChecker = new EquipmentConditionChecker();
        }

        public override void Initialize()
        {
            View.MovingImage.gameObject.SetActive(true);
            InitializeBackPack();
            InitializeEquipment();
            _inventory.BackPackChanged += UpdateBackpack;
            _inventory.EquipmentCnahged += UpdateEquipment;
            View.Show();
            View.CloseClicked += Complete;
        }

        public override void Complete()
        {
            View.Hide();
            ClearBackPack();
            ClearEquipment();
            _inventory.BackPackChanged -= UpdateBackpack;
            _inventory.EquipmentCnahged -= UpdateEquipment;
            View.CloseClicked -= Complete;
        }

        private void InitializeBackPack()
        {
            var backPack = View.ItemSlots;
            Debug.Log(backPack.Count);
            for (int i = 0; i < backPack.Count; i++)
            {
                var slot = backPack[i];
                var item = _inventory.BackPackItems[i];
                _backPackSlots.Add(slot, item);

                if (item == null)
                {
                    slot.ClearItem(GetBackSprite(ItemRarity.None));
                    continue; 
                }
                slot.SetItem(item.Descriptor.ItemSprite, GetBackSprite(item.Descriptor.ItemRarity), item.Amount);
                SubscribeToSlotEvents(slot);
            }
        }

        private void InitializeEquipment()
        {
            var equipment = View.EquipmentSlots;
            foreach (var slot in equipment)
            {
                var item = _inventory.Equipment.Find(equip => equip.EquipmentType == slot.EquipmentType);
                /*if(item == null && slot.EquipmentType == EquipmentType.RightHand)
                    item = _inventory.Equipment.Find(equip => equip.EquipmentType == EquipmentType.B)*/
                _equipmentSlots.Add(slot, item);

                /*
                if (slot.EquipmentType == EquipmentType.LeftHand)
                {
                    var twoHandsWeapon = _inventory.Equipment.Find(equip => equip.EquipmentType == )
                }
                */

                if (item == null)
                {
                    slot.ClearItem(GetBackSprite(ItemRarity.None));
                    continue;
                }

                
                
                slot.SetItem(item.Descriptor.ItemSprite, item.Descriptor.ItemSprite, item.Amount);
                SubscribeToSlotEvents(slot);
            }
        }

        private void SubscribeToSlotEvents(ItemSlot slot)
        {
            slot.SlotClicked += UseSlot;
            slot.SlotClearClicked += ClearSlot;
        }

        private Sprite GetBackSprite(ItemRarity rarity) =>
            _rarityDescriptors.Find(desriptor => desriptor.ItemRarity == rarity).Sprite;

        private void UpdateBackpack()
        {
            ClearBackPack();
            InitializeBackPack();
        }

        private void UpdateEquipment()
        {
            ClearEquipment();
            InitializeEquipment();
        }

        private void UseSlot(ItemSlot slot)
        {
            Equipment equipment;
            if (slot is EquipmentSlot equipmentSlot && _inventory.BackPackItems.Any(element => element == null))
            {
                equipment = _equipmentSlots[equipmentSlot];
                _inventory.UnEquip(equipment, false);
                _inventory.AddItemToBackPack(equipment);
                equipment?.Use();
                return;
            }

            Item item = _backPackSlots[slot];

            if (item is Potion potion)
            {
                potion.Use();
                if(potion.Amount <= 0)
                    _inventory.RemoveFromBackpack(item, false);
                
                return;
            }

            if (item is not Equipment equip)
                return;

            equipment = equip;
            
            if(!_equipmentConditionChecker.IsEquipmentConditionFits(equipment, _inventory.Equipment) 
               || !_equipmentConditionChecker.TryReplaceEquipment(equipment, _inventory.Equipment, out var prevEquipment))
                return;
            
            _inventory.RemoveFromBackpack(equipment, false);

            if (prevEquipment != null)
            {
                _inventory.AddItemToBackPack(prevEquipment);
                prevEquipment.Use();
            }
            
            _inventory.Equip(equipment);
            //slot.SetItem(equipment.Descriptor.ItemSprite, GetBackSprite(equipment.Descriptor.ItemRarity), 0);
            equipment.Use();
           
        }

        private void ClearSlot(ItemSlot slot)
        {
            if(_backPackSlots.TryGetValue(slot, out Item item))
                _inventory.RemoveFromBackpack(item, true);
            
            if(slot is EquipmentSlot equipmentSlot && _equipmentSlots.TryGetValue(equipmentSlot, out Equipment equipment))
                _inventory.UnEquip(equipment, true);
        }
        

        private void ClearBackPack()
        {
            ClearSlots(_backPackSlots.Select(item => item.Key).ToList());
            _backPackSlots.Clear();
        }
        
        private void ClearEquipment()
        {
            ClearSlots(_equipmentSlots.Select(item => item.Key).Cast<ItemSlot>().ToList());
            _equipmentSlots.Clear();
        }

        private void ClearSlots(List<ItemSlot> slots)
        {
            foreach (var slot in slots)
            {
                UnsubscribeSlotEvents(slot);
                slot.ClearItem(_emptyBackgroundSprite);
            }
        }
        
        private void UnsubscribeSlotEvents(ItemSlot slot)
        {
            slot.SlotClicked -= UseSlot;
            slot.SlotClearClicked -= ClearSlot;
        }
        
    }
}