using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerModel Player;
    public PlayerController Opponent;
    
    public GameObject IdleSprite;
    public GameObject StrikingSprite;
    public GameObject GrabbingSprite;
    public GameObject BlockingSprite;
    public GameObject GetHitSprite;

    private string axisName;
    private string squareName;
    private string triangleName;
    private string circleName;
    private string hitBoxName;

    public float DelayInSeconds = 0.25f;
    public float SpeedMultiplier = 10;
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance = 0;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Player.PlayerIndex == 0) //reference Controller 1
        {
            axisName = "Horizontal1";
            squareName = "Square1";
            triangleName = "Triangle1";
            circleName = "Circle1";
            hitBoxName = "HurtBoxP2";
        }
        else //reference Controller 2
        {
            axisName = "Horizontal2";
            squareName = "Square2";
            triangleName = "Triangle2";
            circleName = "Circle2";
            hitBoxName = "HurtBoxP1";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Walking)
            MoveHorizontal();

        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Striking || Player.State == PlayerState.Walking)
            StartCoroutine(Strike(DelayInSeconds));
        
        if(Player.State == PlayerState.Idle || Player.State == PlayerState.Grabbing || Player.State == PlayerState.Walking)
            StartCoroutine(Grab(DelayInSeconds));
        
        if(Player.State == PlayerState.Idle || Player.State == PlayerState.Blocking || Player.State == PlayerState.Walking)
            Block();
    }

    #region Horizontal Movement
    void MoveHorizontal()
    {
        if (Input.GetAxisRaw(axisName) != 0)
        {
            Player.transform.position += Vector3.right * Input.GetAxisRaw(axisName) * Time.deltaTime * SpeedMultiplier;
            Player.State = PlayerState.Walking;
        }

        if (Input.GetAxisRaw(axisName) == 0)
        {
            Player.State = PlayerState.Idle;
        }
    }
    #endregion
    
    #region Block Method
    void Block()
    {
        if (Input.GetButtonDown(circleName))
        {
            Debug.Log("Circle button pressed!");
            IdleSprite.SetActive(false);
            BlockingSprite.SetActive(true);
            Player.State = PlayerState.Blocking;
        }

        if (Input.GetButtonUp(circleName))
        {
            IdleSprite.SetActive(true);
            BlockingSprite.SetActive(false);
            Player.State = PlayerState.Idle;
        }
    }
    #endregion

    #region Strike Method
    public IEnumerator Strike(float DelayInSeconds)
    {
        if (Input.GetButtonDown(squareName))
        {
            Debug.Log("Square button pressed!");
            IdleSprite.SetActive(false);
            StrikingSprite.SetActive(true);
            Player.State = PlayerState.Striking;

            SpawnHitBox(StrikeHitBoxDistance, StrikeHitBoxSize, "strike box ");
            
            yield return new WaitForSeconds(DelayInSeconds);
            
            StrikingSprite.SetActive(false);
            IdleSprite.SetActive(true);
            Player.State = PlayerState.Idle;
        }
    }
    #endregion

    #region Grab Method
    public IEnumerator Grab(float DelayInSeconds)
    {
        if (Input.GetButtonDown(triangleName))
        {
            Debug.Log("Triangle button pressed!");
            IdleSprite.SetActive(false);
            GrabbingSprite.SetActive(true);
            Player.State = PlayerState.Grabbing;
            
            SpawnHitBox(GrabHitBoxDistance, GrabHitBoxSize, "grab box ");
            
            yield return new WaitForSeconds(DelayInSeconds);
            
            GrabbingSprite.SetActive(false);
            IdleSprite.SetActive(true);
            Player.State = PlayerState.Idle;
        }
    }
    #endregion

    public IEnumerator GetHit(float DelayInSeconds)
    {
        IdleSprite.SetActive(false);
        GetHitSprite.SetActive(true);
        Player.State = PlayerState.Damage;
        
        yield return new WaitForSeconds(DelayInSeconds);
        
        GetHitSprite.SetActive(false);
        IdleSprite.SetActive(true);
        Player.State = PlayerState.Idle;
    }
    
    public void SpawnHitBox(float distance, Vector2 size, string boxName)
    {
        Vector2 hitBoxCenter = new Vector2(Player.transform.position.x + distance, 0);
        Collider2D hitCol = Physics2D.OverlapBox(hitBoxCenter,
            size, 0, LayerMask.GetMask(hitBoxName));

        if (hitCol)
        {
            Debug.Log(boxName + hitCol.transform.gameObject);
            Opponent.StartCoroutine(GetHit(DelayInSeconds));
        }
    }
    
    private void OnDrawGizmos()
    {
        //Draw strike hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(Player.transform.position.x + StrikeHitBoxDistance, 0), StrikeHitBoxSize);
        
        //Draw Grab hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(Player.transform.position.x + GrabHitBoxDistance, 0), GrabHitBoxSize);
    }

    #region Old Strike Method
    /*void Strike()
    {
        if (Input.GetButtonDown(squareName))
        {
            Debug.Log("Square button pressed!");
            IdleSprite.SetActive(false);
            StrikingSprite.SetActive(true);
            Player.State = PlayerState.Striking;
        }

        if (Input.GetButtonUp(squareName))
        {
            IdleSprite.SetActive(true);
            StrikingSprite.SetActive(false);
            Player.State = PlayerState.Idle;
        }
    }*/
    #endregion
    
    #region Old Grab Method
    /*void Grab()
    {
        if (Input.GetButtonDown(triangleName))
        {
            Debug.Log("Triangle button pressed!");
            IdleSprite.SetActive(false);
            GrabbingSprite.SetActive(true);
            Player.State = PlayerState.Grabbing;
            
        }

        if (Input.GetButtonUp(triangleName))
        {
            IdleSprite.SetActive(true);
            GrabbingSprite.SetActive(false);
            Player.State = PlayerState.Idle;
        }
    }*/
    #endregion
}
