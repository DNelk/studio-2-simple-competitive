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
    
    #region Player Actions
    //The only player action outside of model, since it must be accessed on a model-by-model basis
    public IEnumerator PlayerAction_GetHit()
    {
        
        //STARTUP
        State = PlayerState.DamageStartup;
        yield return StartCoroutine(WaitFor.Frames(2));
        
        //ACTIVE - this is the window during which a player can input a tech.
        State = PlayerState.DamageActive;
        yield return StartCoroutine(WaitFor.Frames(5));
        
        //RECOVERY
        State = PlayerState.DamageRecovery;
        yield return StartCoroutine(WaitFor.Frames(40)); // 40 is an arbitrary number for now
        
        //FAF
        State = PlayerState.Idle; // for the current prototype, the player returns to Idle here.
        //State = PlayerState.Grounded; // once it's implemented, the player should transition to the Grounded state.
        
    }
    #endregion
}

//Used to determine what our player is currently doing so that actions and animations don't conflict
public enum PlayerState
{
    Idle,
    Walking,
    StrikeStartup,
    StrikeActive,
    StrikeRecovery,
    GrabStartup,
    GrabActive,
    GrabRecovery,
    BlockStartup,
    BlockActive,
    BlockRecovery,
    
    DamageStartup,
    DamageActive,
    DamageRecovery,
	
    Grounded,
	
    GetupStartup,
    GetupActive,
    GetupRecovery,
	
    GetupRollStartup,
    GetupRollActive,
    GetupRollRecovery,
	
    TechInPlaceStartup,
    TechInPlaceActive,
    TechInPlaceRecovery,
	
    TechRollStartup,
    TechRollActive,
    TechRollRecovery,
	
    /* Not being used currently
    Jumping,
    Special,
    Crouching,
    */
};