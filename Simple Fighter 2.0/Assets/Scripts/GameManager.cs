using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Events;

public class GameManager : MonoBehaviour
{
    #region Public Variables
    
    public static GameManager Instance = null; //Our Singleton
    public Vector3[] StartingPositions = {new Vector3(-3.5f, 0f, 0f), new Vector3(3.5f, 0f, 0f)};
    
    #endregion
    
    #region Gameplay Variables
   
    private Player[] players;

    private GameObject model;
    private GameObject controller;
    private GameObject view;
    #endregion

    #region UI Variables

    private Transform uiCanvas;
    private GameObject[] healthBars;
    
    #endregion
    
    
    #region Round Information

    public int TotalRounds = 3;
    private int roundNum;
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
        Init();
    }

    //Set up variables
    private void Init()
    {
        EventManager.Instance.AddHandler<HealthChanged>(OnHealthChanged);
        model = GameObject.Find("Model");
        view = GameObject.Find("View");
        controller = GameObject.Find("Controller");
        
        if (players == null)
        {
            players = new Player[2];
            InitPlayers();
            Debug.Log("Initializing players");
        }
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
            players[i].Model = model.AddComponent<PlayerModel>();
            players[i].View = playerGO.AddComponent<PlayerView>();
            players[i].Controller = controller.AddComponent<PlayerController>();
            
            //Init Components
            players[i].Controller.SetRewiredPlayer(i); //after creating controller, set the player profile
            players[i].Model.PlayerIndex = players[i].Controller.PlayerIndex = players[i].View.PlayerIndex = i;
            players[i].View.transform.position = StartingPositions[i];
            players[i].View.transform.rotation = Quaternion.Euler(0,playerRot,0);
            playerRot -= 180;
        }
        
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<HealthChanged>(OnHealthChanged);
    }
    
    #endregion
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown("r"))
            SceneManager.LoadScene("LevelName");
    }

    #region Events

    private void OnHealthChanged(HealthChanged evt)
    {
        
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