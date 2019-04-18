using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
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
    private bool canHeal;
    private float healthTimer;
    private StateMachine<PlayerModel> stateMachine;
    #endregion

    #region Public Variables

    //Character Statistics
    [Range(1, 6)] public int MaxHitPoints = 6;
    [Range(1, 50)] public float MoveSpeed = 10;
    public float RollSpeed = 10;
    public float TechSpeed = 15;
    public float StrikeHitBoxDistance = 1;
    public Vector2 StrikeHitBoxSize = new Vector2(1.75f, 1f);
    public float GrabHitBoxDistance = 1;
    public Vector2 GrabHitBoxSize = new Vector2(0.5f, 1f);
    public int PlayerIndex;
    public StateTimers[] StateTimers; //this is where you put the timers for each state
    private Dictionary<string, float> stateTimers = new Dictionary<string, float>(); //This is where that is read
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Initialize state machine
        stateMachine = new StateMachine<PlayerModel>(this);
        //Initialize event manager
        EventManager.Instance.AddHandler<ProcessInput>(OnInput);
        EventManager.Instance.AddHandler<HitOpponent>(OnHit);
        
        //Setup dictionary for stateTimers        
        foreach (StateTimers st in StateTimers)
        {
            stateTimers.Add(st.key, st.time);
        }
        
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
    void FixedUpdate()
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
        if (PlayerIndex != evt.PlayerIndex || GameManager.Instance.CurrentManagerState != ManagerState.Fighting)
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
            currentStateType == typeof(FallStartup) ||
            currentStateType == typeof(FallActive)||
            currentStateType == typeof(FallRecovery)||
            currentStateType == typeof(Rolling) || 
            currentStateType == typeof(GetUp))
        {
            Debug.Log("dont hit me");
            EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.WhiffAudioClips));
        }
        else if (currentStateType == typeof(BlockActive) && evt.IsStrike)
        {
            Debug.Log("I am blocking, no hit");
            //if we're opposite directions, successful block
            stateMachine.TransitionTo<CounterStartup>();
            EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.BlockedAudioClips));
        }
        else if (currentStateType == typeof(StrikeActive) && !evt.IsStrike)
        {
            //if we're opposite directions, they got us
            EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.WhiffAudioClips));
        }
        else
        {
            stateMachine.TransitionTo<FallStartup>();
            currentHitPoints--;
            Debug.Log(PlayerIndex + " health = " + currentHitPoints);
            canHeal = false;
            EventManager.Instance.Fire(new HealthChanged(currentHitPoints, PlayerIndex));
            if(evt.IsStrike)
                EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.StrikeAudioClips));
            else
                EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.GrabbedAudioClips));

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
            
        }

        public override void Update()
        {
            base.Update();
            if (Context.canHeal)
            {
                Context.healthTimer -= Time.deltaTime;
                if (Context.healthTimer <= 0)
                {
                    Context.canHeal = false;
                    Context.currentHitPoints++;
                    EventManager.Instance.Fire(new HealthChanged(Context.currentHitPoints, Context.PlayerIndex));
                }
            }
        }
    }

    private class Walking : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            EventManager.Instance.Fire(new AnimationChange("Player_Walking", Context.PlayerIndex));
        }
        
        public override void Update()
        {
            base.Update();
            
            EventManager.Instance.Fire(new TurnAround(Context.PlayerIndex));
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
                    TransitionTo<StrikeStartup>();
                    break;
                case PlayerController.InputState.Grab:
                    TransitionTo<GrabStartup>();
                    break;
                case PlayerController.InputState.Block:
                    TransitionTo<BlockStartup>();
                    break;
            }
        }
    }

    //Startup simply activates the animation trigger and running down a timer
    private class StrikeStartup : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["StrikeStartup"];
            EventManager.Instance.Fire(new AnimationChange("Player_Strike", Context.PlayerIndex));
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TransitionTo<StrikeActive>();
            }
        }
    }

    //Active is where the state fires its function -- hitboxes for attacks, blocking for blocking
    private class StrikeActive : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["StrikeActive"];
            Context.hasHit = false; //a hit has not registered this attack yet
        }

        public override void Update()
        {
            base.Update();
            //Turn on hitbox
            if (!Context.hasHit)
                EventManager.Instance.Fire(new HitBoxActive(Context.StrikeHitBoxDistance, Context.StrikeHitBoxSize, Context.PlayerIndex, true));
            
            //Countdown to recovery
            timer -= Time.deltaTime;     
            if(timer <= 0)
                TransitionTo<StrikeRecovery>();
        }
    }

    //recovery just sets a timer and counts down
    private class StrikeRecovery : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["StrikeRecovery"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class CounterStartup : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["CounterStartup"];
            EventManager.Instance.Fire(new AnimationChange("Player_Counter", Context.PlayerIndex));
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TransitionTo<CounterActive>();
            }
        }
    }

    private class CounterActive : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["CounterActive"];
            Context.hasHit = false; //a hit has not registered this attack yet
        }

        public override void Update()
        {
            base.Update();
            //Turn on hitbox
            if (!Context.hasHit)
                EventManager.Instance.Fire(new HitBoxActive(Context.StrikeHitBoxDistance, Context.StrikeHitBoxSize, Context.PlayerIndex, true));
            
            //Countdown to recovery
            timer -= Time.deltaTime;     
            if(timer <= 0)
                TransitionTo<CounterRecovery>();
        }
    }

    private class CounterRecovery : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["CounterRecovery"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class BlockStartup : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_BlockStartup", Context.PlayerIndex));
            timer = Context.stateTimers["BlockStartup"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<BlockActive>();
        }
    }
    
    private class BlockActive : PlayerState
    {

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_BlockActive", Context.PlayerIndex));
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.EndBlock:
                    TransitionTo<BlockRecovery>(); 
                    break;
            }
        }
    }

    private class BlockRecovery : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_BlockRecovery", Context.PlayerIndex));
            timer = Context.stateTimers["BlockRecovery"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class GrabStartup : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_Grab", Context.PlayerIndex));
            timer = Context.stateTimers["GrabStartup"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<GrabActive>();
        }
    }
    
    private class GrabActive : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["GrabActive"];
            Context.hasHit = false;
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            //Active
            if (!Context.hasHit)
                EventManager.Instance.Fire(new HitBoxActive(Context.GrabHitBoxDistance, Context.GrabHitBoxSize, Context.PlayerIndex, false));
            if(timer <= 0)
                TransitionTo<GrabRecovery>();
        }
    }

    private class GrabRecovery : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["GrabRecovery"];
            Context.canHeal = false;
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class FallStartup : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_GetStriked", Context.PlayerIndex));
            timer = Context.stateTimers["FallStartup"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<FallActive>();
        }
    }

    private class FallActive : PlayerState
    {
        private bool hasPressed;
        
        public override void Init()
        {
            base.Init();
            hasPressed = true;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["FallActive"];        
            hasPressed = true;
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if(timer <= 0)
                TransitionTo<FallRecovery>();
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.MoveRelease:
                    hasPressed = false;
                    break;
                case PlayerController.InputState.Roll:
                    if (!hasPressed)
                    {
                        Context.rollDir = value;
                        TransitionTo<TechRolling>();
                    }
                    break;
                case PlayerController.InputState.GetUp:
                    if (!hasPressed)
                        TransitionTo<TechUp>();
                    break;
            }
        }
    }

    private class FallRecovery : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["FallRecovery"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<Grounded>();
        }
    }

    private class Grounded : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_Grounded", Context.PlayerIndex));
            EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.LandingAudioClips));
            EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.CrowdAudioClips));
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

    private class Rolling : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["Rolling"];
            EventManager.Instance.Fire(new AnimationChange("Player_Roll", Context.PlayerIndex));
            EventManager.Instance.Fire(new ToggleCollider(true));
        }

        public override void Update()
        {
            base.Update();
            EventManager.Instance.Fire(new TranslatePos(Context.rollDir, Context.RollSpeed, Context.PlayerIndex));
            timer -= Time.deltaTime;
            if(timer <= 0)
                TransitionTo<Idle>();
        }

        public override void OnExit()
        {
            base.OnExit();
            EventManager.Instance.Fire(new ToggleCollider(false));
        }
    }

    private class TechRolling : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["TechRolling"];
            EventManager.Instance.Fire(new AnimationChange("Player_Roll", Context.PlayerIndex));
            EventManager.Instance.Fire(new ToggleCollider(true));
        }

        public override void Update()
        {
            base.Update();
            EventManager.Instance.Fire(new TranslatePos(Context.rollDir, Context.TechSpeed, Context.PlayerIndex));
            timer -= Time.deltaTime;
            if(timer <= 0)
                TransitionTo<Idle>();
        }

        public override void OnExit()
        {
            base.OnExit();
            EventManager.Instance.Fire(new ToggleCollider(false));
        }
    }

    private class GetUp : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_GetUp", Context.PlayerIndex));
            timer = Context.stateTimers["GetUp"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if(timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class TechUp : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_GetUp", Context.PlayerIndex));
            timer = Context.stateTimers["TechUp"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if(timer <= 0)
                TransitionTo<Idle>();
        }
    }
    
    private class Idle : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Context.currentHitPoints % 2 == 0)
            {
                Context.canHeal = false;
            }
            else
            {
                Context.canHeal = true;
                Context.healthTimer = 1f;
            }
            EventManager.Instance.Fire(new AnimationChange("Player_Idle", Context.PlayerIndex));
        }

        public override void Update()
        {
            base.Update();
            EventManager.Instance.Fire(new TurnAround(Context.PlayerIndex));
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
                    TransitionTo<StrikeStartup>();
                    break;
                case PlayerController.InputState.Grab:
                    TransitionTo<GrabStartup>();
                    break;
                case PlayerController.InputState.Block:
                    TransitionTo<BlockStartup>();
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