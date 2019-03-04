using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//usage: gets instantiated when the game starts up and persists between loads
//intent: manage games, matches, victory and loss, character select
public class GameManager : MonoBehaviour
{
    #region Public Variables
    
    public static GameManager instance = null;
    #endregion
    
    #region Private Variables

    private int player1Rounds;
    private int player2Rounds;
    private int player1Victories;
    private int player2Victories;
    
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        
        //cap the frame rate at 60fps
        Application.targetFrameRate = 60;
        
        Debug.Log("Game Manager instantiated!");
    }

    // Update is called once per frame
    void Update()
    {
		
    }
    
    //Match Start
    private void MatchStart()
    {
        //Instantiate characters
        
        //Hook those characters up to their respective Controllers, Models, and Views
        
        RoundStart();
    }
    
    //Match End
    private void MatchEnd(int victor)
    {
        //Declare victor from RoundEnd
        
        //That player's victories are incremented
        
        //Offer rematch, character select, and quit options
    }
    
    //Round Timer
    private void RoundTimer()
    {
        //Once the round starts, time the players
        
        //If the time ends, check player health and the healthier player wins
        
        //Otherwise it is a draw
    }
    
    //Round End
    public void RoundEnd(int victor)
    {
        //When the round ends, check for a victor (0 = P2, 1= p1) and increment their round counter\
        if (victor == 0)
        {
            Debug.Log("Player 2 wins!");
        }
        else if (victor == 1)
        {
            Debug.Log("Player 1 wins!");
        }
        
        //If one player has enough rounds to win the game, they win!
    }

    //Round Start
    private void RoundStart()
    {
        //Place Characters at starting positions
        
        //Countdown the round
        
        //Countdown ends, give players control
    }
}
