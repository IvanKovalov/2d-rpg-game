using System.Collections.Generic;
using System.Linq;
using Items.Behaviour;
using Items.Core;
using Items.Data;
using Items.Rarity;
using UnityEngine;

namespace Items
{
  public class ItemsSystem
  {
    private SceneItem _sceneItem;
    private Transform _transform;
    private List<IItemRarityColor> _colors;
    private Dictionary<SceneItem, Item> _itemsOnScene;
    private LayerMask _whatIsPlayer;
    public Inventory _inventory;

    private ItemsFactory _itemsFactory;

    public ItemsSystem(List<IItemRarityColor> colors, LayerMask whatIsPlayer, ItemsFactory itemsFactory, Inventory inventory)
    {
      _sceneItem = Resources.Load<SceneItem>($"{nameof(ItemsSystem)}/{nameof(SceneItem)}");
      _itemsOnScene = new Dictionary<SceneItem, Item>();
      GameObject gameObject = new GameObject();
      gameObject.name = nameof(ItemsSystem);
      _transform = gameObject.transform;
      _colors = colors;
      _whatIsPlayer = whatIsPlayer;
      _itemsFactory = itemsFactory;
      _inventory = inventory;
      _inventory.ItemDropped += DropItem;
    }

    public void DropItem(ItemDescriptor descriptor, Vector2 position) => DropItem(_itemsFactory.CreateItem(descriptor), position);


    private void DropItem(Item item, Vector2 position)
    {
      SceneItem sceneItem = Object.Instantiate(_sceneItem, _transform);
      sceneItem.SetItem(item.Descriptor.ItemSprite, item.Descriptor.ItemId.ToString(), _colors.Find(color => color.ItemRarity == item.Descriptor.ItemRarity).Color);
      sceneItem.PlayDrop(position);
      sceneItem.ItemClicked += TryToPickItem;
      _itemsOnScene.Add(sceneItem, item);
    }


    private void TryToPickItem(SceneItem sceneItem)
    {
      Collider2D player = Physics2D.OverlapCircle(sceneItem.Position, sceneItem.InteractionDistance, _whatIsPlayer);
      if (player == null)
        return;

      Item item = _itemsOnScene[sceneItem];

      /*EquipmentConditionChecker equipmentConditionChecker = new EquipmentConditionChecker();

      if (item is Equipment equipment &&
          _inventory.Equipment.All(element => element.EquipmentType != equipment.EquipmentType) &&
          equipmentConditionChecker.IsEquipmentConditionFits(equipment, _inventory.Equipment))
      {
          _inventory.Equip(equipment);
      } else if (_inventory.BackPackItems.All(one => one != null))
      {
          return;
      }*/

      if (_inventory.TryAddToInventory(item))
      {
        _itemsOnScene.Remove(sceneItem);
        sceneItem.ItemClicked -= TryToPickItem;
        Object.Destroy(sceneItem.gameObject);
        return;
      }


      _inventory.AddItemToBackPack(item);


      /*if (!_inventory.TryAddToInventory(item))
          return;

      Debug.Log($"Adding item {item.Descriptor.ItemId} to inventory");*/
      _itemsOnScene.Remove(sceneItem);
      sceneItem.ItemClicked -= TryToPickItem;
      Object.Destroy(sceneItem.gameObject);

    }
  }
}