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
    public PlayerModel OpponentState;
    
    public GameObject IdleSprite;
    public GameObject StrikingSprite;
    public GameObject GrabbingSprite;
    public GameObject BlockingSprite;
    public GameObject GetHitSprite;

    private string axisName;
    private string keyAxisName;
    private string squareName;
    private string keySquareName;
    private string triangleName;
    private string keyTriangleName;
    private string circleName;
    private string keyCircleName;
    private string hitBoxName;

    public float DelayInSeconds = 0.25f;
    public float SpeedMultiplier = 10;
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance = 0;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance = 0;

    
    //testing the below
    public int frameCount;
    public static class WaitFor
    {
        public static IEnumerator Frames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }
    }
    //testing the above
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (Player.PlayerIndex == 0) //reference Controller 1
        {
            axisName = "Horizontal1";
            keyAxisName = "KeyboardHorizontal1";
            squareName = "Square1";
            keySquareName = "KeySquare1";
            triangleName = "Triangle1";
            keyTriangleName = "KeyTriangle1";
            circleName = "Circle1";
            keyCircleName = "KeyCircle1";
            hitBoxName = "HurtBoxP2";
        }
        else //reference Controller 2
        {
            axisName = "Horizontal2";
            keyAxisName = "KeyboardHorizontal2";
            squareName = "Square2";
            keySquareName = "KeySquare2";
            triangleName = "Triangle2";
            keyTriangleName = "KeyTriangle2";
            circleName = "Circle2";
            keyCircleName = "KeyCircle2";
            hitBoxName = "HurtBoxP1";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Walking)
            MoveHorizontal();
            KeyboardMoveHorizontal();

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
        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Walking) //moving is not possible unless Idle or Walking
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
        
    }

    void KeyboardMoveHorizontal()
    {
        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Walking) //moving is not possible unless Idle or Walking
        {
            if (Input.GetAxisRaw(keyAxisName) != 0)
            {
                Player.transform.position += Vector3.right * Input.GetAxisRaw(keyAxisName) * Time.deltaTime * SpeedMultiplier;
                Player.State = PlayerState.Walking;
            }

            if (Input.GetAxisRaw(keyAxisName) == 0)
            {
                Player.State = PlayerState.Idle;
            }
        }
    }
    #endregion
    
    #region Block Method
    void Block()
    {
        if (Input.GetButtonDown(circleName) || Input.GetButtonDown(keyCircleName))
        {
            Debug.Log("Circle button pressed!");
            //STARTUP
            Player.State = PlayerState.BlockStartup;
            //yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
            
            //ACTIVE
            Player.State = PlayerState.Blocking;
            IdleSprite.SetActive(false);
            BlockingSprite.SetActive(true);
            
        }

        if (Input.GetButtonUp(circleName) || Input.GetButtonDown(keyCircleName))
        {
            //RECOVERY
            Player.State = PlayerState.BlockCooldown;
            //yield return StartCoroutine(WaitFor.Frames(12)); // wait for frames
            
            //FAF
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
            //STARTUP
            Player.State = PlayerState.StrikeStartup;
            IdleSprite.SetActive(false);
            BlockingSprite.SetActive(true); // placeholder for Strike Startup
            //StrikingSprite.SetActive(true);
            yield return StartCoroutine(WaitFor.Frames(5)); // wait for frames

            //ACTIVE
            Player.State = PlayerState.Striking;
            BlockingSprite.SetActive(false); // placeholder for Strike Startup
            StrikingSprite.SetActive(true);
            SpawnHitBox(StrikeHitBoxDistance, StrikeHitBoxSize, "strike box ");
            yield return StartCoroutine(WaitFor.Frames(2)); // wait for frames
            //yield return new WaitForSeconds(DelayInSeconds);
            
            //RECOVERY
            Player.State = PlayerState.StrikeCooldown;
            StrikingSprite.SetActive(false);
            BlockingSprite.SetActive(true); //placeholder for Strike Recovery
            yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
            
            //FAF
            //StrikingSprite.SetActive(false);
            BlockingSprite.SetActive(false); //placeholder for Strike Recovery
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

            //STARTUP
            Player.State = PlayerState.GrabStartup;
            IdleSprite.SetActive(false);
            BlockingSprite.SetActive(true); // placeholder for Grab Startup
            //GrabbingSprite.SetActive(true);
            yield return StartCoroutine(WaitFor.Frames(8)); // wait for frames
            
            //ACTIVE
            Player.State = PlayerState.Grabbing;
            BlockingSprite.SetActive(false); // placeholder for Grab Startup
            GrabbingSprite.SetActive(true);
            SpawnHitBox(GrabHitBoxDistance, GrabHitBoxSize, "grab box ");
                //yield return new WaitForSeconds(DelayInSeconds);
            yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
            
            //RECOVERY
            Player.State = PlayerState.GrabCooldown;
            GrabbingSprite.SetActive(false);
            BlockingSprite.SetActive(true); // placeholder for Grab Recovery
            yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
            
            //FAF
            //GrabbingSprite.SetActive(false);
            BlockingSprite.SetActive(false); // placeholder for Grab Recovery
            IdleSprite.SetActive(true);
            Player.State = PlayerState.Idle;
        }
    }
    #endregion

    #region Get Hit Method
    public IEnumerator GetHit(float DelayInSeconds)
    {
        IdleSprite.SetActive(false);
        GetHitSprite.SetActive(true);
        Player.State = PlayerState.Damage;
        
        //yield return new WaitForSeconds(DelayInSeconds);
        yield return StartCoroutine(WaitFor.Frames(40)); // 40 is an arbitrary number for now
        
        //FAF
        GetHitSprite.SetActive(false);
        IdleSprite.SetActive(true);
        Player.State = PlayerState.Idle;
    }
    #endregion
    
    #region Spawn HitBox Method
    public void SpawnHitBox(float distance, Vector2 size, string boxName)
    {
        Vector2 hitBoxCenter = new Vector2(Player.transform.position.x + distance, 0);
        Collider2D hitCol = Physics2D.OverlapBox(hitBoxCenter,
            size, 0, LayerMask.GetMask(hitBoxName));

        if (hitCol)
        {
            if (Player.State == PlayerState.Striking && OpponentState.State != PlayerState.Blocking)
            {
                Debug.Log(boxName + hitCol.transform.gameObject);
                StartCoroutine(Opponent.GetHit(DelayInSeconds));
            }
            else if (Player.State == PlayerState.Grabbing && OpponentState.State != PlayerState.Striking)
            {
                Debug.Log(boxName + hitCol.transform.gameObject);
                StartCoroutine(Opponent.GetHit(DelayInSeconds));
            }
        }
    }
    #endregion
    
    #region Draw Gizmos Method
    private void OnDrawGizmos()
    {
        //Draw strike hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(Player.transform.position.x + StrikeHitBoxDistance, 0), StrikeHitBoxSize);
        
        //Draw Grab hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(Player.transform.position.x + GrabHitBoxDistance, 0), GrabHitBoxSize);
    }
    #endregion

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
