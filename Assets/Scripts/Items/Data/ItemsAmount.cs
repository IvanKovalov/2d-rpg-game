using System;
using Items.Enums;
using UnityEngine;

namespace Items.Data
{
    [Serializable]
    public class ItemsAmount
    {
        [field: SerializeField] public ItemId ItemId { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
    }
}