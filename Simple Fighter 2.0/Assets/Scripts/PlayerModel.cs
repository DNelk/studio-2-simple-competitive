using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Events;

public class PlayerModel : MonoBehaviour
{

    #region Private Variables

    //Current hitpoints
    private int currentHitPoints;
    private bool hasHit;
    private bool isBlocking;
    private float rollDir;
    private StateMachine<PlayerModel> stateMachine;
    #endregion

    #region Public Variables

    //Character Statistics
    [Range(1, 6)] public int MaxHitPoints = 6;
    [Range(1, 50)] public float MoveSpeed = 10;
    public float RollSpeed = 10;
    public float TechSpeed = 15;
    [Range(1, 10)] public float StrikeStartupFrames = .1f;
    [Range(1, 10)] public float StrikeActiveFrames = .1f;
    public float StrikeHitBoxDistance = 1;
    public Vector2 StrikeHitBoxSize = new Vector2(1.75f, 1f);
    [Range(1, 10)] public int StrikeRecoveryFrames; //currently does nothing. It's just what is leftover in the animation
    [Range(1, 10)] public float GrabStartupFrames = .1f;
    [Range(1, 10)] public float GrabActiveFrames = .1f;
    [Range(1, 10)] public int GrabRecoveryFrames; //currently does nothing. It's just what is leftover in the animation
    public float GrabHitBoxDistance = 1;
    public Vector2 GrabHitBoxSize = new Vector2(0.5f, 1f);
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
        //Initialize event manager
        EventManager.Instance.AddHandler<ProcessInput>(OnInput);
        EventManager.Instance.AddHandler<HitOpponent>(OnHit);
        
        Init();
        
    }

    //Init our variables
    public void Init()
    {
        currentHitPoints = MaxHitPoints;
        stateMachine.TransitionTo<Idle>();
        hasHit = false;
    }
    
    private void OnDestroy()
    {
        //Unhookup the Event Manager
        EventManager.Instance.RemoveHandler<ProcessInput>(OnInput);
        EventManager.Instance.RemoveHandler<HitOpponent>(OnHit);
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

    public void WinRound()
    {
        stateMachine.TransitionTo<Victory>();
    }
    #endregion
    
    //Called by Controller when an input is received
    public void OnInput(ProcessInput evt)
    {
        if (PlayerIndex != evt.PlayerIndex)
            return;
        ((PlayerState)stateMachine.CurrentState).ProcessInput(evt.NewInput, evt.Value);
    }
    
    //Called by View when a hit is detected
    //NOTE: The OTHER Player's view Fires this event. That is fine, but it is important to note
    public void OnHit(HitOpponent evt)
    {
        if (PlayerIndex == evt.PlayerIndex)//The event is fired from the OTHER player, so if it is our own player index, we do not act on this
        {
            hasHit = true;
            return;
        }
        //if not blocking && not grounded && not getup && not rolling// make it a bool that is only true when actually in active frames
        Type currentStateType = stateMachine.CurrentState.GetType();
        
        if (currentStateType == typeof(Grounded) || 
            currentStateType == typeof(Falling) || 
            currentStateType == typeof(Rolling) || 
            currentStateType == typeof(GetUp))
        {
            Debug.Log("dont hit me");
        }
        else if (currentStateType == typeof(Blocking) && evt.IsStrike)
        {
            Debug.Log("I am blocking, no hit");
            //if we're opposite directions, successful block
        }
        else if (currentStateType == typeof(Striking) && !evt.IsStrike)
        {
            //if we're opposite directions, they got us
        }
        else
        {
            stateMachine.TransitionTo<Falling>();
            currentHitPoints--;
            Debug.Log(PlayerIndex + " health = " + currentHitPoints);
            EventManager.Instance.Fire(new HealthChanged(currentHitPoints, PlayerIndex));
        }
       
        
        //
    }
    
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
            animationTrigger = "isStriking";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = 0.3f;
            maxTime = timer;
            activeWindowEnter = maxTime - Context.StrikeStartupFrames;
            activeWindowExit = activeWindowEnter - Context.StrikeActiveFrames;
            Context.hasHit = false;
        }

        public override void Update()
        {
            base.Update();
            timer -= 0.0167f;
            if (timer <= activeWindowEnter && timer > activeWindowExit && !Context.hasHit)
            {
                //Active
                Debug.Log("Fire hitbox active event " + Context.PlayerIndex);
                EventManager.Instance.Fire(new HitBoxActive(Context.StrikeHitBoxDistance, Context.StrikeHitBoxSize, Context.PlayerIndex, true));
            }
            else if (timer <= activeWindowExit)
            {
                //Recovery
            }
            if(timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class Blocking : PlayerState
    {

        private float timer;
        private float startupTime = .3f;
        private float recoveryTime = .3f;

        private bool blocking;
        private bool blockReleased;
        
        public override void Init()
        {
            base.Init();
            animationTrigger = "isBlocking";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = startupTime;
            blockReleased = false;
        }

        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.EndBlock:
                    blockReleased = true;
                    EventManager.Instance.Fire(new Events.AnimationChange("blockRelease", Context.PlayerIndex)); 
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (!blockReleased && !blocking)
            {
                timer -= 0.0167f;
                if (timer <= 0)
                {
                    blocking = true; 
                    timer = recoveryTime;
                }
            }
            else if (blockReleased)
            {
                timer -= 0.0167f;
                if (timer <= 0)
                {
                    TransitionTo<Idle>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            blocking = false;
        }
    }

    private class Grabbing : PlayerState
    {
        private float maxTime;
        private float activeWindowEnter;
        private float activeWindowExit;
        
        public override void Init()
        {
            base.Init();
            
            animationTrigger = "isGrabbing";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = 0.3f;
            maxTime = timer;
            activeWindowEnter = maxTime - Context.GrabStartupFrames;
            activeWindowExit = activeWindowEnter - Context.GrabActiveFrames;
            Context.hasHit = false;
        }

        public override void Update()
        {
            base.Update();
            timer -= 0.0167f;
            if (timer <= activeWindowEnter && timer > activeWindowExit && !Context.hasHit)
            {
                //Active
                EventManager.Instance.Fire(new HitBoxActive(Context.GrabHitBoxDistance, Context.GrabHitBoxSize, Context.PlayerIndex, false));
            }
            else if (timer <= activeWindowExit)
            {
                //Recovery
            }
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
                    Context.rollDir = value;
                    TransitionTo<Rolling>();
                    break;
                case PlayerController.InputState.GetUp:
                    TransitionTo<GetUp>();
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
                    Context.rollDir = value;
                    TransitionTo<Rolling>();
                    break;
                case PlayerController.InputState.GetUp:
                    TransitionTo<GetUp>();
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
            timer = .3f;
        }

        public override void Update()
        {
            base.Update();
            EventManager.Instance.Fire(new TranslatePos(Context.rollDir, Context.GetUpSpeed, Context.PlayerIndex));
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
        public override void Init()
        {
            base.Init();
            animationTrigger = "isIdle";
        }
        
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

    private class Victory : PlayerState
    {
        public override void Init()
        {
            base.Init();
            //need victory anim
            animationTrigger = "isGettingUp";
        }
    }
    #endregion
}