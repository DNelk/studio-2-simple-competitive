using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerController : MonoBehaviour
{
    #region Public Variables
    public PlayerModel Model;
    public PlayerModel OpponentModel;
    public PlayerView View;
    public Player Player;
    #endregion

 /*   #region Sprites
    public GameObject IdleSprite;
    public GameObject StrikeActiveSprite;
    public GameObject GrabActiveSprite;
    public GameObject BlockActiveSprite;
    public GameObject GetHitSprite;
    #endregion
  */
    
    #region Control Keys
    /*private string axisName;
    private string keyAxisName;
    private string squareName;
    private string keySquareName;
    private string triangleName;
    private string keyTriangleName;
    private string circleName;
    private string keyCircleName;*/
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
        Player = ReInput.players.GetPlayer(Model.PlayerIndex);
        
        if (Model.PlayerIndex == 0) //reference Controller 1
        {
            /*axisName = "Horizontal1";
            keyAxisName = "KeyboardHorizontal1";
            squareName = "Square1";
            keySquareName = "KeySquare1";
            triangleName = "Triangle1";
            keyTriangleName = "KeyTriangle1";
            circleName = "Circle1";
            keyCircleName = "KeyCircle1";*/
            hitBoxName = "HurtBoxP2";
        }
        else //reference Controller 2
        {
            /*axisName = "Horizontal2";
            keyAxisName = "KeyboardHorizontal2";
            squareName = "Square2";
            keySquareName = "KeySquare2";
            triangleName = "Triangle2";
            keyTriangleName = "KeyTriangle2";
            circleName = "Circle2";
            keyCircleName = "KeyCircle2";*/
            hitBoxName = "HurtBoxP1";
        }
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Model.State == PlayerState.Idle || Model.State == PlayerState.Walking)
            MoveHorizontal();
            //KeyboardMoveHorizontal();

        if (Model.State == PlayerState.Idle || Model.State == PlayerState.StrikeStartup || Model.State == PlayerState.StrikeActive || Model.State == PlayerState.StrikeRecovery || Model.State == PlayerState.Walking)
            StartCoroutine(PlayerAction_Strike(DelayInSeconds));
        
        if(Model.State == PlayerState.Idle || Model.State == PlayerState.GrabStartup || Model.State == PlayerState.GrabActive || Model.State == PlayerState.GrabRecovery || Model.State == PlayerState.Walking)
            StartCoroutine(PlayerAction_Grab(DelayInSeconds));
        
        if(Model.State == PlayerState.Idle || Model.State == PlayerState.BlockStartup || Model.State == PlayerState.BlockActive || Model.State == PlayerState.BlockRecovery || Model.State == PlayerState.Walking)
            PlayerAction_Block();*/

        switch (Model.State)
        {
            case PlayerState.Idle: //can perform any action from idle
                MoveCheck();
                StrikeCheck();
                GrabCheck();
                BlockCheck();
                break;
            case PlayerState.Walking: //can perform any action while walking
                MoveCheck();
                StrikeCheck();
                GrabCheck();
                BlockCheck();
                break;
            case PlayerState.StrikeStartup: //can't do anything while punching
            case PlayerState.StrikeActive:
            case PlayerState.StrikeRecovery:
                break;
            case PlayerState.BlockStartup: //can release blocking while blocking
            case PlayerState.BlockActive:
            case PlayerState.BlockRecovery:
                BlockCheck();
                break;
            case PlayerState.GrabStartup: //can't do anything while grabbing
            case PlayerState.GrabActive:
            case PlayerState.GrabRecovery:
                break;
            case PlayerState.Grounded: //currently can't do anything while grounded
                break;
        }
    }

    #region Horizontal Movement

    //Check for inputs during states where the player can move
    void MoveCheck()
    {
        if (Player.GetAxisRaw("Horizontal Movement") != 0)
        {
            MoveHorizontal();
        }

        if (Player.GetAxisRaw("Horizontal Movement") == 0)
        {
            StopMoving();
        }
    }

    //Move the player
    void MoveHorizontal()
    {
        View.Translate(Player.GetAxis("Horizontal Movement"), SpeedMultiplier);
        Model.State = PlayerState.Walking;
    }

    //Stop moving the player and reset to Idle state
    void StopMoving()
    {
        Model.State = PlayerState.Idle;
    }

    /*void KeyboardMoveHorizontal()
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
    }*/
    #endregion
    
    #region Block Method

    //Check for block input during viable states
    void BlockCheck()
    {
        if (Player.GetButtonDown("Block"))
        {
            PlayerAction_Block();
        }

        if (Player.GetButtonUp("Block"))
        {
            PlayerAction_ReleaseBlock();
        }
    }
    
    //Startup block animation
    //Currently the startup frames don't work
    void PlayerAction_Block()
    {
        Debug.Log("Circle button pressed!");
        //STARTUP
        Model.State = PlayerState.BlockStartup;
        //yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
        //ACTIVE
        Model.State = PlayerState.BlockActive; 
    }

    //Go into block recovery and back to idle
    //Currently the revovery frames don't work
    void PlayerAction_ReleaseBlock()
    {
        //RECOVERY
        Model.State = PlayerState.BlockRecovery;
        //yield return StartCoroutine(WaitFor.Frames(12)); // wait for frames
            
        //FAF
        Model.State = PlayerState.Idle;
    }
    #endregion

    #region Strike Method

    public void StrikeCheck()
    {
        if (Player.GetButtonDown("Strike"))
        {
            //STARTUP STRIKE
            Model.State = PlayerState.StrikeStartup;
            StartCoroutine(PlayerAction_Strike(DelayInSeconds));
        }
    }
    
    public IEnumerator PlayerAction_Strike(float DelayInSeconds)
    {
        Debug.Log("Square button pressed!");
        yield return StartCoroutine(WaitFor.Frames(5)); // wait
        //ACTIVE
        Model.State = PlayerState.StrikeActive;
        SpawnHitBox(StrikeHitBoxDistance, StrikeHitBoxSize, "strike box ");
        yield return StartCoroutine(WaitFor.Frames(2)); // wait for frames
        //yield return new WaitForSeconds(DelayInSeconds);
        //RECOVERY
        Model.State = PlayerState.StrikeRecovery;
        yield return StartCoroutine(WaitFor.Frames(16)); // wait for frames
        //FAF
        Model.State = PlayerState.Idle;
    }
    #endregion

    #region Grab Method

    void GrabCheck()
    {
        if (Player.GetButtonDown("Grab"))
        {
            StartCoroutine(PlayerAction_Grab(DelayInSeconds));
        }
    }
    
    public IEnumerator PlayerAction_Grab(float DelayInSeconds)
    {
        Debug.Log("Triangle button press");
        //STARTUP
        Model.State = PlayerState.GrabStartup;
        yield return StartCoroutine(WaitFor.Frames(8)); // wait for frames
        
        //ACTIVE
        Model.State = PlayerState.GrabActive;
        SpawnHitBox(GrabHitBoxDistance, GrabHitBoxSize, "grab box ");
        yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
        //RECOVERY
        Model.State = PlayerState.GrabRecovery;
        yield return StartCoroutine(WaitFor.Frames(6)); // wait for frames
        
        //FAF
        Model.State = PlayerState.Idle;
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
            if (Model.State == PlayerState.StrikeActive && OpponentModel.State != PlayerState.BlockActive)
            {
                Debug.Log(boxName + hitCol.transform.gameObject);
                StartCoroutine(OpponentModel.PlayerAction_GetHit(DelayInSeconds));
            }
            else if (Model.State == PlayerState.GrabActive && OpponentModel.State != PlayerState.StrikeActive)
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
            StrikeActiveSprite.SetActive(true);
            Model.State = PlayerState.StrikeActive;
        }

        if (Input.GetButtonUp(squareName))
        {
            IdleSprite.SetActive(true);
            StrikeActiveSprite.SetActive(false);
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
            GrabActiveSprite.SetActive(true);
            Model.State = PlayerState.GrabActive;
            
        }

        if (Input.GetButtonUp(triangleName))
        {
            IdleSprite.SetActive(true);
            GrabActiveSprite.SetActive(false);
            Model.State = PlayerState.Idle;
        }
    }*/
    #endregion
}
