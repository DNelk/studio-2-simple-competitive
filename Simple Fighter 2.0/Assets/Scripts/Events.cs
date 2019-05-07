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
        public AudioClip[] Clip { get; }

        public PlaySoundEffect(AudioClip[] clip)
        {
            Clip = clip;
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
        public string HitType { get; }
        
        public HitBoxActive(float hitBoxDistance, Vector2 hitBoxSize, int playerIndex, string hitType)
        {
            HitBoxDistance = hitBoxDistance;
            HitBoxSize = hitBoxSize;
            PlayerIndex = playerIndex;
            HitType = hitType;
        }   
    }
    
    //Get Hit Event
    public class HitOpponent : GameEvent
    {
        public int PlayerIndex { get; }
        public string HitType { get; }
        
        public HitOpponent(int playerIndex, string hitType)
        {
            PlayerIndex = playerIndex;
            HitType = hitType;
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
    
    //Play particle event
    public class PlayParticle : GameEvent
    {
        public string ParticleType { get; }
        public int PlayerIndex { get; }
        public int Health { get; }

        public PlayParticle(int playerIndex, string particleType, int health)
        {
            PlayerIndex = playerIndex;
            ParticleType = particleType;
            Health = health;
        }
    }
    
    //Stop Time Event
    public class StopTime : GameEvent {}
    
    //Restart Time Event
    public class RestartTime : GameEvent {}
    
    //Round End Event
    //Used when someone is KOed at the end of a round
    public class RoundEnd : GameEvent
    {
        public int WinnerIndex;
        public int LoserIndex;
        public bool TimeOut;
        public bool Draw;

        public RoundEnd(int winnerIndex, int loserIndex, bool timeOut, bool draw)
        {
            WinnerIndex = winnerIndex;
            LoserIndex = loserIndex;
            TimeOut = timeOut;
            Draw = draw;
        }
    }
    
    //Match End
    public class MatchEnd : GameEvent
    {
        
    }
    
    public class Pause : GameEvent {}
}
