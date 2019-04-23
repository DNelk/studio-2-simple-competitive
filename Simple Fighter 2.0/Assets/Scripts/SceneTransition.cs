using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEditor;
using UnityEngine.SceneManagement;

//When someone presses the spacebar or a select button, move from title scene to LevelName
public class SceneTransition : MonoBehaviour
{
    
    //Player controllers
    private Rewired.Player rewiredPlayer1;
    private Rewired.Player rewiredPlayer2;

    //State enum
    private enum ScreenState
    {
        Title,
        Cracked,
        Disclaimer,
        ModeSelect
    }
    private ScreenState _state;
    
    // Start is called before the first frame update
    void Start()
    {
        _state = ScreenState.ModeSelect;
        rewiredPlayer1 = ReInput.players.GetPlayer(0);
        rewiredPlayer2 = ReInput.players.GetPlayer(1);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case ScreenState.Title :
                Title();
                break;
            case ScreenState.Cracked :
                break;
            case ScreenState.Disclaimer :
                Disclaimer();
                break;
            case ScreenState.ModeSelect :
                ModeSelect();
                break;
        }
    }

    private void Title()
    {
        if (rewiredPlayer1.GetButtonDown("ConfirmP1") || rewiredPlayer2.GetButtonDown("ConfirmP2"))
        {
            Instantiate(Resources.Load("Prefabs/PalmmyEffect/TitleAnimation"), transform.parent);
            _state = ScreenState.Cracked;
        }
    }

    private void Disclaimer()
    {
        
    }

    private void ModeSelect()
    {
        if (rewiredPlayer1.GetButtonDown("Confirm") || rewiredPlayer2.GetButtonDown("Confirm"))
        {
            StartCoroutine(LoadYourAsyncScene());
        }
    }
    
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
}
