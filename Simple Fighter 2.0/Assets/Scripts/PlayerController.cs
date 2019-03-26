using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

//Receives inputs from the controller and passes them to the Model
public class PlayerController : MonoBehaviour
{
    
    #region Private Variables

    private Rewired.Player rewiredPlayer; //holds the player profile from Rewired
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Called from the GameManager to set which player profile to use
    public void SetRewiredPlayer(int playerNum)
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerNum);
    }
    
    #region Input Functions
    
    //Check for inputs during moveable states
    private void MoveCheck()
    {
        if (rewiredPlayer.GetAxisRaw("HorizontalMovement") != 0)
        {
            //switch to walking
        }

        if (rewiredPlayer.GetAxisRaw("HorizontalMovement") == 0)
        {
            //stop walking and switch to idle
        }
    }
    
    //Check for Strike
    private void StrikeCheck()
    {
        if (rewiredPlayer.GetButtonDown("Strike"))
        {
            //switch to Strike
        }
    }
    
    //Check for Grab
    private void GrabCheck()
    {
        if (rewiredPlayer.GetButtonDown("Grab"))
        {
            //switch to Grab
        }
    }
    
    //Check for Block
    private void BlockCheck()
    {
        if (rewiredPlayer.GetButtonDown("Block"))
        {
            //switch to Block
        }

        if (rewiredPlayer.GetButtonUp("Block"))
        {
            //stop blocking, switch to Idle
        }
    }
    
    //Check for Tech
    private void TechCheck()
    {
        if (rewiredPlayer.GetAxisRaw("HorizontalMovement") != 0)
        {
            //switch to tech roll and go in the correct direction
        }
        
        else if (rewiredPlayer.GetAxisRaw("VerticalMovement") != 0)
        {
            //if up, Tech Stand
        }
    }
    #endregion
}
