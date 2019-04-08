using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*Updates the player's appearances and displays input*/
public class PlayerView : MonoBehaviour
{
    
    #region Public Variables
    public PlayerModel Model;
    public SpriteWithKey[] SpritesWithKeys;
    public Animator animator;
    public BoxCollider2D hitBox;
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance = 0;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance = 0;
    #endregion
    
    #region Private Variables
    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> sprites;
    private int direction; 
    #endregion
    
    private void Start()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        sprites = new Dictionary<string, Sprite>();
        foreach (SpriteWithKey var in SpritesWithKeys)
        {
            sprites.Add(var.key, var.sprite);
        }

        int rotInt = Mathf.RoundToInt(transform.rotation.eulerAngles.y);
        if (rotInt == 180)
            direction = -1;
        else if (rotInt == 0)
            direction = 1;
        else
            Debug.Log("error: direction not found");
        
        animator.enabled = false; //use this for now
    }

    // Update is called once per frame
    void Update()
    {
        DrawSprite();
    }

    #region Public Methods
    //Move the player by amount times speed
    public void Translate(float amount, float speed)
    {
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
    #endregion
    
    #region Private Methods

    private void DrawSprite()
    {
        switch (Model.State)
        {
            case PlayerState.BlockStartup:
                spriteRenderer.sprite = sprites["BlockStartup"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.BlockActive:
                spriteRenderer.sprite = sprites["BlockActive"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.BlockRecovery:
                spriteRenderer.sprite = sprites["BlockRecovery"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.StrikeStartup:
                spriteRenderer.sprite = sprites["StrikeStartup"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.StrikeActive:
                spriteRenderer.sprite = sprites["StrikeActive"];
                spriteRenderer.color = Color.red;
                break;
            case PlayerState.StrikeRecovery:
                spriteRenderer.sprite = sprites["StrikeRecovery"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.GrabStartup:
                spriteRenderer.sprite = sprites["GrabStartup"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.GrabActive:
                spriteRenderer.sprite = sprites["GrabActive"];
                spriteRenderer.color = Color.red;
                break;
            case PlayerState.GrabRecovery:
                spriteRenderer.sprite = sprites["GrabRecovery"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.DamageStartup:
                spriteRenderer.sprite = sprites["DamageStartup"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.DamageActive:
                spriteRenderer.sprite = sprites["DamageActive"];
                spriteRenderer.color = Color.black;
                break;
            case PlayerState.DamageRecovery:
                spriteRenderer.sprite = sprites["DamageRecovery"];
                spriteRenderer.color = Color.yellow;
                break;

            case PlayerState.Grounded:
                spriteRenderer.sprite = sprites["Grounded"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.GetupStartup:
                spriteRenderer.sprite = sprites["GetupStartup"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.GetupActive:
                spriteRenderer.sprite = sprites["GetupActive"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.GetupRecovery:
                spriteRenderer.sprite = sprites["GetupRecovery"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.GetupRollStartup:
                spriteRenderer.sprite = sprites["GetupRollStartup"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.GetupRollActive:
                spriteRenderer.sprite = sprites["GetupRollActive"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.GetupRollRecovery:
                spriteRenderer.sprite = sprites["GetupRollRecovery"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.TechInPlaceStartup:
                spriteRenderer.sprite = sprites["TechInPlaceStartup"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.TechInPlaceActive:
                spriteRenderer.sprite = sprites["TechInPlaceActive"];
                spriteRenderer.color = Color.blue;
                break;
            case PlayerState.TechInPlaceRecovery:
                spriteRenderer.sprite = sprites["TechInPlaceRecovery"];
                spriteRenderer.color = Color.yellow;
                break;
            case PlayerState.TechRollStartup:
                spriteRenderer.sprite = sprites["TechRollStartup"];
                spriteRenderer.color = Color.black;
                break;
            case PlayerState.TechRollActive:
                spriteRenderer.sprite = sprites["TechRollActive"];
                spriteRenderer.color = Color.black;
                break;
            case PlayerState.TechRollRecovery:
                spriteRenderer.sprite = sprites["TechRollRecovery"];
                spriteRenderer.color = Color.yellow;
                break;

            case PlayerState.Ko:
                spriteRenderer.sprite = sprites["KO"];
				spriteRenderer.color = Color.white;
                break;
            case PlayerState.Win:
                animator.enabled = true;
                animator.SetBool("isWinning", true);
                break;

            case PlayerState.Idle:
            default:
                spriteRenderer.sprite = sprites["Idle"];
                spriteRenderer.color = Color.white;
                break;
        }
    }

    #endregion
    
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

//Hack so we can set sprites in editor lol sorry 
[System.Serializable]
public struct SpriteWithKey {
    public string key;
    public Sprite sprite;
}
