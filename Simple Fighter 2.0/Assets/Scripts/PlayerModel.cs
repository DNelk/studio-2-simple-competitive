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
    private bool isBlocking = false;
    private float rollDir;
    private bool canHeal;
    private bool isHit;
    private bool isCounter;
    private bool hasWon;
    private bool hasLost;
    private bool hasDraw;
    private bool hasTimeLost;
    private float healthTimer;
    private float stopTime = 1; //This is for hitstop set it to 0 when hitstop happens. Do not touch otherwise.
    private StateMachine<PlayerModel> stateMachine;
    #endregion

    #region Public Variables

    //Character Statistics
    [Range(1, 6)] public int MaxHitPoints = 6;
    [Range(1, 50)] public float MoveSpeed = 10;
    public float RegenSpeed = 5;
    public float RollSpeed = 10;
    public float TechSpeed = 15;
    public float StrikeHitBoxDistance = 1;
    public Vector2 StrikeHitBoxSize = new Vector2(1.75f, 1f);
    public float GrabHitBoxDistance = 1;
    public Vector2 GrabHitBoxSize = new Vector2(0.5f, 1f);
    public float HitDelay = .2f;
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
        EventManager.Instance.AddHandler<RoundEnd>(OnRoundEnd);
        
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
        isHit = false;
        isCounter = false;
        hasWon = false;
        hasLost = false;
        hasDraw = false;
    }
    
    private void OnDestroy()
    {
        //Unhookup the Event Manager
        EventManager.Instance.RemoveHandler<ProcessInput>(OnInput);
        EventManager.Instance.RemoveHandler<HitOpponent>(OnHit);
        EventManager.Instance.RemoveHandler<RoundEnd>(OnRoundEnd);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        stateMachine.Update();
        isHit = false;
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
            currentStateType == typeof(RollStartup) ||
            currentStateType == typeof(RollActive)||
            currentStateType == typeof(TechRollStartup)||
            currentStateType == typeof(TechRollActive)||
            currentStateType == typeof(GetUp)||
            currentStateType == typeof(TechUp))
        {
            //No hit
            AudioManager.Instance.PlayAudio(AudioManager.Instance.WhiffAudioClips);
            //EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.WhiffAudioClips));
        }
        else if (currentStateType == typeof(BlockActive) && evt.HitType == "Strike")
        {
            //no hit and counter starts
            //successful block
            isCounter = true;
            AudioManager.Instance.PlayAudio(AudioManager.Instance.BlockedAudioClips);
            EventManager.Instance.Fire(new PlayParticle(PlayerIndex, "Block", currentHitPoints));
        }
        else if (currentStateType == typeof(StrikeActive) && evt.HitType == "Grab")
        {
            //no hit
            AudioManager.Instance.PlayAudio(AudioManager.Instance.WhiffAudioClips);
        }
        else
        {
            //hit
            isHit = true;
            currentHitPoints--;
            canHeal = false;
            EventManager.Instance.Fire(new HealthChanged(currentHitPoints, PlayerIndex));
            if (evt.HitType == "Strike")
            {      
                AudioManager.Instance.PlayAudio(AudioManager.Instance.StrikeAudioClips);
                EventManager.Instance.Fire(new PlayParticle(PlayerIndex, evt.HitType, currentHitPoints));
            }
            else if (evt.HitType == "Grab")
            {
                AudioManager.Instance.PlayAudio(AudioManager.Instance.GrabbedAudioClips);
                EventManager.Instance.Fire(new PlayParticle(PlayerIndex, evt.HitType, currentHitPoints));
            }
            else if (evt.HitType == "Counter")
            {
                AudioManager.Instance.PlayAudio(AudioManager.Instance.StrikeAudioClips);
                EventManager.Instance.Fire(new PlayParticle(PlayerIndex, evt.HitType, currentHitPoints));
            }

            StartCoroutine(HitStop(HitDelay));
        }
    }
    
    //Called when the round is over
    public void OnRoundEnd(RoundEnd evt)
    {
        if (evt.WinnerIndex == PlayerIndex)
            hasWon = true;
        else if (evt.Draw)
            hasDraw = true;
        else if (evt.LoserIndex == PlayerIndex && evt.TimeOut)
            hasLost = true;
        else if (evt.LoserIndex == PlayerIndex)
        {
            hasLost = true;
        }
    }
    
    //Hit Stop Coroutine
    IEnumerator HitStop(float hitDelay)
    {
        stopTime = 0;
        EventManager.Instance.Fire(new StopTime());
        yield return new WaitForSeconds(hitDelay);
        stopTime = 1;
        EventManager.Instance.Fire(new RestartTime());
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
            //Round End Checking
            if (Context.hasWon)
            {
                TransitionTo<Victory>();
                return;
            }
            if (Context.hasLost)
            {
                TransitionTo<Loss>();
                return;
            }

            if (Context.hasDraw)
            {
                TransitionTo<Draw>();
                return;
            }

            if (Context.hasTimeLost)
            {
                TransitionTo<TimeLose>();
                return;
            }

            //Healing
            if (Context.canHeal)
            {
                Context.healthTimer -= Time.deltaTime * Context.stopTime;
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
            EventManager.Instance.Fire(new TurnAround(Context.PlayerIndex));
        }
        
        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            EventManager.Instance.Fire(new TurnAround(Context.PlayerIndex));
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.Walk:
                    EventManager.Instance.Fire(new TranslatePos(value, Context.MoveSpeed * Context.stopTime, Context.PlayerIndex));
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
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
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
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            //Turn on hitbox
            if (!Context.hasHit)
                EventManager.Instance.Fire(new HitBoxActive(Context.StrikeHitBoxDistance, Context.StrikeHitBoxSize, Context.PlayerIndex, "Strike"));
            
            //Countdown to recovery
            timer -= Time.deltaTime * Context.stopTime;     
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
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
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
            timer -= Time.deltaTime * Context.stopTime;
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
                EventManager.Instance.Fire(new HitBoxActive(Context.StrikeHitBoxDistance, Context.StrikeHitBoxSize, Context.PlayerIndex, "Counter"));
            
            //Countdown to recovery
            timer -= Time.deltaTime * Context.stopTime;     
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
            timer -= Time.deltaTime * Context.stopTime;
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
            Context.isBlocking = true;
            Context.isCounter = false;
        }

        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
            if (timer <= 0)
                TransitionTo<BlockActive>();
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.EndBlock:
                    Context.isBlocking = false;
                    break;
            }
        }
    }
    
    private class BlockActive : PlayerState
    {

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_BlockActive", Context.PlayerIndex));
            EventManager.Instance.Fire(new PlayParticle(Context.PlayerIndex, "BlockBubble", Context.currentHitPoints));
        }

        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            if (Context.isCounter)
            {
                Context.isCounter = false;
                TransitionTo<CounterStartup>();
                return;
            }
            if (!Context.isBlocking)
                TransitionTo<BlockRecovery>();
        }
        
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.EndBlock:
                    Context.isBlocking = false;
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
            EventManager.Instance.Fire(new PlayParticle(Context.PlayerIndex, "BlockBubble", Context.currentHitPoints));
            timer = Context.stateTimers["BlockRecovery"];
        }

        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime; 
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
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
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
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
            //Active
            if (!Context.hasHit)
                EventManager.Instance.Fire(new HitBoxActive(Context.GrabHitBoxDistance, Context.GrabHitBoxSize, Context.PlayerIndex, "Grab"));
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
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
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
            timer -= Time.deltaTime * Context.stopTime;
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
            timer -= Time.deltaTime * Context.stopTime;
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
                        TransitionTo<TechRollStartup>();
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
            timer -= Time.deltaTime * Context.stopTime;
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
            AudioManager.Instance.PlayAudio(AudioManager.Instance.LandingAudioClips);
            AudioManager.Instance.PlayAudio(AudioManager.Instance.CrowdAudioClips);
            //EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.LandingAudioClips));
            //EventManager.Instance.Fire(new PlaySoundEffect(AudioManager.Instance.CrowdAudioClips));
            timer = Context.stateTimers["GroundedMax"];
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
                TransitionTo<GetUp>();
        }
        public override void ProcessInput(PlayerController.InputState input, float value)
        {
            base.ProcessInput(input, value);
            switch (input)
            {
                case PlayerController.InputState.Roll:
                    Context.rollDir = value;
                    TransitionTo<RollStartup>();
                    break;
                case PlayerController.InputState.GetUp:
                    TransitionTo<GetUp>();
                    break;
            }
        }
    }

    private class RollStartup : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["RollStartup"];
            EventManager.Instance.Fire(new AnimationChange("Player_Roll", Context.PlayerIndex));
            EventManager.Instance.Fire(new ToggleCollider(true));
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
            if (timer <= 0)
                TransitionTo<RollActive>();
        }
    }

    private class RollActive : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["RollActive"];
        }

        public override void Update()
        {
            base.Update();
            EventManager.Instance.Fire(new TranslatePos(Context.rollDir, Context.RollSpeed, Context.PlayerIndex));
            timer -= Time.deltaTime * Context.stopTime;
            if(timer <= 0)
                TransitionTo<RollRecovery>();
        }
    }

    private class RollRecovery : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["RollRecovery"];
            EventManager.Instance.Fire(new ToggleCollider(false));
        }

        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
            if (timer <= 0)
                TransitionTo<Idle>();
        }
    }

    private class TechRollStartup : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["TechRollStartup"];
            EventManager.Instance.Fire(new AnimationChange("Player_Roll", Context.PlayerIndex));
            EventManager.Instance.Fire(new ToggleCollider(true));
            EventManager.Instance.Fire(new PlayParticle(Context.PlayerIndex, "Tech", Context.currentHitPoints));
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
            if (timer <= 0)
                TransitionTo<TechRollActive>();
        }
    }

    private class TechRollActive : PlayerState
    {
        public override void Init()
        {
            base.Init();
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            timer = Context.stateTimers["TechRollActive"];
        }

        public override void Update()
        {
            base.Update();
            EventManager.Instance.Fire(new TranslatePos(Context.rollDir, Context.TechSpeed, Context.PlayerIndex));
            timer -= Time.deltaTime * Context.stopTime;
            if(timer <= 0)
                TransitionTo<TechRollRecovery>();
        }
    }

    private class TechRollRecovery : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new ToggleCollider(false));
            timer = Context.stateTimers["TechRollRecovery"];
        }

        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
            if (timer <= 0)
                TransitionTo<Idle>();
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
            timer -= Time.deltaTime * Context.stopTime;
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
            EventManager.Instance.Fire(new PlayParticle(Context.PlayerIndex, "Tech", Context.currentHitPoints));
        }

        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime * Context.stopTime;
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
            else if (Context.canHeal == false)
            {
                Context.canHeal = true;
                Context.healthTimer = Context.RegenSpeed;
            }
            EventManager.Instance.Fire(new AnimationChange("Player_Idle", Context.PlayerIndex));
            EventManager.Instance.Fire(new TurnAround(Context.PlayerIndex));
        }

        public override void Update()
        {
            if (Context.isHit)
            {
                TransitionTo<FallStartup>();
                return;
            }
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
        }

        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_Win", Context.PlayerIndex));
        }
    }

    private class Loss : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_Grounded", Context.PlayerIndex));
        }
    }

    private class TimeLose : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_KO", Context.PlayerIndex));
        }
    }

    private class Draw : PlayerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EventManager.Instance.Fire(new AnimationChange("Player_Idle", Context.PlayerIndex));
        }
    }
    #endregion
}