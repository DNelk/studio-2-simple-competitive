using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Public Variables
    public PlayerModel Model;
    public PlayerModel OpponentModel;
    public PlayerView View;
    #endregion

 /*   #region Sprites
    public GameObject IdleSprite;
    public GameObject StrikingSprite;
    public GameObject GrabbingSprite;
    public GameObject BlockingSprite;
    public GameObject GetHitSprite;
    #endregion
  */
    
    #region Control Keys
    private string axisName;
    private string keyAxisName;
    private string squareName;
    private string keySquareName;
    private string triangleName;
    private string keyTriangleName;
    private string circleName;
    private string keyCircleName;
    private string hitBoxName;
    #endregion
    
    #region Physics
    public float DelayInSeconds = 0.25f;
    public float SpeedMultiplier = 10;
    public Vector2 StrikeHitBoxSize;
    public float StrikeHitBoxDistance = 0;
    public Vector2 GrabHitBoxSize;
    public float GrabHitBoxDistance = 0;
    #endregion
    
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
        if (Model.PlayerIndex == 0) //reference Controller 1
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
        if (Model.State == PlayerState.Idle || Model.State == PlayerState.Walking)
            MoveHorizontal();
            KeyboardMoveHorizontal();

        if (Model.State == PlayerState.Idle || Model.State == PlayerState.Striking || Model.State == PlayerState.Walking)
            StartCoroutine(PlayerAction_Strike(DelayInSeconds));
        
        if(Model.State == PlayerState.Idle || Model.State == PlayerState.Grabbing || Model.State == PlayerState.Walking)
            StartCoroutine(PlayerAction_Grab(DelayInSeconds));
        
        if(Model.State == PlayerState.Idle || Model.State == PlayerState.Blocking || Model.State == PlayerState.Walking)
            PlayerAction_Block();
    }

    #region Horizontal Movement
    void MoveHorizontal()
    {
        if (Model.State == PlayerState.Idle || Model.State == PlayerState.Walking) //moving is not possible unless Idle or Walking
        {
            if (Input.GetAxisRaw(axisName) != 0)
            {
                View.Translate(Input.GetAxisRaw(axisName), SpeedMultiplier);
                Model.State = PlayerState.Walking;
            }

            if (Input.GetAxisRaw(axisName) == 0)
            {
                Model.State = PlayerState.Idle;
            }    
        }
        
    }

    void KeyboardMoveHorizontal()
    {
        if (Model.State == PlayerState.Idle || Model.State == PlayerState.Walking) //moving is not possible unless Idle or Walking
        {
            if (Input.GetAxisRaw(keyAxisName) != 0)
            {
                View.Translate(Input.GetAxisRaw(keyAxisName), SpeedMultiplier);
                Model.State = PlayerState.Walking;
            }

            if (Input.GetAxisRaw(keyAxisName) == 0)
            {
                Model.State = PlayerState.Idle;
            }
        }
    }
    #endregion
    
    #region Block Method
    void PlayerAction_Block()
    {
        if (Input.GetButtonDown(circleName) || Input.GetButtonDown(keyCircleName))
        {
            Debug.Log("Circle button pressed!");
            //STARTUP
            Model.State = PlayerState.BlockStartup;
            //yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
            
            //ACTIVE
            Model.State = PlayerState.Blocking;          
        }

        if (Input.GetButtonUp(circleName) || Input.GetButtonDown(keyCircleName))
        {
            //RECOVERY
            Model.State = PlayerState.BlockCooldown;
            //yield return StartCoroutine(WaitFor.Frames(12)); // wait for frames
            
            //FAF
            Model.State = PlayerState.Idle;
            
        }
    }
    #endregion

    #region Strike Method
    public IEnumerator PlayerAction_Strike(float DelayInSeconds)
    {
        if (Input.GetButtonDown(squareName))
        {
            Debug.Log("Square button pressed!");
            //STARTUP
            Model.State = PlayerState.StrikeStartup;
            yield return StartCoroutine(WaitFor.Frames(5)); // wait for frames

            //ACTIVE
            Model.State = PlayerState.Striking;
            SpawnHitBox(StrikeHitBoxDistance, StrikeHitBoxSize, "strike box ");
            yield return StartCoroutine(WaitFor.Frames(2)); // wait for frames
            //yield return new WaitForSeconds(DelayInSeconds);
            
            //RECOVERY
            Model.State = PlayerState.StrikeCooldown;
            yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
            
            //FAF
            Model.State = PlayerState.Idle;
        }
    }
    #endregion

    #region Grab Method
    public IEnumerator PlayerAction_Grab(float DelayInSeconds)
    {
        if (Input.GetButtonDown(triangleName))
        {
            Debug.Log("Triangle button pressed!");

            //STARTUP
            Model.State = PlayerState.GrabStartup;
            yield return StartCoroutine(WaitFor.Frames(8)); // wait for frames
            
            //ACTIVE
            Model.State = PlayerState.Grabbing;
            SpawnHitBox(GrabHitBoxDistance, GrabHitBoxSize, "grab box ");
            yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
            
            //RECOVERY
            Model.State = PlayerState.GrabCooldown;
            yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
            
            //FAF
            Model.State = PlayerState.Idle;
        }
    }
    #endregion
    
    #region Spawn HitBox Method
    public void SpawnHitBox(float distance, Vector2 size, string boxName)
    {
        Vector2 hitBoxCenter = new Vector2(View.transform.position.x + distance, 0);
        Collider2D hitCol = Physics2D.OverlapBox(hitBoxCenter,
            size, 0, LayerMask.GetMask(hitBoxName));

        if (hitCol)
        {
            if (Model.State == PlayerState.Striking && OpponentModel.State != PlayerState.Blocking)
            {
                Debug.Log(boxName + hitCol.transform.gameObject);
                StartCoroutine(OpponentModel.PlayerAction_GetHit(DelayInSeconds));
            }
            else if (Model.State == PlayerState.Grabbing && OpponentModel.State != PlayerState.Striking)
            {
                Debug.Log(boxName + hitCol.transform.gameObject);
                StartCoroutine(OpponentModel.PlayerAction_GetHit(DelayInSeconds));
            }
        }
    }
    #endregion
    
    #region Draw Gizmos Method
    private void OnDrawGizmos()
    {
        //Draw strike hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(View.transform.position.x + StrikeHitBoxDistance, 0), StrikeHitBoxSize);
        
        //Draw Grab hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(View.transform.position.x + GrabHitBoxDistance, 0), GrabHitBoxSize);
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
            Model.State = PlayerState.Striking;
        }

        if (Input.GetButtonUp(squareName))
        {
            IdleSprite.SetActive(true);
            StrikingSprite.SetActive(false);
            Model.State = PlayerState.Idle;
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
            Model.State = PlayerState.Grabbing;
            
        }

        if (Input.GetButtonUp(triangleName))
        {
            IdleSprite.SetActive(true);
            GrabbingSprite.SetActive(false);
            Model.State = PlayerState.Idle;
        }
    }*/
    #endregion
}
