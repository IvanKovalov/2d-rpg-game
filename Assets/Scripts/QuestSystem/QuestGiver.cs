using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using TMPro;
using NPC.Enums;

namespace QuestSystem
{

  public class QuestGiver : MonoBehaviour
  {
    public Quest quest;
    public PlayerEntityBehavior player;
    [SerializeField] private GameObject questWindow;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI goldText;
    public void OpenQuestWindow()
    {
      questWindow.SetActive(true);
      title.text = quest.title;
      description.text = quest.description;
      goldText.text = quest.goldReward.ToString();
    }

    public void AcceptQuest()
    {
      questWindow.SetActive(false);
      quest.isActive = true;
      player.activeQuest = quest;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.P))
      {
        OpenQuestWindow();
      }
    }
  }
}
