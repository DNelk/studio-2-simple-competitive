using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    #region Public Variables
    
    public PlayerState State = PlayerState.Idle; //The current state of the player
    public int Health = 10; //How much health the player has
    public int Stamina = 10; //How much stamina the player has
    public int PlayerIndex;
    
    #endregion

    #region Updates
    
    void Update()
    {
        //Do specific things depending on our state
        switch (State)
        {
             default:
                 break;
        }
    }
    
    #endregion
}

//Used to determine what our player is currently doing so that actions and animations don't conflict
public enum PlayerState
{
    Walking,
    Jumping,
    StrikeStartup,
    Striking,
    StrikeCooldown,
    GrabStartup,
    Grabbing,
    GrabCooldown,
    BlockStartup,
    Blocking,
    BlockCooldown,
    Damage,
    Special,
    Crouching,
    Grounded,
    Idle
};

//boop

