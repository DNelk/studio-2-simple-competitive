using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*Updates the player's appearances and displays input*/
public class PlayerView : MonoBehaviour
{
    
    #region Public Variables
    public PlayerModel Model;
    public SpriteWithKey[] SpritesWithKeys;
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
            case PlayerState.BlockRecovery:
            case PlayerState.BlockActive:
                spriteRenderer.sprite = sprites["block"];
                break;
            case PlayerState.StrikeStartup:
            case PlayerState.StrikeRecovery:
            case PlayerState.StrikeActive:
                spriteRenderer.sprite = sprites["strike"];
                break;
            case PlayerState.GrabStartup:
            case PlayerState.GrabRecovery:
            case PlayerState.GrabActive:
                spriteRenderer.sprite = sprites["grab"];
                break;
            case PlayerState.DamageStartup:
            case PlayerState.DamageRecovery:
            case PlayerState.DamageActive:
                spriteRenderer.sprite = sprites["hurt"];
                break;

            case PlayerState.Grounded:
            case PlayerState.GetupStartup:
            case PlayerState.GetupActive:
            case PlayerState.GetupRecovery:
            case PlayerState.GetupRollStartup:
            case PlayerState.GetupRollActive:
            case PlayerState.GetupRollRecovery:
            case PlayerState.TechInPlaceStartup:
            case PlayerState.TechInPlaceActive:
            case PlayerState.TechInPlaceRecovery:
            case PlayerState.TechRollStartup:
            case PlayerState.TechRollActive:
            case PlayerState.TechRollRecovery:

            case PlayerState.Idle:
            default:
                spriteRenderer.sprite = sprites["idle"];
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
