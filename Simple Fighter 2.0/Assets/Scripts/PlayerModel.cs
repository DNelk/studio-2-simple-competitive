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

    private PlayerView playerView;
    #endregion

    #region Public Variables

    //Character Statistics
    [Range(1, 5)] public int MaxHitPoints;
    [Range(1, 50)] public float MoveSpeed = 10;
    [Range(1, 10)] public int StrikeStartupFrames;
    [Range(1, 10)] public int StrikeActiveFrames;
    [Range(1, 10)] public int StrikeRecoveryFrames;
    [Range(1, 10)] public int GrabStartupFrames;
    [Range(1, 10)] public int GrabActiveFrames;
    [Range(1, 10)] public int GrabRecoveryFrames;
    [Range(1, 10)] public int BlockStartupFrames;
    [Range(1, 10)] public int BlockRecoveryFrames;

    public int PlayerIndex;
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

    #region Getters/Setters
    
    public StateMachine<PlayerModel>.State GetState()
    {
        return stateMachine.CurrentState;
    }

    public PlayerView PlayerView
    {
        get { return playerView; }
        set { playerView = value; }
    }
    
    #endregion
    
    //Called by Controller when an input is received
    public void ProcessInput(PlayerController.inputState input)
    {
        ((PlayerState)stateMachine.CurrentState).ProcessInput(input);
    }

    public void ProcessInput(PlayerController.inputState input, float value)
    {
        ((PlayerState)stateMachine.CurrentState).ProcessInput(input, value);
    }
    
    #region States

    //Base Player State
    private class PlayerState : StateMachine<PlayerModel>.State
    {
        protected float timer;
        protected string animationTrigger;
        
        public virtual void CooldownAnimationEnded(){}

        //Process our input with optional value
        public virtual void ProcessInput(PlayerController.inputState input){}
        public virtual void ProcessInput(PlayerController.inputState input, float value){}

        public override void OnEnter()
        {
            base.OnEnter();
            Context.playerView.SetAnimationState(animationTrigger);
        }
    }

    private class Walking : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isWalking";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
        }

        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.Walk:
                    break;
                case PlayerController.inputState.EndWalk:
                    TransitionTo<Idle>();
                    break;
                case PlayerController.inputState.Strike:
                    TransitionTo<Striking>();
                    break;
                case PlayerController.inputState.Grab:
                    TransitionTo<Grabbing>();
                    break;
                case PlayerController.inputState.Block:
                    TransitionTo<Blocking>();
                    break;
            }
        }

        public override void ProcessInput(PlayerController.inputState input, float value)
        {
            base.ProcessInput(input, value);
            ProcessInput(input);
            Context.PlayerView.Translate(value, Context.MoveSpeed);
        }
    }

    private class Striking : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isStriking";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = 0.3f;
        }

        public override void Update()
        {
            base.Update();
            timer -= 0.0167f;
            if(timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class Blocking : PlayerState
    {
        public override void ProcessInput(PlayerController.inputState input)
        {
            base.ProcessInput(input);
            switch (input)
            {
                case PlayerController.inputState.EndBlock:
                    TransitionTo<Idle>();
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
                    TransitionTo<Walking>();
                    break;
                case PlayerController.inputState.Strike:
                    TransitionTo<Striking>();
                    break;
                case PlayerController.inputState.Grab:
                    TransitionTo<Grabbing>();
                    break;
                case PlayerController.inputState.Block:
                    TransitionTo<Blocking>();
                    break;
            }
        }
        
        public override void ProcessInput(PlayerController.inputState input, float value)
        {
            base.ProcessInput(input, value);
            ProcessInput(input);
            Context.PlayerView.Translate(value, Context.MoveSpeed);
        }
    }
    private class Victory : PlayerState{}
    #endregion
}