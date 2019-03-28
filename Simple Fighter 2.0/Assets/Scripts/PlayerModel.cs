using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{

    #region Private Variables

    //Current hitpoints
    private int currentHitPoints;

    private StateMachine<PlayerModel> stateMachine;

    

    #endregion

    #region Public Variables

    //Character Statistics
    [Range(1, 5)] public int MaxHitPoints;
    [Range(1, 50)] public float MoveSpeed;
    [Range(1, 10)] public int StrikeStartupFrames;
    [Range(1, 10)] public int StrikeActiveFrames;
    [Range(1, 10)] public int StrikeRecoveryFrames;
    [Range(1, 10)] public int GrabStartupFrames;
    [Range(1, 10)] public int GrabActiveFrames;
    [Range(1, 10)] public int GrabRecoveryFrames;
    [Range(1, 10)] public int BlockStartupFrames;
    [Range(1, 10)] public int BlockRecoveryFrames;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Initialize state machine
        stateMachine = new StateMachine<PlayerModel>(this);
        stateMachine.TransitionTo<Idle>();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public StateMachine<PlayerModel>.State GetState()
    {
        return stateMachine.CurrentState;
    }

    //Called by Controller when an input is received
    public void ProcessInput(PlayerController.inputState input)
    {
        ((PlayerState)stateMachine.CurrentState).ProcessInput(input);
    }
    
    #region States

    //Base Player State
    private class PlayerState : StateMachine<PlayerModel>.State
    {
        private int timer;

        public virtual void CooldownAnimationEnded(){}

        public virtual void ProcessInput(PlayerController.inputState input)
        {
            //This function will be overridden by each state to only include the relevant inputs
        }
    }

    private class Walking : PlayerState
    {
        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.Walk:
                    break;
                case PlayerController.inputState.EndWalk:
                    break;
                case PlayerController.inputState.Strike:
                    Debug.Log("Strike!");
                    break;
                case PlayerController.inputState.Grab:
                    break;
                case PlayerController.inputState.Block:
                    break;
            }
        }
    }
    private class Striking : PlayerState{}

    private class Blocking : PlayerState
    {
        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.EndBlock:
                    break;
            }
        }
    }
    private class Grabbing : PlayerState{}

    private class Falling : PlayerState
    {
        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.TechRoll:
                    break;
                case PlayerController.inputState.TechUp:
                    break;
            }
        }
    }

    private class Grounded : PlayerState
    {
        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.Roll:
                    break;
                case PlayerController.inputState.GetUp:
                    break;
            }
        }
    }
    private class Rolling : PlayerState{}

    private class Idle : PlayerState
    {
        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.Walk:
                    Debug.Log("Walking!");
                    break;
                case PlayerController.inputState.Strike:
                    Debug.Log("Strike!");
                    break;
                case PlayerController.inputState.Grab:
                    Debug.Log("Grab!");
                    break;
                case PlayerController.inputState.Block:
                    break;
            }
        }
    }
    private class Victory : PlayerState{}
    #endregion
}