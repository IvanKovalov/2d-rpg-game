using System.Collections;
using System.Collections.Generic;
using Core;
using NPC.Behaviour;
using NPC.Controller;
using Player;
using TMPro;
using UnityEngine;

namespace QuestSystem
{
  [System.Serializable]
  public class QuestGoal
  {
    public GoalType goalType;

    public int requiredAmount;
    public int currentAmount;

    public bool isReached()
    {
      return currentAmount >= requiredAmount;
    }
    public void EnemyKilled(Entity entity)
    {
      if (goalType == GoalType.Kill)
        currentAmount++;
      if (isReached())
      {
        var playerEntityBehaviour = UnityEngine.GameObject.FindObjectOfType<Player.PlayerEntityBehavior>();
        var levelInitializer = UnityEngine.GameObject.FindObjectOfType<GameLevelInitializer>();
        levelInitializer._itemsSystem._inventory.goldAmount += playerEntityBehaviour.activeQuest.goldReward;
        //Debug.Log(levelInitializer._itemsSystem._inventory.goldAmount + " gold we have now");
        UnityEngine.GameObject.FindGameObjectWithTag("Gold Balance Text").GetComponent<TextMeshProUGUI>().text = levelInitializer._itemsSystem._inventory.goldAmount.ToString();
        playerEntityBehaviour.activeQuest.Complete();
        //ObjectDied -= player.activeQuest.goal.EnemyKilled;
      }
    }
  }
  public enum GoalType
  {
    Kill,
    Gather,
    Explore,
    Other
  }
}
