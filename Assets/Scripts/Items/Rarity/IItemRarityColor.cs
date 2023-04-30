using Items.Enums;
using UnityEngine;

namespace Items.Rarity
{
    public interface IItemRarityColor
    {
        ItemRarity ItemRarity { get; }
        Color Color { get; }
    }
}