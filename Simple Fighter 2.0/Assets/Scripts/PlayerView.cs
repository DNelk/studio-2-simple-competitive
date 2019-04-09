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
        
    #endregion

    #region Hitbox Tweaking Vars
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance;
    public PlayerModel PlayerModelState;
    #endregion
    
    #region Public Variables
    public int PlayerIndex;
    #endregion
    
    
    
    private void Awake()
    {
        //Create Components
        animator = gameObject.AddComponent<Animator>();

        spriteRen = gameObject.AddComponent<SpriteRenderer>();

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
        transform.localScale *= 0.15844f;
        
        StrikeHitBoxSize = new Vector2(1.75f, 1f);
        GrabHitBoxSize = new Vector2(0.5f, 1f);
        
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
            StrikeHitBoxDistance = GrabHitBoxDistance = 1f;
        }
        else if (rotInt == 0)
        {
            direction = -1;
            StrikeHitBoxDistance = GrabHitBoxDistance = -1f;
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
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<AnimationChange>(OnAnimationChange);
        EventManager.Instance.RemoveHandler<TranslatePos>(Translate);
        EventManager.Instance.RemoveHandler<HitBoxActive>(OnHitBoxActive);
    }

    //Allows us to call animator from model -- replace with event system
    public void OnAnimationChange(AnimationChange evt)
    {
        if (evt.PlayerIndex != PlayerIndex)
            return;
        animator.SetTrigger(evt.AnimTrigger);
    }
     
    //Move the player by amount times speed
    public void Translate(TranslatePos evt)
    {
        if (PlayerIndex != evt.PlayerIndex)
            return;
        
        float amount = evt.RawAxis;
        float speed = evt.Speed;
        float oldX = transform.position.x; //Our old position
        
        transform.position += Vector3.right * amount * Time.deltaTime * speed;
        
        //Update our direction and change our rotation if necessary
        if (transform.position.x > oldX && direction == -1)
        {
            transform.Rotate(0f,-180f,0f);
            direction = 1;
            StrikeHitBoxDistance = GrabHitBoxDistance = 1;
        }
        if (transform.position.x < oldX && direction == 1)
        {
            transform.Rotate(0f,180f,0f);
            direction = -1;
            StrikeHitBoxDistance = GrabHitBoxDistance = -1;
        }
    }

    public void OnHitBoxActive(HitBoxActive evt)
    {
        if (evt.PlayerIndex != PlayerIndex)
            return;
        Debug.Log("HitBox Active " + PlayerIndex);
        Vector2 hitBoxCenter = new Vector2(transform.position.x + evt.HitBoxDistance * StrikeHitBoxDistance, 0);
        Collider2D hitCol = Physics2D.OverlapBox(hitBoxCenter, evt.HitBoxSize, 0, opponentLayer);

        if (hitCol)
        {
            EventManager.Instance.Fire(new HitOpponent(PlayerIndex, evt.IsStrike));
        }
    }
    #region Tools
    private void OnDrawGizmos()
    {
        //Draw strike hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + StrikeHitBoxDistance, 0), StrikeHitBoxSize);
        
        //Draw Grab hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + GrabHitBoxDistance, 0), GrabHitBoxSize);
    }
    #endregion
}
