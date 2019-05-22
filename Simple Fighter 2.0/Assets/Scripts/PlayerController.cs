using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using static Events;

//Receives inputs from the controller and passes them to the Model
public class PlayerController : MonoBehaviour
{
    
    #region Private Variables

    private Rewired.Player rewiredPlayer; //holds the player profile from Rewired
    #endregion
    
    public enum InputState
    {
        Walk,
        EndWalk,
        Strike,
        Grab,
        Block,
        EndBlock,
        Roll,
        GetUp,
        MoveRelease,
        Confirm,
        Quit,
        Empty
    }

    #region Public Variables
    public int PlayerIndex;
    #endregion

    
    // Update is called once per frame
    void Update()
    {
        MoveCheck();
        StrikeCheck();
        GrabCheck();
        BlockCheck();
        GetUpCheck();
        MoveReleaseCheck();
        ConfirmCheck();
        QuitCheck();
        PauseGame();
    }
    
    //Called from the GameManager to set which player profile to use
    public void SetRewiredPlayer(int playerNum)
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerNum);
    }
    
    #region Input Functions

    private void SendInput(InputState state, float value)
    {
        EventManager.Instance.Fire(new ProcessInput(state, value, PlayerIndex));
    }
    
    //Check for inputs during moveable states
    private void MoveCheck()
    {
        float axisRaw = rewiredPlayer.GetAxisRaw("MoveHorizontal");
        if (axisRaw != 0)
        {
            //switch to walking
            SendInput(InputState.Walk, axisRaw);
        }

        if (axisRaw == 0)
        {
            //stop walking and switch to idle
            SendInput(InputState.EndWalk, axisRaw);
        }
    }
    
    //Check for Strike
    private void StrikeCheck()
    {
        if (rewiredPlayer.GetButtonDown("Strike"))
        {
            //switch to Strike
            SendInput(InputState.Strike, 0f);
        }
    }
    
    //Check for Grab
    private void GrabCheck()
    {
        if (rewiredPlayer.GetButtonDown("Grab"))
        {
            //switch to Grab
            SendInput(InputState.Grab, 0f);
        }
    }
    
    //Check for Block
    private void BlockCheck()
    {
        if (rewiredPlayer.GetButtonDown("Block"))
        {
            //switch to Block
            SendInput(InputState.Block, 0f);
        }

        if (rewiredPlayer.GetButtonUp("Block"))
        {
            //stop blocking, switch to Idle
            SendInput(InputState.EndBlock, 0f);
        }
    }
    
    //Check for Roll
    private void GetUpCheck()
    {
        float axisRaw = rewiredPlayer.GetAxisRaw("MoveHorizontal");
        if (rewiredPlayer.GetAxisRaw("MoveHorizontal") != 0)
        {
            //switch to roll and go in the correct direction
            SendInput(InputState.Roll, axisRaw);
        }
        
        else if (rewiredPlayer.GetAxisRaw("MoveVertical") != 0)
        {
            //if up, Stand
            SendInput(InputState.GetUp, axisRaw);
        }
    }
    
    //Check to see if the player releases movement
    private void MoveReleaseCheck()
    {
        if (rewiredPlayer.GetAxisRaw("MoveHorizontal") == 0 && rewiredPlayer.GetAxisRaw("MoveVertical") == 0)
        {
            SendInput(InputState.MoveRelease, 0f);
        }
    }

    private void ConfirmCheck()
    {
        if (rewiredPlayer.GetButtonDown("Confirm"))
        {
            //switch to Grab
            SendInput(InputState.Confirm, 0f);
        }
    }

    private void QuitCheck()
    {
        if (rewiredPlayer.GetButtonDown("Quit"))
        {
            SendInput(InputState.Quit, 0f);
        }
    }

    private void PauseGame()
    {
        if (rewiredPlayer.GetButtonDown("Pause"))
        {
            if (GameManager.Instance.CurrentManagerState == ManagerState.Fighting)
                GameManager.Instance.pauseMenu(PlayerIndex);
        }
    }
    #endregion
    
    #region Public Functions

    #endregion
}
