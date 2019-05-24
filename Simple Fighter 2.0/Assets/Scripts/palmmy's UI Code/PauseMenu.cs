using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public int playerIndex;
    private int menuOrder;
    // 1 = resume, 2 = control, 3 = restart match, 4 = quit game

    private PauseState currentPauseState;

    public Image playerNumber;
    public Sprite[] player;
    public Image playerSelection;
    public Sprite[] PlayerSelectionSprite;
    public RectTransform outline;

    public GameObject controlWindow;
    public Image controlWindow_Outline;

    public Animator anim;
    private string destinationScene = "Empty";

    private Rewired.Player rewiredPlayer;
    private bool DpadPressing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(playerIndex);

        currentPauseState = PauseState.selectionWindow;
        menuOrder = 1;

        if (playerIndex == 0)
        {
            playerNumber.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-180,259);
            playerNumber.sprite = player[0];
            playerSelection.sprite = PlayerSelectionSprite[0];
        }

        if (playerIndex == 1)
        {
            playerNumber.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(180,259);
            playerNumber.sprite = player[1];
            playerSelection.sprite = PlayerSelectionSprite[1];
        }
        
        //set animator
        anim = GetComponent<Animator>();
        
        //set controller
        rewiredPlayer = ReInput.players.GetPlayer(playerIndex);

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPauseState)
        {
            case PauseState.selectionWindow:
                confirmSelection();
                moveSelection();
                exitPauseScreen();
                break;
            case PauseState.control:
                ControlWindow();
                break;
        }
    }

    private void moveSelection()
    {
        //when move
        if (DpadPressing == false)
        {
            if (rewiredPlayer.GetAxisRaw("MoveVertical") >= 0.5f)
            {
                menuOrder--;
                DpadPressing = true;
            }
            if (rewiredPlayer.GetAxisRaw("MoveVertical") <= -0.5f)
            {
                menuOrder++;
                DpadPressing = true;
            }
        }
        else
        {
            if (rewiredPlayer.GetAxisRaw("MoveVertical") == 0)
                DpadPressing = false;
        }

        //when move pass limit
        if (menuOrder > 4)
            menuOrder = 1;
        if (menuOrder < 1)
            menuOrder = 4;
        
        //position of outline for selection
        if (menuOrder == 1)
            outline.anchoredPosition = 
                new Vector2(0,70);
        if (menuOrder == 2)
            outline.anchoredPosition = 
                new Vector2(0,-13);
        if (menuOrder == 3)
            outline.anchoredPosition = 
                new Vector2(0,-96);
        if (menuOrder == 4)
            outline.anchoredPosition = 
                new Vector2(0,-179);
    }
    
    private void confirmSelection()
    {
        if (rewiredPlayer.GetButtonDown("Confirm"))
        {
            if (menuOrder == 1)
            {
                GameManager.Instance.pauseMenu(playerIndex);
                Destroy(gameObject);
            }
            else if (menuOrder == 2)
            {
                controlWindow.SetActive(true);
                currentPauseState = PauseState.control;
                controlWindow_Outline.sprite = PlayerSelectionSprite[playerIndex];
            }
            else if (menuOrder == 3)
            {
                currentPauseState = PauseState.restartingMatch;
                destinationScene = "LevelName";
                anim.SetBool("Reset", true);
                //StartCoroutine(LoadYourAsyncScene());
            }
            else if (menuOrder == 4)
            {
                currentPauseState = PauseState.ReturningToMainMenu;
                destinationScene = "TitleScreen";
                anim.SetBool("Reset", true);
                //StartCoroutine(LoadYourAsyncScene());
            }
        }
        
    }

    private void exitPauseScreen()
    {
        if (rewiredPlayer.GetButtonDown("Pause"))
        {
            Debug.Log("unPause");
            GameManager.Instance.pauseMenu(playerIndex);
            Destroy(gameObject);
        }
    }

    private void ControlWindow()
    {
        if (rewiredPlayer.GetButtonDown("Confirm"))
        {
            controlWindow.SetActive(false);
            currentPauseState = PauseState.selectionWindow;
        }
    }
    
    public IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        GameManager.Instance.pauseMenu(playerIndex);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destinationScene);
        Debug.Log("Load");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    enum PauseState
    {
        selectionWindow,
        control,
        restartingMatch,
        ReturningToMainMenu
    }
  
}
