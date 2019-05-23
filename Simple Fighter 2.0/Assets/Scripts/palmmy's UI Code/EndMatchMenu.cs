using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMatchMenu : MonoBehaviour
{
    private int menuOrder;
    // 1 = restart match, 2 = quit game

    private EndMatchState currentState;

    public int playerwin;
    public Image playerSelection;
    public Sprite[] PlayerSelectionSprite;
    public RectTransform outline;
    
    public Animator anim;
    private string destinationScene = "Empty";
    
    private Rewired.Player rewiredPlayer1;
    private Rewired.Player rewiredPlayer2;
    private bool DpadPressing = false;

    
    // Start is called before the first frame update
    void Start()
    {
        currentState = EndMatchState.SelectionWindow;
        menuOrder = 1;
        
        //set animator
        anim = GetComponent<Animator>();
        
        //set selectioncolor
        if (playerwin == 1)
            playerSelection.sprite = PlayerSelectionSprite[0];
        if (playerwin == 2)
            playerSelection.sprite = PlayerSelectionSprite[1];
            
        rewiredPlayer1 = ReInput.players.GetPlayer(0);
        rewiredPlayer2 = ReInput.players.GetPlayer(1);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EndMatchState.SelectionWindow:
                confirmSelection() ;
                moveSelection();
                break;
        }
    }
    
    private void moveSelection()
    {
        //when move
        if (DpadPressing == false)
        {
            if (rewiredPlayer1.GetAxisRaw("MoveVertical") >= 0.5f || rewiredPlayer2.GetAxisRaw("MoveVertical") >= 0.5f)
            {
                menuOrder--;
                DpadPressing = true;
            }
            if (rewiredPlayer1.GetAxisRaw("MoveVertical") <= -0.5f || rewiredPlayer2.GetAxisRaw("MoveVertical") <= -0.5f)
            {
                menuOrder++;
                DpadPressing = true;
            }
        }
        else
        {
            if (rewiredPlayer1.GetAxisRaw("MoveVertical") == 0 && rewiredPlayer2.GetAxisRaw("MoveVertical") == 0)
                DpadPressing = false;
        }

        //when move pass limit
        if (menuOrder > 2)
            menuOrder = 1;
        if (menuOrder < 1)
            menuOrder = 2;
        
        //position of outline for selection
        if (menuOrder == 1)
            outline.anchoredPosition = 
                new Vector2(0,46);
        if (menuOrder == 2)
            outline.anchoredPosition = 
                new Vector2(0,-37);
    }
    
    private void confirmSelection()
    {
        if (rewiredPlayer1.GetButtonDown("Confirm") || rewiredPlayer2.GetButtonDown("Confirm"))
        {
            if (menuOrder == 1)
            {
                currentState = EndMatchState.RestartMatch;
                destinationScene = "LevelName";
                anim.SetBool("Reset", true);
                //StartCoroutine(LoadYourAsyncScene());            
            }
            else if (menuOrder == 2)
            {
                currentState = EndMatchState.ReturningToMainMenu;
                destinationScene = "TitleScreen";
                anim.SetBool("Reset", true);
                //StartCoroutine(LoadYourAsyncScene());
            }
        }   
    }

    public void processMatchEnding()
    {
        if (destinationScene == "LevelName")
        {
            GameManager.Instance.rematch();
        }

        if (destinationScene == "TitleScreen")
        {
            StartCoroutine(LoadYourAsyncScene());
        }    
    }
    
    public IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destinationScene);
        Debug.Log("Load");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    enum EndMatchState
    {
        SelectionWindow,
        RestartMatch,
        ReturningToMainMenu
    }
}
