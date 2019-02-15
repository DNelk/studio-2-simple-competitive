using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerModel Player;
    public GameObject StandingSprite;
    public GameObject StrikingSprite;
    public GameObject GrabbingSprite;
    public GameObject BlockingSprite;

    private string axisName;
    private string squareName;
    private string triangleName;
    private string circleName;
        

    public float SpeedMultiplier = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Player.PlayerIndex == 0)
        {
            axisName = "Horizontal1";
            squareName = "Square1";
            triangleName = "Triangle1";
            circleName = "Circle1";
        }
        else
        {
            axisName = "Horizontal2";
            squareName = "Square2";
            triangleName = "Triangle2";
            circleName = "Circle2";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Walking)
            MoveHorizontal();

        if (Player.State == PlayerState.Idle || Player.State == PlayerState.Striking || Player.State == PlayerState.Walking)
            Strike();
        
        if(Player.State == PlayerState.Idle || Player.State == PlayerState.Grabbing || Player.State == PlayerState.Walking)
            Grab();
        
        if(Player.State == PlayerState.Idle || Player.State == PlayerState.Blocking || Player.State == PlayerState.Walking)
            Block();
    }

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

    void Strike()
    {
        if (Input.GetButtonDown(squareName))
        {
            Debug.Log("Square button pressed!");
            StandingSprite.SetActive(false);
            StrikingSprite.SetActive(true);
            Player.State = PlayerState.Striking;
        }

        if (Input.GetButtonUp(squareName))
        {
            StandingSprite.SetActive(true);
            StrikingSprite.SetActive(false);
            Player.State = PlayerState.Idle;
        }
    }

    void Grab()
    {
        if (Input.GetButtonDown(triangleName))
        {
            Debug.Log("Triangle button pressed!");
            StandingSprite.SetActive(false);
            GrabbingSprite.SetActive(true);
            Player.State = PlayerState.Grabbing;
        }

        if (Input.GetButtonUp(triangleName))
        {
            StandingSprite.SetActive(true);
            GrabbingSprite.SetActive(false);
            Player.State = PlayerState.Idle;
        }
    }

    void Block()
    {
        if (Input.GetButtonDown(circleName))
        {
            Debug.Log("Circle button pressed!");
            StandingSprite.SetActive(false);
            BlockingSprite.SetActive(true);
            Player.State = PlayerState.Blocking;
        }

        if (Input.GetButtonUp(circleName))
        {
            StandingSprite.SetActive(true);
            BlockingSprite.SetActive(false);
            Player.State = PlayerState.Idle;
        }
    }
}
