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
        public string AnimState { get; }
        public int PlayerIndex { get; }

        public AnimationChange(string animState, int playerIndex)
        {
            AnimState = animState;
            PlayerIndex = playerIndex;
        }
    }
    
    //Sound Effect
    public class PlaySoundEffect : GameEvent
    {
        public string SoundEffect { get; }

        public PlaySoundEffect(string soundEffect)
        {
            SoundEffect = soundEffect;
        }
    }
    
    //Translate Event
    public class TranslatePos : GameEvent
    {
        public float Speed { get; }
        public float RawAxis { get; }
        public int PlayerIndex { get; }

        public TranslatePos(float rawAxis, float speed, int playerIndex)
        {
            Speed = speed;
            RawAxis = rawAxis;
            PlayerIndex = playerIndex;
        }
    }
    
    //Turn Around Event
    public class TurnAround : GameEvent
    {
        public int PlayerIndex { get; }

        public TurnAround(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }

    //Called from model when the hitboxes become active
    public class HitBoxActive : GameEvent
    {
        public float HitBoxDistance { get; }
        public Vector2 HitBoxSize { get; }
        public int PlayerIndex { get; }
        public bool IsStrike { get; }
        
        public HitBoxActive(float hitBoxDistance, Vector2 hitBoxSize, int playerIndex, bool isStrike)
        {
            HitBoxDistance = hitBoxDistance;
            HitBoxSize = hitBoxSize;
            PlayerIndex = playerIndex;
            IsStrike = isStrike;
        }   
    }
    
    //Get Hit Event
    public class HitOpponent : GameEvent
    {
        public int PlayerIndex { get; }
        public bool IsStrike { get; }
        
        public HitOpponent(int playerIndex, bool isStrike)
        {
            PlayerIndex = playerIndex;
            IsStrike = isStrike;
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
        public PlayerController.InputState NewInput { get; }
        public float Value { get; }
        public int PlayerIndex { get; }

        public ProcessInput(PlayerController.InputState newInput, int playerIndex)
        {
            NewInput = newInput;
            PlayerIndex = playerIndex;
        }

        public ProcessInput(PlayerController.InputState newInput, float value, int playerIndex)
        {
            NewInput = newInput;
            Value = value;
            PlayerIndex = playerIndex;
        }
    }
    
    //Toggle collider Event
    public class ToggleCollider : GameEvent
    {
        public bool IsOn { get; }

        public ToggleCollider(bool isOn)
        {
            IsOn = isOn;
        }
    }
    
    //Game End Event
    public class GameEnd : GameEvent {}
    
    public class TimeOut : GameEvent {}
    
    public class Pause : GameEvent {}
    
    
}
