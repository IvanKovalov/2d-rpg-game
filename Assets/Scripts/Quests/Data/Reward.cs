using System;
using System.Collections.Generic;
using Core.StatSystem;
using Items.Data;
using UnityEngine;

namespace Quests.Data
{
    [Serializable]
    public class Reward
    {
        [field: SerializeField] public List<ItemsAmount> Items { get; private set; }
        [field: SerializeField] public List<StatModificator> Stats { get; private set; }

        public List<string> GetRewardsTexts()
        {
            var texts = new List<string>();
            foreach (var item in Items)
                texts.Add($"{item.ItemId}: {item.Amount}");
            
            foreach (var statModificator in Stats)
            {
                var increase = statModificator.Type == StatModificatorType.Additive ? "+" : "+%";
                var text = $"{statModificator.Stat.Type} {increase} {statModificator.Stat.Value}";
                if (statModificator.Duration > 0)
                {
                    text = $"Increase for {statModificator.Duration} seconds of {text}";
                }
                texts.Add(text);
            }

            return texts;
        }
    }
}