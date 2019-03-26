using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Credit to Mattia Romeo for basis of this code*/

public class StateMachine<TContext>
{
    //What object we're a state machine to
    private readonly TContext context;

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

    //Abstract State Class
    public abstract class State
    {
        internal StateMachine<TContext> Parent { get; set; }

        protected TContext Context
        {
            get { return Parent.context; }
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

