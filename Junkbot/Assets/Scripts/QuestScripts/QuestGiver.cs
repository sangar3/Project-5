using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class QuestGiver : MonoBehaviour
{
   
    public AudioClip sfx1;
    public Quest[] quests = new Quest[10];
    public Quest activeQuest;
    public int questCount = 0;

    public FirstPersonAIO player;

    public GameObject questWindow;

    static Text OBJECTIVE;
    public Text titletext;
    public Text descriptiontext;

    public void OpenQuestWindow()
    { 
        questWindow.SetActive(true);
        activeQuest = quests[questCount];
        titletext.text = activeQuest.title;
        descriptiontext.text = activeQuest.description;
    }

    public void nextQuest()
    {
        questCount++;
        AudioManager.Instance.PlaySFX(sfx1,1.0f);    //add sound fx 
        OpenQuestWindow();


    }


}
