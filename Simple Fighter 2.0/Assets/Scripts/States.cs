using System;
using System.Collections.Generic;
using System.Diagnostics;

/*Credit to Mattia Romeo for basis of this code*/

public class StateMachine<TContext>
{
    //What object we're a state machine to
    private readonly TContext context;

    //Dictionary of states
    private readonly Dictionary<Type, State> stateCache = new Dictionary<Type, State>();
    
    //Our current state
    public State CurrentState { get; private set; }

    //The state we're trying to be
    private State pendingState;

    //Constructor
    public StateMachine(TContext newContext)
    {
        context = newContext;
    }

    //Update our states
    public void Update()
    {
        //Go to a pending state if we have one
        TransitionToPending();
        //Update the state
        CurrentState.Update();
        //We might have changed state again
        TransitionToPending();
        
    }
    
    // Queues transition to a new state
    public void TransitionTo<TState>() where TState : State
    {
        // We do the actual transtion
        pendingState = GetState<TState>();
    }
    
    private void TransitionToPending()
    {
        if (pendingState != null)
        {
            if (CurrentState != null) CurrentState.OnExit();
            CurrentState = pendingState;
            CurrentState.OnEnter();
            pendingState = null;
        }
    }

    private TState GetState<TState>() where TState : State
    {
        State state;
        //See if we've done this state before, if so we pass it back
        if (stateCache.TryGetValue(typeof(TState), out state))
        {
            return (TState) state;
        }
        else
        {
            var newState = Activator.CreateInstance<TState>();
            newState.Parent = this;
            newState.Init();
            stateCache[typeof(TState)] = newState;
            return newState;
        }
    }
    
    //Abstract State Class
    public abstract class State
    {
        internal StateMachine<TContext> Parent { get; set; }

        protected TContext Context
        {
            get { return Parent.context; }
        }
        
        // A convenience method for transitioning from inside a state
        protected void TransitionTo<TState>() where TState : State
        {
            Parent.TransitionTo<TState>();
        }
        
        // This is called once when the state is first created (think of it like Unity's Awake)
        public virtual void Init() { }

        // This is called whenever the state becomes active (think of it like Unity's Start)
        public virtual void OnEnter() { }
        // this is called whenever the state becomes inactive

        public virtual void OnExit() { }

        // This is your standard update method where most of your work should go
        public virtual void Update() { }

        // called when the state machine is cleared, and where you should clear resources
        public virtual void CleanUp() { }
    }
}

