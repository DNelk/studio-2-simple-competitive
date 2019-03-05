using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    #region Public Variables
    public PlayerModel Model; //Our Model
    public PlayerModel OpponentModel; //The model of the other player
    public PlayerView View; //Our View
    public Player Player; //Rewired Player reference
    public PlayerStatus status; // HP and Stamina manager
    #endregion
    
    #region Physics Vars
    public float SpeedMultiplier = 10;
    public float RollMultiplier = 10;
    public float TechRollMultiplier = 15;
    private float rollDirection;
    private string hitBoxLayer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Player = ReInput.players.GetPlayer(Model.PlayerIndex);
        
        if (Model.PlayerIndex == 0) //reference Controller 1
        {
            hitBoxLayer = "HurtBoxP2";
        }
        else //reference Controller 2
        {
            hitBoxLayer = "HurtBoxP1";
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (Model.State)
        {
            case PlayerState.Idle: //can perform any action from idle
                MoveCheck();
                StrikeCheck();
                GrabCheck();
                BlockCheck();
                status.HPRecovery();
                status.StaminaRecovery();
                break;
            case PlayerState.Walking: //can perform any action while walking
                MoveCheck();
                StrikeCheck();
                GrabCheck();
                BlockCheck();
                status.HPRecovery();
                status.StaminaRecovery();
                break;
            case PlayerState.StrikeStartup: //can't do anything while punching
            case PlayerState.StrikeActive:
            case PlayerState.StrikeRecovery:
                break;
            case PlayerState.BlockStartup: //can release blocking while blocking
            case PlayerState.BlockActive:
            case PlayerState.BlockRecovery:
                BlockCheck();
                break;
            case PlayerState.GrabStartup: //can't do anything while grabbing
            case PlayerState.GrabActive:
            case PlayerState.GrabRecovery:
                break;
            case PlayerState.DamageStartup: //we got hit, let's resolve this
                StartCoroutine(PlayerAction_GetHit());
                break;
            case PlayerState.DamageActive: //Check for teching!
                WakeUpCheck();
                break;
            case PlayerState.TechInPlaceStartup: //Teching up
            case PlayerState.TechInPlaceActive:
            case PlayerState.TechInPlaceRecovery:
            case PlayerState.GetupStartup: //Getting up
            case PlayerState.GetupActive:
            case PlayerState.GetupRecovery:
                break;
            case PlayerState.TechRollStartup: //Tech roll
            case PlayerState.TechRollActive:
                View.Translate(rollDirection, TechRollMultiplier); //move player view during active frames
                break;
            case PlayerState.TechRollRecovery:
            case PlayerState.GetupRollStartup: //Getting up with a roll
            case PlayerState.GetupRollActive:
                View.Translate(rollDirection, RollMultiplier); //move player view during active frames
                break;
            case PlayerState.GetupRollRecovery:
                break;
            case PlayerState.Grounded: //On the ground
                WakeUpCheck();
                break;
            case PlayerState.Ko: //KO
            case PlayerState.Win: //Win
                break;
        }
    }

    #region Horizontal Movement

    //Check for inputs during states where the player can move
    private void MoveCheck()
    {
        if (Player.GetAxisRaw("Horizontal Movement") != 0)
            MoveHorizontal();

        if (Player.GetAxisRaw("Horizontal Movement") == 0)
            StopMoving();
    }

    //Move the player
    private void MoveHorizontal()
    {
        View.Translate(Player.GetAxis("Horizontal Movement"), SpeedMultiplier);
        Model.State = PlayerState.Walking;
    }

    //Stop moving the player and reset to Idle state
    private void StopMoving()
    {
        Model.State = PlayerState.Idle;
    }
    #endregion
    
    #region Blocking

    //Check for block input during viable states
    private void BlockCheck()
    {
        if (Player.GetButtonDown("Block"))
        {
            StartCoroutine(PlayerAction_Block());
        }

        if (Player.GetButtonUp("Block"))
        {
            StartCoroutine(PlayerAction_ReleaseBlock());
        }
    }
    
    //Startup block animation
    //Currently the startup frames don't work
    private IEnumerator PlayerAction_Block()
    {
        //STARTUP
        Model.State = PlayerState.BlockStartup;
        yield return StartCoroutine(WaitFor.Frames(3)); // wait for frames
        
        //ACTIVE
        Model.State = PlayerState.BlockActive; 
    }

    //Go into block recovery and back to idle
    //Currently the revovery frames don't work
    private IEnumerator PlayerAction_ReleaseBlock()
    {
        //RECOVERY
        Model.State = PlayerState.BlockRecovery;
        yield return StartCoroutine(WaitFor.Frames(22)); // wait for frames
            
        //FAF
        Model.State = PlayerState.Idle;
    }
    #endregion

    #region Striking

    private void StrikeCheck()
    {
        if (Player.GetButtonDown("Strike") && Model.Stamina > 0) //also check there is stamina left
        {
            status.performAction(1); //change number for different amount of Stamina uses
            //STARTUP STRIKE
            Model.State = PlayerState.StrikeStartup;
            StartCoroutine(PlayerAction_Strike());
        }
    }
    
    private IEnumerator PlayerAction_Strike()
    {
        yield return StartCoroutine(WaitFor.Frames(5)); // wait
        //ACTIVE
        Model.State = PlayerState.StrikeActive;
        SpawnHitBox(View.StrikeHitBoxDistance, View.StrikeHitBoxSize, "strike box ");
        yield return StartCoroutine(WaitFor.Frames(2)); // wait for frames
        //RECOVERY
        Model.State = PlayerState.StrikeRecovery;
        yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
        //FAF
        if (OpponentModel.State != PlayerState.Ko)
            Model.State = PlayerState.Idle;
        else
            Model.State = PlayerState.Win;
    }
    #endregion

    #region Grabbing

    private void GrabCheck()
    {
        if (Player.GetButtonDown("Grab") && Model.Stamina > 0) //also check there is stamina left
        {
            status.performAction(1); //change number for different amount of Stamina uses
            StartCoroutine(PlayerAction_Grab());
        }
    }
    
    private IEnumerator PlayerAction_Grab()
    {
        //STARTUP
        Model.State = PlayerState.GrabStartup;
        yield return StartCoroutine(WaitFor.Frames(8)); // wait for frames
        
        //ACTIVE
        Model.State = PlayerState.GrabActive;
        SpawnHitBox(View.GrabHitBoxDistance, View.GrabHitBoxSize, "grab box ");
        yield return StartCoroutine(WaitFor.Frames(5)); // wait for frames
        
        //RECOVERY
        Model.State = PlayerState.GrabRecovery;
        yield return StartCoroutine(WaitFor.Frames(12)); // wait for frames
        
        //FAF
        if (OpponentModel.State != PlayerState.Ko)
            Model.State = PlayerState.Idle;
        else
            Model.State = PlayerState.Win;
    }
    #endregion
    
    #region Hitting and Getting Hit
    
    //Spawn a hitbox when we launch a hit
    private void SpawnHitBox(float distance, Vector2 size, string boxName)
    {
        Vector2 hitBoxCenter = new Vector2(View.transform.position.x + distance, 0);
        Collider2D hitCol = Physics2D.OverlapBox(hitBoxCenter,
            size, 0, LayerMask.GetMask(hitBoxLayer));

        if (hitCol)
        {
            if (Model.State == PlayerState.StrikeActive && OpponentModel.State != PlayerState.BlockActive)
            {
               // StartCoroutine(OpponentModel.PlayerAction_GetHit());
                OpponentModel.State = PlayerState.DamageStartup;
            }
            else if (Model.State == PlayerState.GrabActive && OpponentModel.State != PlayerState.StrikeActive)
            {
                //StartCoroutine(OpponentModel.PlayerAction_GetHit());
                OpponentModel.State = PlayerState.DamageStartup;
            }
        }
    }
    
    
    
    //What happens when we get hit
    private IEnumerator PlayerAction_GetHit()
    {
        status.getHit(1);
        //yield return StartCoroutine((WaitFor.Frames(5))); //Wait a sec for hit animation

        if (Model.State != PlayerState.Ko)
        {
            //ACTIVE - this is the window during which a player can input a tech.
            Model.State = PlayerState.DamageActive;
            View.hitBox.enabled = false; //turn off the hitBox for the duration of knockdown
            Debug.Log("Player " + Model.PlayerIndex + "hitbox " + View.hitBox.isActiveAndEnabled);
            yield return StartCoroutine(WaitFor.Frames(5));

            //Did we tech in this window? if not, let's process the rest of this
            if (Model.State == PlayerState.DamageActive)
            {
                //RECOVERY
                Model.State = PlayerState.DamageRecovery;
                yield return StartCoroutine(WaitFor.Frames(20)); // arbitrary number for now

                //FAF
                Model.State = PlayerState.Grounded; //transition to the Grounded state.
            }
        }
    }
    #endregion

    #region Tech Moves and Getting Up

    private void WakeUpCheck()
    {
        bool tech = false; //Are we teching?
        if (Model.State == PlayerState.DamageActive)
        {
            tech = true;
            StopCoroutine(PlayerAction_GetHit());
        }
        //If we move up
        if (Player.GetAxisRaw("Up") != 0)
        {
            StartCoroutine(PlayerAction_WakeUpInPlace(tech));
        }

        //If we move rectilinearly
        if (Player.GetAxisRaw("Horizontal Movement") != 0)
        {
            rollDirection = Player.GetAxisRaw("Horizontal Movement");
            StartCoroutine(PlayerAction_WakeUpRoll(tech));
        }
    }

    //Tech Up from falling or get up from ground
    private IEnumerator PlayerAction_WakeUpInPlace(bool tech)
    {
        if (tech)
        {            //go into tech in place states
            //STARTUP
            Model.State = PlayerState.TechInPlaceStartup;
            yield return StartCoroutine(WaitFor.Frames(10)); // wait for frames
        
            //ACTIVE
            Model.State = PlayerState.TechInPlaceActive;
            yield return StartCoroutine(WaitFor.Frames(10)); // wait for frames
        
            //RECOVERY
            Model.State = PlayerState.TechInPlaceRecovery;
            View.hitBox.enabled = true; //reactivate hitbox as player stands
            Debug.Log("Player " + Model.PlayerIndex + "hitbox " + View.hitBox.isActiveAndEnabled);
            yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
            //FAF
            Model.State = PlayerState.Idle;
            Debug.Log("tech up");
        }
        else //Go into normal wakeup states
        {
            Model.State = PlayerState.GetupStartup;
            yield return StartCoroutine(WaitFor.Frames(24)); // wait for frames
        
            //ACTIVE
            Model.State = PlayerState.GetupActive;
            yield return StartCoroutine(WaitFor.Frames(24)); // wait for frames
        
            //RECOVERY
            Model.State = PlayerState.GetupRecovery;
            View.hitBox.enabled = true;
            Debug.Log("Player " + Model.PlayerIndex + "hitbox " + View.hitBox.isActiveAndEnabled);
            yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
            //FAF
            Model.State = PlayerState.Idle;
            Debug.Log("grounded wakeup");
        }

        yield return StartCoroutine(WaitFor.Frames(1));
    }
    

    //Tech roll from falling
    private IEnumerator PlayerAction_WakeUpRoll(bool tech)
    {
        if (tech) //go into tech roll states
        {
            //STARTUP
            Model.State = PlayerState.TechRollStartup;
            yield return StartCoroutine(WaitFor.Frames(10)); // wait for frames
        
            //ACTIVE
            Model.State = PlayerState.TechRollActive;
            yield return StartCoroutine(WaitFor.Frames(10)); // wait for frames
        
            //RECOVERY
            Model.State = PlayerState.TechRollRecovery;
            View.hitBox.enabled = true;
            Debug.Log("Player " + Model.PlayerIndex + "hitbox " + View.hitBox.isActiveAndEnabled);
            yield return StartCoroutine(WaitFor.Frames(20)); // wait for frames
        
            //FAF
            Model.State = PlayerState.Idle;
            Debug.Log("tech roll");
            
        }
        else //Go into normal roll states
        {
            //STARTUP
            Model.State = PlayerState.GetupRollStartup;
            yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
        
            //ACTIVE
            Model.State = PlayerState.GetupRollActive;
            yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
        
            //RECOVERY
            Model.State = PlayerState.GetupRollRecovery;
            View.hitBox.enabled = true;
            Debug.Log("Player " + Model.PlayerIndex + "hitbox " + View.hitBox.isActiveAndEnabled);
            yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
        
            //FAF
            Model.State = PlayerState.Idle;
            Debug.Log("grounded roll");
        }
        yield return StartCoroutine(WaitFor.Frames(1));
    }
    #endregion
    
    #region Tools
    #endregion
}

#region Helper Classes

//Gets Time in frames
public static class WaitFor
{
    public static IEnumerator Frames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }
}

#endregion
