using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Items.Core;
using Items.Data;
using Items.Enums;
using Spine;
using UI.Core;
using UI.InventoryUI.Element;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
        private ItemSlot _focusedSlot;
        private Item _movingItem;
        private float _slotClickTime;
        

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
            View.MovingImage.gameObject.SetActive(false);
            InitializeBackPack();
            InitializeEquipment();
            _inventory.BackPackChanged += UpdateBackpack;
            _inventory.EquipmentCnahged += UpdateEquipment;
            base.Initialize();
        }

        public override void Complete()
        {
            ClearBackPack();
            ClearEquipment();
            _inventory.BackPackChanged -= UpdateBackpack;
            _inventory.EquipmentCnahged -= UpdateEquipment;
            base.Complete();
        }

        private void InitializeBackPack()
        {
            var backPack = View.ItemSlots;
            for (int i = 0; i < Inventory.InventorySize; i++)
            {
                var slot = backPack[i];
               
                var item = _inventory.BackPackItems[i];
                _backPackSlots.Add(slot, item);

                if (item == null) 
                    continue;
                
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
                    //slot.ClearItem(GetBackSprite(ItemRarity.None));
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

            slot.SlotClickedDown += OnSlotDown;
            slot.DragStarted += OnDragStarted;
            slot.Dragged += OnDragged;
            slot.DragEnded += DragEnded;
        }

        private void DragEnded(ItemSlot slot, Vector2 position)
        {
            if (_focusedSlot != slot)
            {
                return;
            }

            var item = _movingItem;
            _focusedSlot = null;
            _movingItem = null;
            View.MovingImage.gameObject.SetActive(false);
            slot.SetItem(item.Descriptor.ItemSprite, GetBackSprite(item.Descriptor.ItemRarity), item.Amount);

            if (!TryGetSlotOnPosition(position, out var anotherSlot))
            {
                return;
            }

            switch (anotherSlot)
            {
                case EquipmentSlot when slot is EquipmentSlot:
                    return;
                case EquipmentSlot:
                    _inventory.TryEquip(item);
                    return;
                default:
                {
                    var newPlace = _backPackSlots.Keys.ToList().IndexOf(anotherSlot);
                    _inventory.MoveItemToPositionBackPack(item, newPlace);
                    break;
                }
            }
        }

        private bool TryGetSlotOnPosition(Vector2 position, out ItemSlot itemSlot)
        {
            itemSlot = null;
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                { position = position };
            List<RaycastResult> results = new List<RaycastResult>();
           EventSystem.current.RaycastAll(pointerEventData, results);
            if (results.Count < 1)
            {
                return false;
            }

            foreach (var result in results)
            {
                if (result.gameObject.TryGetComponent(out itemSlot))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDragged(ItemSlot slot, Vector2 position)
        {
            if (_focusedSlot != slot)
            {
                return;
            }

            View.MovingImage.rectTransform.position = position;
        }

        private void OnDragStarted(ItemSlot slot)
        {
            if (_focusedSlot != slot)
            {
                return;
            }
            
            slot.ClearItem(_emptyBackgroundSprite);
            TryGetItem(slot, out _movingItem);
            View.MovingImage.gameObject.SetActive(true);
            View.MovingImage.sprite = _movingItem.Descriptor.ItemSprite;
        }

        private void OnSlotDown(ItemSlot slot)
        {
            if (_focusedSlot != null)
            {
                return;
            }

            _focusedSlot = slot;
        }

        private Sprite GetBackSprite(ItemRarity rarity) =>
            _rarityDescriptors.Find(descriptor => descriptor.ItemRarity == rarity).Sprite;

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
            if (slot == _focusedSlot)
            {
                if (TryGetItem(slot, out var item))
                    _inventory.UseItem(item);
            }

            _focusedSlot = null;
        }

        private bool TryGetItem(ItemSlot slot, out Item item)
        {
            item = null;
            if (slot is EquipmentSlot equipmentSlot)
            {
                item = _equipmentSlots[equipmentSlot];
                return item != null;
            }

            item = _backPackSlots[slot];
            return item != null;
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
            
            slot.SlotClickedDown -= OnSlotDown;
            slot.DragStarted -= OnDragStarted;
            slot.Dragged -= OnDragged;
            slot.DragEnded -= DragEnded;
        }
    }
}