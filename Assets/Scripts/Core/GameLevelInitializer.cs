using System;
using System.Collections.Generic;
using System.Linq;
using BattleSystem;
using Core.Services.Updater;
using Drawing;
using Player;
using UnityEngine;
using InputReader;
using Items;
using Items.Data;
using Items.Rarity;
using Items.Storage;
using NPC.Enums;
using NPC.Spawner;
using UI;
using UnityEngine.Serialization;

namespace Core
{
  public class GameLevelInitializer : MonoBehaviour
  {
    [FormerlySerializedAs("_playerEntity")][SerializeField] private PlayerEntityBehavior playerEntityBehavior;
    [SerializeField] private GameUIInputView _gameUIInputView;
    [SerializeField] private ItemRarityDescriptorsStorage _rarityDescriptorsStorage;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private ItemStorage _itemStorage;
    [SerializeField] private Transform _spawner;

    private ExternalDevicesInputReader _externalDevicesInput;
    private PlayerSystem _playerSystem;
    private ProjectUpdater _projectUpdater;
    public ItemsSystem _itemsSystem;
    private DropGenerator _dropGenerator;
    private UIContext _uiContext;
    private LevelDrawer _levelDrawer;
    private EntitySpawner _entitySpawner;

    private List<IDisposable> _disposables;

    private bool _onPause;
    private void Awake()
    {

      _disposables = new List<IDisposable>();
      if (ProjectUpdater.Instance == null)
      {
        _projectUpdater = new GameObject().AddComponent<ProjectUpdater>();
      }
      else
      {
        _projectUpdater = ProjectUpdater.Instance as ProjectUpdater;
      }
      _externalDevicesInput = new ExternalDevicesInputReader();
      _disposables.Add(_externalDevicesInput);

      WeaponsFactory weaponsFactory = new WeaponsFactory(playerEntityBehavior.Attacker);

      _playerSystem = new PlayerSystem(playerEntityBehavior, new List<IEntityInputSource>
            {
                _gameUIInputView,
                _externalDevicesInput
            }, weaponsFactory);

      _disposables.Add(_playerSystem);

      ItemsFactory itemsFactory = new ItemsFactory(_playerSystem.StatsController);
      List<IItemRarityColor> rarityColors = _rarityDescriptorsStorage.RarityDescriptors.Cast<IItemRarityColor>().ToList();
      _itemsSystem = new ItemsSystem(rarityColors, _whatIsPlayer, itemsFactory, _playerSystem.Inventory);
      List<ItemDescriptor> descriptors =
          _itemStorage.ItemScriptables.Select(scriptable => scriptable.ItemDescriptor).ToList();
      _dropGenerator = new DropGenerator(descriptors, playerEntityBehavior, _itemsSystem);

      UIContext.Data data =
          new UIContext.Data(_playerSystem.Inventory, _rarityDescriptorsStorage.RarityDescriptors);
      _uiContext = new UIContext(new List<IWindowsInputSource>()
            {
                _gameUIInputView,
                _externalDevicesInput
            }, data);

      _levelDrawer = new LevelDrawer(LevelId.Level1);
      _levelDrawer.RegisterElement(playerEntityBehavior);

      _entitySpawner = new EntitySpawner(_levelDrawer);
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        _uiContext.CloserCurrentScreen();
        //_projectUpdater.IsPaused = !_projectUpdater.IsPaused;
      }


      if (Input.GetKeyDown(KeyCode.Q))
      {
        _entitySpawner.SpawnEntity(EntityId.Knight, _spawner.position);
      }
    }

    private void OnDestroy()
    {
      foreach (var disposable in _disposables)
      {
        disposable.Dispose();
      }
    }
  }
}