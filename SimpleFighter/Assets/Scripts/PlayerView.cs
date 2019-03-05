using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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

            case PlayerState.Idle:
            default:
                spriteRenderer.sprite = sprites["Idle"];
                spriteRenderer.color = Color.white;
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
