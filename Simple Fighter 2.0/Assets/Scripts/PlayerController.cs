using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

//Receives inputs from the controller and passes them to the Model
public class PlayerController : MonoBehaviour
{
    
    #region Private Variables

    private Rewired.Player rewiredPlayer; //holds the player profile from Rewired
    private PlayerModel playerModel; //holds this character's model
    #endregion
    
    public enum inputState
    {
        Walk,
        EndWalk,
        Strike,
        Grab,
        Block,
        EndBlock,
        TechRoll,
        TechUp,
        Roll,
        GetUp
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCheck();
        StrikeCheck();
        GrabCheck();
        BlockCheck();
        TechCheck();
        GetUpCheck();
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
        if (rewiredPlayer.GetAxisRaw("MoveHorizontal") != 0)
        {
            //switch to walking
            playerModel.ProcessInput(inputState.Walk);
        }

        if (rewiredPlayer.GetAxisRaw("MoveHorizontal") == 0)
        {
            //stop walking and switch to idle
            playerModel.ProcessInput(inputState.EndWalk);
        }
    }
    
    //Check for Strike
    private void StrikeCheck()
    {
        if (rewiredPlayer.GetButtonDown("Strike"))
        {
            //switch to Strike
            playerModel.ProcessInput(inputState.Strike);
        }
    }
    
    //Check for Grab
    private void GrabCheck()
    {
        if (rewiredPlayer.GetButtonDown("Grab"))
        {
            //switch to Grab
            playerModel.ProcessInput(inputState.Grab);
        }
    }
    
    //Check for Block
    private void BlockCheck()
    {
        if (rewiredPlayer.GetButtonDown("Block"))
        {
            //switch to Block
            playerModel.ProcessInput(inputState.Block);
        }

        if (rewiredPlayer.GetButtonUp("Block"))
        {
            //stop blocking, switch to Idle
            playerModel.ProcessInput(inputState.EndBlock);
        }
    }
    
    //Check for Tech
    private void TechCheck()
    {
        if (rewiredPlayer.GetAxisRaw("MoveHorizontal") != 0)
        {
            //switch to tech roll and go in the correct direction
            playerModel.ProcessInput(inputState.TechRoll);
        }
        
        else if (rewiredPlayer.GetAxisRaw("MoveVertical") != 0)
        {
            //if up, Tech Stand
            playerModel.ProcessInput(inputState.TechUp);
        }
    }
    
    //Check for Roll
    private void GetUpCheck()
    {
        if (rewiredPlayer.GetAxisRaw("MoveHorizontal") != 0)
        {
            //switch to roll and go in the correct direction
            playerModel.ProcessInput(inputState.Roll);
        }
        
        else if (rewiredPlayer.GetAxisRaw("MoveVertical") != 0)
        {
            //if up, Stand
            playerModel.ProcessInput(inputState.GetUp);
        }
    }
    #endregion
    
    #region Public Functions

    //Assign the character's model to playerModel from the GameManager
    public void AssignModel(PlayerModel model)
    {
        playerModel = model;
    }
    #endregion
}
