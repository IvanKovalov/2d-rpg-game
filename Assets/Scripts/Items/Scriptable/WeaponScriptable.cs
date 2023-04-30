using Items.Data;
using UnityEngine;

namespace Items.Scriptable
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ItemsSystem/Weapon")]
    public class WeaponScriptable : BaseItemScriptable
    {
        [SerializeField] private WeaponDescriptor _weaponDecDescriptor;
        public override ItemDescriptor ItemDescriptor => _weaponDecDescriptor;
    }
}