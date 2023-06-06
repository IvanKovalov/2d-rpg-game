using BattleSystem.Weapon;
using Items;
using Items.Enums;

namespace BattleSystem
{
    public class WeaponsFactory
    {
        
        private readonly Attacker _meleeAttacker;

        public WeaponsFactory(Attacker meleeAttacker)
        {
            _meleeAttacker = meleeAttacker;
        }

        public BaseWeapon GetWeapon(ItemId itemId)
        {
            ItemType itemType = itemId.GetItemType();
            switch (itemType)
            {
                case ItemType.Weapon:
                    return new SingleTargetWeapon(_meleeAttacker);
            }

            return null;
        }
    }
}