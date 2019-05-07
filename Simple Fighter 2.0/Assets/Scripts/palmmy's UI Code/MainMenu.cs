using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    //set states
    public MenuState currentMenuState;
    public NoticeState currentNoticeState;

    //set noticeScreen
    public GameObject noticeScreen;
    public bool generalNotice = true;
    
    //setComponent
    private ModeSelector_CenterRotation modeSelecter_Circle;
    private MenuWindow menuWindow;
    private ModeSelector_ScreenChange modeSelector_screen;
    public bool bubbleChange;
    
    //set controller
    //Player controllers
    private Rewired.Player rewiredPlayer1;
    private Rewired.Player rewiredPlayer2;
    private bool DpadPressing;

    //do this before anything else
    void Awake()
    {
        //Set up the singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {    
        //set controller
        rewiredPlayer1 = ReInput.players.GetPlayer(0);
        rewiredPlayer2 = ReInput.players.GetPlayer(1);

        //check if there is any ganeral notice for the game at the moment
        if (generalNotice)
            currentNoticeState = NoticeState.OpeningNotice;
        else
        {
            currentNoticeState = NoticeState.OffNotice;
        }
        
        //set component
        modeSelecter_Circle = GameObject.Find("ModeSelector").GetComponent<ModeSelector_CenterRotation>();
        menuWindow = GameObject.Find("MenuWindow").GetComponent<MenuWindow>();
        modeSelector_screen = GameObject.Find("Screens").GetComponent<ModeSelector_ScreenChange>();
        bubbleChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentNoticeState)
        {
            case NoticeState.OpeningNotice:
                callNotice();              
                break;
            case NoticeState.OnNotice:
                onNotice();
                break;
            case NoticeState.OffNotice:
                offNotice();
                break;
        }
    }

    #region noticeStateFunction
    
    //this happen when notice screen is called
    void callNotice()
    {
            noticeScreen = Instantiate(Resources.Load("Prefabs/PalmmyEffect/NoticeScreen"), transform) as GameObject;
            currentNoticeState = NoticeState.OnNotice;
    }

    //this happen when notice screen is showing, allowing player to close it
    void onNotice()
    {
        if (rewiredPlayer1.GetButtonDown("Confirm") || rewiredPlayer2.GetButtonDown("Confirm"))
        {
            noticeScreen.GetComponent<Animator>().SetBool("isExit",true);
            currentNoticeState = NoticeState.OffNotice;
        }  
    }
    
    //this happen when notice screen is not presence, allowing player to choose mode and move on
    void offNotice()
    {
        switch (currentMenuState)
        {
            case MenuState.VersusSelected:
                VerticalInput(MenuState.QuitSelected, MenuState.ArcadeSelected);
                break;
            case MenuState.ArcadeSelected:
                VerticalInput(MenuState.VersusSelected,MenuState.QuitSelected);
                break;
            case MenuState.QuitSelected:
                VerticalInput(MenuState.ArcadeSelected,MenuState.VersusSelected);
                break;
        }
        
        if (rewiredPlayer1.GetButtonDown("Confirm") || rewiredPlayer2.GetButtonDown("Confirm"))
            ConfirmInput();
    }
    
    #endregion

    #region inputCheck
    
    public void VerticalInput(MenuState upperMenu, MenuState lowerMenu)
    {
        if (DpadPressing == false)
        {
            if (rewiredPlayer1.GetAxisRaw("MoveVertical") >= 0.5f || rewiredPlayer2.GetAxisRaw("MoveVertical") >= 0.5f)
            {
                currentMenuState = upperMenu;
                DpadPressing = true;
                modeSelecter_Circle.circlingUp();
                menuWindow.changeMenu();
                modeSelector_screen.changeModeScreen();
                bubbleChange = true;

            }

            if (rewiredPlayer1.GetAxisRaw("MoveVertical") <= -0.5f || rewiredPlayer2.GetAxisRaw("MoveVertical") <= -0.5f)
            {
                currentMenuState = lowerMenu;
                DpadPressing = true;
                modeSelecter_Circle.circlingDown();
                menuWindow.changeMenu();
                modeSelector_screen.changeModeScreen();
                bubbleChange = true;
            }
        }
        else
        {
            if (rewiredPlayer1.GetAxisRaw("MoveVertical") == 0 && rewiredPlayer2.GetAxisRaw("MoveVertical") == 0)
                DpadPressing = false;

            bubbleChange = false;
        }
    }

    void ConfirmInput()
    {
        switch (currentMenuState)
        {
            case MenuState.VersusSelected:
                StartCoroutine(LoadYourAsyncScene());
                break;
            case MenuState.ArcadeSelected:
                callNotice();
                break;
            case MenuState.QuitSelected:
                Application.Quit();
                break;
        }
    }
    
    #endregion
    
    #region MenuState

    public enum MenuState
    {
        VersusSelected,
        ArcadeSelected,
        QuitSelected
    }

    #endregion

    #region NoticeState

    public enum NoticeState
    {
        OpeningNotice,
        OnNotice,
        OffNotice,
    }

    #endregion

    #region goToVersusScene

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelName");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    #endregion
}
