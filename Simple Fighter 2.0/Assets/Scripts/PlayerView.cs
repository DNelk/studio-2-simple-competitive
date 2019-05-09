using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using static Events;

public class PlayerView : MonoBehaviour
{
    #region Private Variables
    
    private Animator animator;
    private SpriteRenderer spriteRen;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private int direction;
    private int opponentLayer;
    private GameObject blockBubble; //holds the block bubble game object for blocking
    private GameObject UIcanvas;
        
    #endregion
    
    #region Public Variables
    public int PlayerIndex;
    public PlayerModel PlayerModelState;
    public float MoveCastDistance;
    #endregion
    
    private void Awake()
    {
        //Create Components
        animator = gameObject.AddComponent<Animator>();

        spriteRen = gameObject.AddComponent<SpriteRenderer>();
        spriteRen.sortingOrder = 5;
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.isKinematic = false;
        rb.freezeRotation = true;
        
        col = gameObject.AddComponent<BoxCollider2D>();
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        EventManager.Instance.AddHandler<AnimationChange>(OnAnimationChange);
        EventManager.Instance.AddHandler<TranslatePos>(Translate);
        EventManager.Instance.AddHandler<HitBoxActive>(OnHitBoxActive);
        EventManager.Instance.AddHandler<ToggleCollider>(OnToggleCollider);
        EventManager.Instance.AddHandler<TurnAround>(OnTurnAround);
        EventManager.Instance.AddHandler<RestartTime>(OnRestartTime);
        EventManager.Instance.AddHandler<StopTime>(OnStopTime);
        EventManager.Instance.AddHandler<PlayParticle>(OnPlayParticle);
        transform.localScale *= 0.15844f;
        
        //Assign sprite and animator
        spriteRen.sprite = Resources.Load<Sprite>("Textures/NormalStance");
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Player");
        
        col.offset = new Vector2(1.155829f, -2.894697f);
        col.size = new Vector2(5.765483f, 20.96054f);
        
        //Set Direction
        int rotInt = Mathf.RoundToInt(transform.rotation.eulerAngles.y);
        if (rotInt == 180)
        {
            direction = 1;
        }
        else if (rotInt == 0)
        {
            direction = -1;
        }
        else
            Debug.Log("error: direction not found");
        
        //Set View layer for Hit Detection
        if (PlayerIndex == 0)
        {
            gameObject.layer = LayerMask.NameToLayer("1PBox");
            opponentLayer = LayerMask.GetMask("2PBox");
        }
        else if (PlayerIndex == 1)
        {
            gameObject.layer = LayerMask.NameToLayer("2PBox");
            opponentLayer = LayerMask.GetMask("1PBox");
        }
        else
        {
            Debug.Log("LAYER NOT SET FOR VIEW " + PlayerIndex);
        }
        
        //set canvas to call textEffect
        UIcanvas = GameObject.Find("Canvas");
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<AnimationChange>(OnAnimationChange);
        EventManager.Instance.RemoveHandler<TranslatePos>(Translate);
        EventManager.Instance.RemoveHandler<HitBoxActive>(OnHitBoxActive);
        EventManager.Instance.RemoveHandler<ToggleCollider>(OnToggleCollider);
        EventManager.Instance.RemoveHandler<TurnAround>(OnTurnAround);
        EventManager.Instance.RemoveHandler<RestartTime>(OnRestartTime);
        EventManager.Instance.RemoveHandler<StopTime>(OnStopTime);
        EventManager.Instance.RemoveHandler<PlayParticle>(OnPlayParticle);
    }
    
    //Play the correct particle
    public void OnPlayParticle(PlayParticle evt)
    {
        if (evt.PlayerIndex == PlayerIndex)
        {
            if (evt.ParticleType == "Block")
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Guard_Counter"),transform);
                Destroy(blockBubble);
            }
            else if (evt.ParticleType == "Tech")
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/TechRoll"),transform);
            }
            else if (evt.ParticleType == "BlockBubble")
            {
                if (blockBubble == null)
                {
                    blockBubble = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Guard"),transform) as GameObject;
                }
                else
                {
                    Destroy(blockBubble);
                }
            }
        }
        else if (evt.ParticleType == "Strike")
        {
            if (evt.Health % 2 == 0)
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Attacks_Break"),transform);
            } 
            else
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Attacks_Normal"),transform);
            }
        }
        else if (evt.ParticleType == "Grab")
        {
            if (evt.Health % 2 == 0)
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Kick_Break"),transform);
            }
            else
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Kick_Normal"),transform);
            }   
        }
        else if (evt.ParticleType == "Counter")
        {
            if (evt.Health % 2 == 0)
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Counter_Break"),transform);
            }
            else
            {
                Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/Counter_Normal"),transform);
            }
            Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/CounterText"),UIcanvas.transform);
        }
    }
    
    //Stop animations when damage is registered for hitstop
    public void OnStopTime(StopTime evt)
    {
        animator.speed = 0;
    }
    
    //Start animations back up when time restarts
    public void OnRestartTime(RestartTime evt)
    {
        animator.speed = 1;
    }
    
    //Allows us to toggle collider off while rolling
    public void OnToggleCollider(ToggleCollider evt)
    {
        if (evt.IsOn)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        else if (!evt.IsOn)
        {
            GetComponent<Collider2D>().enabled = true;
        }
    }

    //Allows us to call animator from model -- replace with event system
    public void OnAnimationChange(AnimationChange evt)
    {
        if (evt.PlayerIndex != PlayerIndex)
            return;
        animator.Play(evt.AnimState, 0);
    }
     
    //Move the player by amount times speed
    public void Translate(TranslatePos evt)
    {
        if (PlayerIndex != evt.PlayerIndex)
            return;
        
        float amount = evt.RawAxis;
        float speed = evt.Speed;
        float oldX = transform.position.x; //Our old position

        RaycastHit2D oppCol =
            Physics2D.Raycast(transform.position, Vector2.right * amount, MoveCastDistance, opponentLayer);
        RaycastHit2D wallCol = Physics2D.Raycast(transform.position, Vector2.right * amount, MoveCastDistance,
            LayerMask.GetMask("Bounds"));

        if (!wallCol)
        {
            transform.position += Vector3.right * amount * Time.deltaTime * speed;
        }
    }

    public void OnTurnAround(TurnAround evt)
    {
        if (PlayerIndex != evt.PlayerIndex)
            return;

        RaycastHit2D hitCol = Physics2D.Raycast(transform.position, Vector2.left * direction, 20, opponentLayer);
        if (hitCol)
        {
            transform.Rotate(0f, 180f, 0f); //rotate the sprite
            direction *= -1; //Flip the hitbox directions
        }
    }

    public void OnHitBoxActive(HitBoxActive evt)
    {
        if (evt.PlayerIndex != PlayerIndex)
            return;
        //Debug.Log("HitBox Active " + PlayerIndex);
        Vector2 hitBoxCenter = new Vector2(transform.position.x + evt.HitBoxDistance * direction, 0);
        Collider2D hitCol = Physics2D.OverlapBox(hitBoxCenter, evt.HitBoxSize, 0, opponentLayer);

        if (hitCol)
        {
            EventManager.Instance.Fire(new HitOpponent(PlayerIndex, evt.HitType));
        }
    }
    #region Tools
    /*private void OnDrawGizmos()
    {
        //Draw strike hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + StrikeHitBoxDistance, 0), StrikeHitBoxSize);
        
        //Draw Grab hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + GrabHitBoxDistance, 0), GrabHitBoxSize);
    }*/
    #endregion
}
