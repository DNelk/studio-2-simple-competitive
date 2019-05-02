using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
    private GameObject announcer;
    #endregion
    
    
    #region Round Information

    public int TotalRounds = 3;
    public int roundNum;
    private int setNum;
    private int[] playerWins;
    private int[] playerHealthCached;
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
        EventManager.Instance.AddHandler<HealthChanged>(OnHealthChanged);
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
        announcer = Instantiate(Resources.Load<GameObject>("Prefabs/winAnnouncer"), uiCanvas.transform);
        if (players == null)
        {
            players = new Player[2];
            playerWins = new int[2];
            playerHealthCached = new int[2]{6,6};
            InitPlayers();
            //Debug.Log("Initializing players");
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
            GameObject playerView, playerModel, playerController;
            players[i] = new Player(); //Create Player Object
        
            //Initialize Components
            playerModel = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerModel"), model.transform);
            players[i].Model = playerModel.GetComponent<PlayerModel>();
            
            playerView = new GameObject("Player" + (i+1)); //Create GameObject in scene for the player's view
            playerView.transform.parent = view.transform; //Set the parent for the player
            players[i].View = playerView.AddComponent<PlayerView>();
            
            playerController = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerController"), controller.transform);
            players[i].Controller = playerController.GetComponent<PlayerController>();
            
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
            for (int j = 0; j < playerWins[i]; j++)
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/Winpoint"), healthBars[i].GetComponent<HealthBar>().WinCounter[j].transform);
            }
            
            //Set up camera manager                 
            CameraManager.Instance.AddView(players[i].View.gameObject);
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
        //Test reset
        if (Input.GetKeyDown("r")){
            SceneManager.LoadScene("LevelName");
        }

        switch (CurrentManagerState)
        {
                case ManagerState.Fighting:
                    //Check if we timed out
                    if (timer.GetComponent<Timer>().TimerRaw <= 0)
                    {
                        if (playerHealthCached[0] > playerHealthCached[1])
                        {
                            GameObject TimeUp = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Time'sUp"), uiCanvas.transform) as GameObject;
                            TimeUp.GetComponent<EndRound>().losingPlayer = 1;
                            CurrentManagerState = ManagerState.End;
                        }
                        else if (playerHealthCached[1] > playerHealthCached[0])
                        {
                            GameObject TimeUp = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Time'sUp"), uiCanvas.transform) as GameObject;
                            TimeUp.GetComponent<EndRound>().losingPlayer = 0;
                            CurrentManagerState = ManagerState.End;
                        }
                        else
                        {
                            //Uh oh... time out with no winners.. need a solution for this
                        }
                    }
                    break;
        }
        
    }

    #region Events

    private void OnHealthChanged(HealthChanged evt)
    {
        int index = evt.PlayerIndex;
        int newHealth = evt.NewHealth;
        HealthBar healthBar = healthBars[index].GetComponent<HealthBar>();
        healthBar.UpdateHealth(newHealth);

        //get the other player index
        int opponentIndex = 0;
        for(int i = 0; i < playerHealthCached.Length; i++)
        {
            if(i != index){
                opponentIndex = i;
            }
        }
        
        //Cache health in case we time out
        playerHealthCached[index] = newHealth;
        
        //when a player lose
        if (newHealth <= 0)
        {
            GameObject result = null;
            if (playerHealthCached[opponentIndex] >= 6)
            {
                result = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Perfect"), uiCanvas.transform) as GameObject;
            }        
            else if (playerHealthCached[opponentIndex] < 6)
            {
                result = Instantiate(Resources.Load("Prefabs/PalmmyEffect/KO"), uiCanvas.transform) as GameObject;
            }
            result.GetComponent<EndRound>().losingPlayer = index;
            playerHealthCached[index] = 6;
            playerHealthCached[opponentIndex] = 6;
            CurrentManagerState = ManagerState.End;
        }
    }

    private void OnInput(ProcessInput evt)
    {
        switch (CurrentManagerState)
        {
                case ManagerState.RoundOver:
                    if(evt.NewInput == PlayerController.InputState.Confirm)
                        announcer.GetComponent<WinningAnnouncer>().ResetBool();
                    break;
        }
    }
    #endregion

    #region Round Management

    //End the current round
    public void EndRound(int losingPlayerIndex)
    {
        for (int i = 0; i < players.Length ; i++)
        {
            if (i != losingPlayerIndex)
            {
               // Debug.Log("player " + (i + 1) + " wins");
                players[i].Model.WinRound();
                announcer.GetComponent<WinningAnnouncer>().PlayerWin(i);
                playerWins[i]++;
                if (playerWins[i] >= TotalRounds)
                {
                    //stop the timer
                    CurrentManagerState = ManagerState.SetOver;
                    for (int j = 0; j < playerWins[i]; j++)
                    {
                        Instantiate(Resources.Load("Prefabs/PalmmyEffect/Winpoint"), healthBars[i].GetComponent<HealthBar>().WinCounter[j].transform);
                    }
                    //End the set somehow lol
                }
                else
                {
                    //stop the timer
                    CurrentManagerState = ManagerState.RoundOver;
                }
            }
        }
        
        CameraManager.Instance.ClearViews();
    }

    //Start a new round
    public void StartRound()
    {
        for(int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].Model.gameObject);
            Destroy(players[i].View.gameObject);
            Destroy(players[i].Controller.gameObject);
            Destroy(healthBars[i]);
        }
        players = new Player[2];
        InitPlayers();
        roundNum++;
        
        //reset the timer
        CurrentManagerState = ManagerState.Start;
        timer.GetComponent<Timer>().RoundUpdate();
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
    End,
    RoundOver,
    SetOver,
    Menu
}