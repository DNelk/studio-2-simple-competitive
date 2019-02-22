﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEditor.Experimental.UIElements;

public class PlayerController : MonoBehaviour
{
    #region Public Variables
    public PlayerModel Model; //Our Model
    public PlayerModel OpponentModel; //The model of the other player
    public PlayerView View; //Our View
    public Player Player; //Rewired Player reference
    #endregion
    
    #region Physics Vars
    public float DelayInSeconds = 0.25f;
    public float SpeedMultiplier = 10;
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance = 0;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance = 0;
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
                break;
            case PlayerState.Walking: //can perform any action while walking
                MoveCheck();
                StrikeCheck();
                GrabCheck();
                BlockCheck();
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
                break;
            case PlayerState.TechRollStartup: //Tech roll
            case PlayerState.TechRollActive:
            case PlayerState.TechRollRecovery:
                break;
            case PlayerState.GetupStartup: //Getting up
            case PlayerState.GetupActive:
            case PlayerState.GetupRecovery:
                break;
            case PlayerState.GetupRollStartup: //Getting up with a roll
            case PlayerState.GetupRollActive:
            case PlayerState.GetupRollRecovery:
                break;
            case PlayerState.Grounded: //On the ground
                WakeUpCheck();
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
        yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
        //ACTIVE
        Model.State = PlayerState.BlockActive; 
    }

    //Go into block recovery and back to idle
    //Currently the revovery frames don't work
    private IEnumerator PlayerAction_ReleaseBlock()
    {
        //RECOVERY
        Model.State = PlayerState.BlockRecovery;
        yield return StartCoroutine(WaitFor.Frames(12)); // wait for frames
            
        //FAF
        Model.State = PlayerState.Idle;
    }
    #endregion

    #region Striking

    private void StrikeCheck()
    {
        if (Player.GetButtonDown("Strike"))
        {
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
        SpawnHitBox(StrikeHitBoxDistance, StrikeHitBoxSize, "strike box ");
        yield return StartCoroutine(WaitFor.Frames(2)); // wait for frames
        //RECOVERY
        Model.State = PlayerState.StrikeRecovery;
        yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
        //FAF
        Model.State = PlayerState.Idle;
    }
    #endregion

    #region Grabbing

    private void GrabCheck()
    {
        if (Player.GetButtonDown("Grab"))
        {
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
        SpawnHitBox(GrabHitBoxDistance, GrabHitBoxSize, "grab box ");
        yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
        //RECOVERY
        Model.State = PlayerState.GrabRecovery;
        yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
        //FAF
        Model.State = PlayerState.Idle;
    }
    #endregion
    
    #region Hitting and Getting Hit
    //Spawn a hitbox when we get hit
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
        yield return StartCoroutine((WaitFor.Frames(2))); //Wait a sec for hit animation
        
        //ACTIVE - this is the window during which a player can input a tech.
        Model.State = PlayerState.DamageActive;
        yield return StartCoroutine(WaitFor.Frames(5));
        
        //Did we tech in this window? if not, let's process the rest of this
        if (Model.State == PlayerState.DamageActive)
        {

            //RECOVERY
            Model.State = PlayerState.DamageRecovery;
            yield return StartCoroutine(WaitFor.Frames(40)); // 40 is an arbitrary number for now

            //FAF
            Model.State = PlayerState.Grounded; // once it's implemented, the player should transition to the Grounded state.
        }

    }
    #endregion

    #region Tech Moves and Getting Up

    private void WakeUpCheck()
    {
        if (Model.State == PlayerState.DamageActive) //We're teching!
        {
            //If we move up
            if (Player.GetAxisRaw("Up") != 0)
                StartCoroutine(TechInPlace());
        
            //If we move rectilinearly
            if (Player.GetAxisRaw("Horizontal Movement") != 0)
                StartCoroutine(TechRoll());
        }
        else
        {
         
        }
    }

    //Tech Up from falling or get up from ground
    private IEnumerator TechInPlace()
    {
        //go into tech in place states
        Debug.Log("tech up");
        yield return StartCoroutine(WaitFor.Frames(1));
    }
    

    //Tech roll from falling
    private IEnumerator TechRoll()
    {
        //go into tech roll states
        Debug.Log("tech roll");
        yield return StartCoroutine(WaitFor.Frames(1));
    }
    #endregion
    
    #region Tools
    private void OnDrawGizmos()
    {
        //Draw strike hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(View.transform.position.x + StrikeHitBoxDistance, 0), StrikeHitBoxSize);
        
        //Draw Grab hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(View.transform.position.x + GrabHitBoxDistance, 0), GrabHitBoxSize);
    }
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
