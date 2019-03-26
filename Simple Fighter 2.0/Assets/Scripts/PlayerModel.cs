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

    #region States

    //Base Player State
    private class PlayerState : StateMachine<PlayerModel>.State
    {
        private int timer;

        public virtual void CooldownAnimationEnded(){}     
    }

    private class Walking : PlayerState{}
    private class Striking : PlayerState{}
    private class Blocking : PlayerState{}
    private class Grabbing : PlayerState{}
    private class Falling : PlayerState{}
    private class Grounded : PlayerState{}
    private class Rolling : PlayerState{}
    private class Idle : PlayerState{}
    private class Victory : PlayerState{}
    #endregion
}