using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*Updates the player's appearances and displays input*/
public class PlayerView : MonoBehaviour
{
    
    #region Public Variables
    public PlayerModel Model;
    public SpriteWithKey[] SpritesWithKeys;
    public BoxCollider2D hitBox;
    #endregion
    
    #region Private Variables
    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> sprites;
    #endregion
    
    private void Start()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        sprites = new Dictionary<string, Sprite>();
        foreach (SpriteWithKey var in SpritesWithKeys)
        {
            sprites.Add(var.key, var.sprite);
        }
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
        transform.position += Vector3.right * amount * Time.deltaTime * speed;
    }
    
    //Roll the player a set distance
    public IEnumerator Roll(float distance, float speed)
    {
        
        yield return null;
    }
    #endregion
    
    #region Private Methods

    private void DrawSprite()
    {
        switch (Model.State)
        {
            case PlayerState.BlockStartup:
                spriteRenderer.sprite = sprites["BlockStartup"];
                break;
            case PlayerState.BlockActive:
                spriteRenderer.sprite = sprites["BlockActive"];
                break;
            case PlayerState.BlockRecovery:
                spriteRenderer.sprite = sprites["BlockRecovery"];
                break;
            case PlayerState.StrikeStartup:
                spriteRenderer.sprite = sprites["StrikeStartup"];
                break;
            case PlayerState.StrikeActive:
                spriteRenderer.sprite = sprites["StrikeActive"];
                break;
            case PlayerState.StrikeRecovery:
                spriteRenderer.sprite = sprites["StrikeRecovery"];
                break;
            case PlayerState.GrabStartup:
                spriteRenderer.sprite = sprites["GrabStartup"];
                break;
            case PlayerState.GrabActive:
                spriteRenderer.sprite = sprites["GrabActive"];
                break;
            case PlayerState.GrabRecovery:
                spriteRenderer.sprite = sprites["GrabRecovery"];
                break;
            case PlayerState.DamageStartup:
                spriteRenderer.sprite = sprites["DamageStartup"];
                break;
            case PlayerState.DamageActive:
                spriteRenderer.sprite = sprites["DamageActive"];
                break;
            case PlayerState.DamageRecovery:
                spriteRenderer.sprite = sprites["DamageRecovery"];
                break;

            case PlayerState.Grounded:
                spriteRenderer.sprite = sprites["Grounded"];
                break;
            case PlayerState.GetupStartup:
                spriteRenderer.sprite = sprites["GetupStartup"];
                break;
            case PlayerState.GetupActive:
                spriteRenderer.sprite = sprites["GetupActive"];
                break;
            case PlayerState.GetupRecovery:
                spriteRenderer.sprite = sprites["GetupRecovery"];
                break;
            case PlayerState.GetupRollStartup:
                spriteRenderer.sprite = sprites["GetupRollStartup"];
                break;
            case PlayerState.GetupRollActive:
                spriteRenderer.sprite = sprites["GetupRollActive"];
                break;
            case PlayerState.GetupRollRecovery:
                spriteRenderer.sprite = sprites["GetupRollRecovery"];
                break;
            case PlayerState.TechInPlaceStartup:
                spriteRenderer.sprite = sprites["TechInPlaceStartup"];
                break;
            case PlayerState.TechInPlaceActive:
                spriteRenderer.sprite = sprites["TechInPlaceActive"];
                break;
            case PlayerState.TechInPlaceRecovery:
                spriteRenderer.sprite = sprites["TechInPlaceRecovery"];
                break;
            case PlayerState.TechRollStartup:
                spriteRenderer.sprite = sprites["TechRollStartup"];
                break;
            case PlayerState.TechRollActive:
                spriteRenderer.sprite = sprites["TechRollActive"];
                break;
            case PlayerState.TechRollRecovery:
                spriteRenderer.sprite = sprites["TechRollRecovery"];
                break;

            case PlayerState.Idle:
            default:
                spriteRenderer.sprite = sprites["Idle"];
                break;;
        }
    }

    #endregion
    
}

//Hack so we can set sprites in editor lol sorry 
[System.Serializable]
public struct SpriteWithKey {
    public string key;
    public Sprite sprite;
}
