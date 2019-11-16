using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    private static GameManager2 _instance;

    public static GameManager2 Instance { get { return _instance; } }

    public struct Level
    {
        public Vector3 initPos;                 //the place the player will be spawned at level start 
        public Vector3 initRot;                 //the rotation at which the player will be spawned at level start

        public MoveableObject[] objects;        //moveable objects that will need to be respawen if the player resets

        public List<Checkpoint> sections;       //List of checkpoints for this level

        private bool _completed;
        public bool completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        public Level(Checkpoint cp)
        {
            initPos = cp.respawnPos;
            initRot = cp.respawnRot;

            objects = FindObjectsOfType<MoveableObject>();

            sections = new List<Checkpoint>();
            sections.Add(cp);

            _completed = false;
        }
    }

    public struct Checkpoint
    {
        public Vector3 respawnPos;              //where the player will be placed if they reset during this checkpoint
        public Vector3 respawnRot;              //respawn rotation

        public Vector3[] pCollider;             //used for moving the ProgressCollider gameobject and setting its size
                                                //it will be used as a trigger to track when the player has reached the
                                                //next checkpoint

        public Checkpoint(Vector3 rPos, Vector3 rRot, Vector3 cPos, Vector3 cSize)
        {
            respawnPos = rPos;
            respawnRot = rRot;

            pCollider = new Vector3[2];
            pCollider[0] = cPos;
            pCollider[1] = cSize;
        }
    }

    private Level[] levels = new Level[5];
    private ProgressTrigger progressTrigger;

    private int currentLevel;
    private int currentSection;

    private FirstPersonAIO player;
    public ObjectManipulation playerObjManip;
    public QuestGiver questUI;
    public GameObject questMarker;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        player = FindObjectOfType<FirstPersonAIO>();
        playerObjManip = player.GetComponent<ObjectManipulation>();
        progressTrigger = FindObjectOfType<ProgressTrigger>();
        questMarker = GameObject.FindGameObjectWithTag("CheckpointMarker");

        questUI = FindObjectOfType<QuestGiver>();
        questUI.OpenQuestWindow();

        SetLevels();
        currentLevel = 0;
        currentSection = 0;

        progressTrigger.transform.position = levels[0].sections[0].pCollider[0];
        progressTrigger.GetComponent<BoxCollider>().size = levels[0].sections[0].pCollider[1];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            PlayerReset();

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

    private void SetLevels()    //...and also checkpoints
    {
        //create first Checkpoint (player's first spawn)
        //use first Checkpoint to create first level
        levels[0] = new Level(new Checkpoint(player.transform.position, player.transform.rotation.eulerAngles, new Vector3(-8, -8, 46.6f), new Vector3(4.2f, 2, 4)));

        //add other checkpoints to first level
        levels[0].sections.Add(new Checkpoint(new Vector3(-9.319f, -8.314f, 45.46f), new Vector3(0, -320.211f, 0), new Vector3(4.42f, -8.01f, 66.47f), new Vector3(4.2f, 4, 4)));
        ///levels[0].sections.Add(new Checkpoint(/*player pos*/, /*player rot*/, /*pCollider pos*/, /*pCollider size*/));
        //repeat above for however many levels and checkpoints we want
    }

    public void EndLevel()
    {
        //transition out of current level
        //record the completion of the level
        levels[currentLevel].completed = true;
        //load hubworld
        //spawn player in hubworld
    }

    public void NextCheckpoint()
    {
        //iterate through Levels[current].sections[]
        currentSection++;

        //move ProgressCollider to the place indicated in the new section
        progressTrigger.transform.position = levels[currentLevel].sections[currentSection].pCollider[0];
        progressTrigger.GetComponent<BoxCollider>().size = levels[currentLevel].sections[currentSection].pCollider[1];

        //update object respawn positions to match where they were when the checkpoint iterated???
        foreach (MoveableObject O in levels[currentLevel].objects)
        {
            O.SetRespawn(O.transform.position, O.transform.rotation);
        }
    }

    private void PlayerReset()
    {
        //respawn player
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = levels[currentLevel].sections[currentSection].respawnPos;
        player.transform.eulerAngles = levels[currentLevel].sections[currentSection].respawnRot;

        //respawn objects
        foreach (MoveableObject O in levels[currentLevel].objects)
        {
            O.ObjectRespawn();
        }

        playerObjManip.DropSelected();
    }
}
