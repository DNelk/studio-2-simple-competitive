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

    private void MoveCheck()
    {
       
    }
    #endregion
}
