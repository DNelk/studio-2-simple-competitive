using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    #region Private Variables
    
    private Animator animator;
    private SpriteRenderer spriteRen;
    private Rigidbody2D rb;
    private BoxCollider2D col;
        
    #endregion
    
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance;
    
    private void Awake()
    {
        //Create Components
        animator = gameObject.AddComponent<Animator>();
        

        spriteRen = gameObject.AddComponent<SpriteRenderer>();

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        col = gameObject.AddComponent<BoxCollider2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale *= 0.15844f;
        
        StrikeHitBoxSize = new Vector2(1.75f, 1f);
        StrikeHitBoxDistance = 1f;
        GrabHitBoxSize = new Vector2(0.5f, 1f);
        GrabHitBoxDistance = 1f;
        
        //Assign sprite and animator
        spriteRen.sprite = Resources.Load<Sprite>("Textures/NormalStance");
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Player");
        
        col.offset = new Vector2(1.155829f, -2.894697f);
        col.size = new Vector2(5.765483f, 20.96054f);
    }

    // Update is called once per frame
    void Update()
    {
       animator.SetTrigger("isStriking");
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
