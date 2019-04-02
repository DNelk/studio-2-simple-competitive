using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fill this with Events which are subclasses of GameEvent
public class Events : MonoBehaviour
{
    //Animation Change Event
    //Fired from Model and received by View
    public class AnimationChange : GameEvent
    {
        public string AnimTrigger { get; }
        public int PlayerIndex { get; }

        public AnimationChange(string animTrigger, int playerIndex)
        {
            AnimTrigger = animTrigger;
            PlayerIndex = playerIndex;
        }
    }
    
    //Translate Event
    public class TranslatePos : GameEvent
    {
        public float Speed { get; }
        public float RawAxis { get; }
        public int PlayerIndex { get; }

        public TranslatePos(float rawAxis, float speed)
        {
            Speed = speed;
            RawAxis = rawAxis;
        }
    }
    
    //Get Hit Event
    public class HitOpponent : GameEvent
    {
        public int PlayerIndex { get; }

        public HitOpponent(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
    
    //Health Changed Event
    public class HealthChanged : GameEvent
    {
        public int NewHealth { get; }
        public int PlayerIndex { get; }

        public HealthChanged(int newHealth, int playerIndex)
        {
            NewHealth = newHealth;
            PlayerIndex = playerIndex;
        }
    }
    
    //Process Input Event
    public class ProcessInput : GameEvent
    {
        public PlayerController.inputState NewInput { get; }
        public float Value { get; }
        public int PlayerIndex { get; }

        public ProcessInput(PlayerController.inputState newInput, int playerIndex)
        {
            NewInput = newInput;
            PlayerIndex = playerIndex;
        }

        public ProcessInput(PlayerController.inputState newInput, float value, int playerIndex)
        {
            NewInput = newInput;
            Value = value;
            PlayerIndex = playerIndex;
        }
    }
    
    //Game End Event
    public class GameEnd : GameEvent {}
    
    public class TimeOut : GameEvent {}
    
    public class Pause : GameEvent {}
    
    
}
