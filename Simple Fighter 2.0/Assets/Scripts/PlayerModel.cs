﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Events;

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
    [Range(1, 50)] public float MoveSpeed = 10;
    [Range(1, 10)] public int StrikeStartupFrames;
    [Range(1, 10)] public int StrikeActiveFrames;
    [Range(1, 10)] public int StrikeRecoveryFrames;
    [Range(1, 10)] public int GrabStartupFrames;
    [Range(1, 10)] public int GrabActiveFrames;
    [Range(1, 10)] public int GrabRecoveryFrames;
    [Range(1, 10)] public int BlockStartupFrames;
    [Range(1, 10)] public int BlockRecoveryFrames;
    [Range(1, 50)] public float GetUpSpeed = 10;
    public int PlayerIndex;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Initialize state machine
        stateMachine = new StateMachine<PlayerModel>(this);
        stateMachine.TransitionTo<Idle>();
        //Initialize event manager
        EventManager.Instance.AddHandler<ProcessInput>(OnInput);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<ProcessInput>(OnInput);
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

    #endregion

    #region Public Functions
    //Called by Controller when an input is received
    public void OnInput(ProcessInput evt)
    {
        if (PlayerIndex != evt.PlayerIndex)
            return;
        ((PlayerState)stateMachine.CurrentState).ProcessInput(evt.NewInput, evt.Value);
    }

    //Gets called when we win
    public void WinRound()
    {
        stateMachine.TransitionTo<Victory>();    
    }
    #endregion
    #region States

    //Base Player State
    private class PlayerState : StateMachine<PlayerModel>.State
    {
        protected float timer;
        protected string animationTrigger;
        
        public virtual void CooldownAnimationEnded(){}

        //Process our input with optional value
        public virtual void ProcessInput(PlayerController.InputState input, float value){}

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new Events.AnimationChange(animationTrigger, Context.PlayerIndex));
        }
    }

    private class Walking : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isWalking";
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.Walk:
                    EventManager.Instance.Fire(new TranslatePos(value, Context.MoveSpeed, Context.PlayerIndex));
                    break;
                case PlayerController.InputState.EndWalk:
                    TransitionTo<Idle>();
                    break;
                case PlayerController.InputState.Strike:
                    TransitionTo<Striking>();
                    break;
                case PlayerController.InputState.Grab:
                    TransitionTo<Grabbing>();
                    break;
                case PlayerController.InputState.Block:
                    TransitionTo<Blocking>();
                    break;
            }
        }
    }

    private class Striking : PlayerState
    {
        private float maxTime;
        private float activeWindowEnter;
        private float activeWindowExit;
        
        public override void Init()
        {
            base.Init();
            activeWindowEnter = maxTime - Context.StrikeStartupFrames;
            animationTrigger = "isStriking";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = 0.3f;
            maxTime = timer;
        }

        public override void Update()
        {
            base.Update();
            timer -= 0.0167f;
            if (timer <= activeWindowEnter)
            {
                //Activate the hitbox
            }
            if(timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class Blocking : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isBlocking";
        }

        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.EndBlock:
                    TransitionTo<Idle>();
                    break;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            EventManager.Instance.Fire(new Events.AnimationChange("endBlock", Context.PlayerIndex));
            
        }
    }

    private class Grabbing : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isGrabbing";
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

    private class Falling : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "gotStriked";
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
                TransitionTo<Grounded>();
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.Roll:
                    TransitionTo<Rolling>();
                    Context.GetUpSpeed = 15;
                    break;
                case PlayerController.InputState.GetUp:
                    TransitionTo<GetUp>();
                    Context.GetUpSpeed = 15;
                    break;
            }
        }
    }

    private class Grounded : PlayerState
    {
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.Roll:
                    TransitionTo<Rolling>();
                    Context.GetUpSpeed = 10;
                    break;
                case PlayerController.InputState.GetUp:
                    TransitionTo<GetUp>();
                    Context.GetUpSpeed = 10;
                    break;
            }
        }
    }

    private class Rolling : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isRolling";
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

    private class GetUp : PlayerState
    {
        public override void Init()
        {
            base.Init();
            animationTrigger = "isGettingUp";
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
    
    private class Idle : PlayerState
    {
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.Walk:
                    TransitionTo<Walking>();
                    EventManager.Instance.Fire(new TranslatePos(value, Context.MoveSpeed, Context.PlayerIndex));
                    break;
                case PlayerController.InputState.Strike:
                    TransitionTo<Striking>();
                    break;
                case PlayerController.InputState.Grab:
                    TransitionTo<Grabbing>();
                    break;
                case PlayerController.InputState.Block:
                    TransitionTo<Blocking>();
                    break;
            }
        }
    }
    
    private class Victory : PlayerState{}
    #endregion
}