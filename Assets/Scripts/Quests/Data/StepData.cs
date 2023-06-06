using System;
using UnityEngine;
namespace Quests.Data
{
    [Serializable]
    public class StepData
    {
        [field: SerializeField] public string StepDescription { get; private set; }
        [field: SerializeField] public string TokenId { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
    }
}
