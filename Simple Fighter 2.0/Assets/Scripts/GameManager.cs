﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Events;

public class GameManager : MonoBehaviour
{
    #region Public Variables
    
    public static GameManager Instance = null; //Our Singleton
    public Vector3[] StartingPositions = {new Vector3(-3.5f, 0f, 0f), new Vector3(3.5f, 0f, 0f)};
    public ManagerState CurrentManagerState;
    #endregion
    
    #region Gameplay Variables
   
    private Player[] players;

    private GameObject model;
    private GameObject controller;
    private GameObject view;
    #endregion

    #region UI Variables

    private GameObject uiCanvas;
    private GameObject[] healthBars;
    private GameObject timer;
    #endregion
    
    
    #region Round Information

    public int TotalRounds = 3;
    public int roundNum;
    private int setNum;
    
    #endregion
    
    #region Starting Functions
    private void Awake()
    {
        //Set up the singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        EventManager.Instance.AddHandler<HealthChanged>(OnHealthChanged);
        EventManager.Instance.AddHandler<ProcessInput>(OnInput);
        healthBars = new GameObject[2];
        roundNum = 1;
        CurrentManagerState = ManagerState.Start;
        Init();
    }

    //Set up variables
    private void Init()
    {
        model = GameObject.Find("Model");
        view = GameObject.Find("View");
        controller = GameObject.Find("Controller");
        uiCanvas = GameObject.Find("Canvas");
        if (players == null)
        {
            players = new Player[2];
            InitPlayers();
            Debug.Log("Initializing players");
        }

        timer = Instantiate(Resources.Load<GameObject>("Prefabs/Timer"));
        timer.transform.SetParent(uiCanvas.transform, false);
    }
    
    //Create our player objects and their fields
    private void InitPlayers()
    {
        //GameObject[] playerGOs = new GameObject[players.Length];
        int playerRot = 180;
        for (int i = 0; i < players.Length; i++)
        {
            //Create Player
            GameObject playerGO;
            players[i] = new Player(); //Create Player Object
            playerGO = new GameObject("Player" + (i+1)); //Create GameObject in scene for the player's view
            playerGO.transform.parent = view.transform; //Set the parent for the player
            
            //Initialize Components
            GameObject newModel = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerModel"), model.transform);
            players[i].Model = newModel.GetComponent<PlayerModel>();
            players[i].View = playerGO.AddComponent<PlayerView>();
            players[i].Controller = controller.AddComponent<PlayerController>();
            
            //Init Components
            players[i].Controller.SetRewiredPlayer(i); //after creating controller, set the player profile
            players[i].Model.PlayerIndex = players[i].Controller.PlayerIndex = players[i].View.PlayerIndex = i;
            players[i].View.transform.position = StartingPositions[i];
            players[i].View.transform.rotation = Quaternion.Euler(0,playerRot,0);
            playerRot -= 180;
            players[i].View.PlayerModelState = players[i].Model; //give View a reference to the model so it can know state
            
            //Create UI
            healthBars[i] = Instantiate(Resources.Load<GameObject>("prefabs/p" + (i+1) + "Healthbar"));
            healthBars[i].transform.SetParent(uiCanvas.transform, false);
            healthBars[i].GetComponent<HealthBar>().PlayerIndex = i;
        }
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<HealthChanged>(OnHealthChanged);
        EventManager.Instance.RemoveHandler<ProcessInput>(OnInput);
    }
    
    #endregion
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown("r")){
            SceneManager.LoadScene("LevelName");
        }
    }

    #region Events

    private void OnHealthChanged(HealthChanged evt)
    {
        int index = evt.PlayerIndex;
        int newHealth = evt.NewHealth;
        HealthBar healthBar = healthBars[index].GetComponent<HealthBar>();
        healthBar.UpdateHealth(newHealth);
        if (newHealth <= 0)
            EndRound(index);
    }

    private void OnInput(ProcessInput evt)
    {
        switch (CurrentManagerState)
        {
                case ManagerState.RoundOver:
                    StartRound();
                    break;
        }
    }
    #endregion

    #region Round Management

    //End the current round
    private void EndRound(int losingPlayerIndex)
    {
        for (int i = 0; i < players.Length ; i++)
        {
            if (i != losingPlayerIndex)
            {
                Debug.Log("player " + (i + 1) + " wins");
                players[i].Model.WinRound();
            }
        }
        
        //stop the timer
    }

    //Start a new round
    private void StartRound()
    {
        players = null;
        InitPlayers();
        roundNum++;
        
        //reset the timer
    }
    
    #endregion
}

//Object that holds a player's model, view and controller for referencing as a group
public class Player
{
    #region Fields
    private PlayerModel model;
    private PlayerView view;
    private PlayerController controller;
    #endregion
    
    #region Constructors
    public Player(PlayerModel newModel, PlayerView newView, PlayerController newController)
    {
        model = newModel;
        view = newView;
        controller = newController;
    }

    public Player()
    {
        model = null;
        view = null;
        controller = null;
    }
    #endregion

    #region Properties
    public PlayerModel Model
    {
        get { return model; }
        set { model = value; }
    }

    public PlayerView View
    {
        get { return view; }
        set { view = value; }
    }

    public PlayerController Controller
    {
        get { return controller; }
        set { controller = value; }
    }
    #endregion
}

public enum ManagerState
{
    Start,
    Fighting,
    RoundOver,
    SetOver,
    Menu
}