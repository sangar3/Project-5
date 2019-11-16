using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public FirstPersonAIO player;
    public  ObjectManipulation playerObjManip;
    //public  ObjectManager playerManager;
    public QuestGiver questUI;
    public Canvas questMarker;

    public float horizontal;

    // Start is called before the first frame update
    void Start()
    {
        playerObjManip = player.GetComponent<ObjectManipulation>();
        //playerManager = player.GetComponent<ObjectManager>();
        questUI.OpenQuestWindow();
    }

    // Update is called once per frame
    void Update()
    {
        if (questUI.questCount == 0 && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {   //mouse looking -> wasd movement
            questUI.nextQuest();
        }

        if (questUI.questCount == 1 && player.transform.position.z >= 10)
        {   //wasd movement -> jumping
            questUI.nextQuest();
            
        }

        if (questUI.questCount == 2 && player.transform.position.z >= 19)
        {   //jumping -> box pickup
            questUI.nextQuest();
            questMarker.gameObject.SetActive(true);
            questMarker.transform.position = GameObject.Find("Box").transform.position;
            //playerManager.resetPos = new Vector3();
            //playerManager.resetRot = new Quaternion();
            questMarker.transform.Translate(Vector3.up * 2.5f);
        }

        if (questUI.questCount == 3 && playerObjManip.selected != null)
        {   //picking up -> progress
            questUI.nextQuest();
            questMarker.transform.position = new Vector3(-7.5f, -4f, 40f);

          

        }

        if (questUI.questCount == 4 && player.transform.position.z >= 40)
        {   //progress -> rotate object
            questUI.nextQuest();
            questMarker.transform.position = GameObject.Find("Plank").transform.position;
            questMarker.transform.Translate(0.25f, 2f, 0f);
           
        }

        if (questUI.questCount == 5 && (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
        {   //rotate object -> progress
            questUI.nextQuest();
            questMarker.transform.position = new Vector3(1.5f, -7f, 55.5f);
        }

        if (questUI.questCount == 6 && player.transform.position.z >= 53)
        {   //progress -> progress
            questUI.nextQuest();
            questMarker.transform.position = new Vector3(-.25f, -7.25f, 71f);
        }

        if (questUI.questCount == 7 && (player.transform.position.y >= -8 && player.transform.position.z >= 69))
        {   //progress -> ball puzzle
            questUI.nextQuest();
            questMarker.transform.position = new Vector3(-3f, -4f, 90f);
        }
    }
}
