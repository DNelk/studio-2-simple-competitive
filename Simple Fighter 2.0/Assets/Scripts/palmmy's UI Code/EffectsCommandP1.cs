using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EffectsCommandP1 : MonoBehaviour
{
    public Sprite StartUP;
    public Sprite strikeActive;
    public Sprite kickActive;

    public GameObject opponent;
    public Sprite opponentStartUP;
    public Sprite opponentAttack;
    private bool opponentIsActive;
    private int opponentCooldown;

    private bool strikeIsActive;
    private bool kickIsActive;

    private int coolDown;

    private bool counterNormal;
    private int counterNormalFrame;

    private bool counterBreak;
    private int counterBreakFrame;

    public TestHPbar opponentHP;
    
    // Start is called before the first frame update
    void Start()
    {
        strikeIsActive = false;
        kickIsActive = false;
        opponentIsActive = false;
        coolDown = 0;
        opponentCooldown = 0;
        counterNormalFrame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        effectAttack();
        effectGuard();
        effectCounter();
    }

    void effectAttack()
    {
        //Attack
        if (Input.GetKeyDown(KeyCode.D))
        {
            GetComponent<SpriteRenderer>().sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Attacks_Normal"), transform) as GameObject;
            strikeIsActive = true;
        }
        
        //Attack_Break
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<SpriteRenderer>().sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Attacks_Break"), transform) as GameObject;
            strikeIsActive = true;
        }
        
        //Kick
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<SpriteRenderer>().sprite = kickActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Kick_Normal"), transform) as GameObject;
            kickIsActive = true;
        }
        
        //Kick_break
        if (Input.GetKeyDown(KeyCode.V))
        {
            GetComponent<SpriteRenderer>().sprite = kickActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Kick_Break"), transform) as GameObject;
            kickIsActive = true;
        }        

        if (strikeIsActive == true || kickIsActive == true)
            coolDown++;

        if (coolDown == 15)
        {
            strikeIsActive = false;
            kickIsActive = false;
            GetComponent<SpriteRenderer>().sprite = StartUP;
            coolDown = 0;
        }

    }


    void effectGuard()
    {
        //Guard
        if (Input.GetKeyDown(KeyCode.T))
        {
            opponent.GetComponent<SpriteRenderer>().sprite = opponentAttack;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Guard"), transform) as GameObject;
            opponentIsActive = true;
        }

        if (opponentIsActive == true)
            opponentCooldown++;

        if (opponentCooldown == 15)
        {
            opponentIsActive = false;
            opponent.GetComponent<SpriteRenderer>().sprite = opponentStartUP;
            opponentCooldown = 0;
        }
    }

    void effectCounter()
    {
        //counter
        if (Input.GetKeyDown(KeyCode.G))
        {
            opponent.GetComponent<SpriteRenderer>().sprite = opponentAttack;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Guard_Counter"), transform) as GameObject;
            opponentIsActive = true;
            counterNormal = true;
        }
        
        //counter_Break
        if (Input.GetKeyDown(KeyCode.B))
        {
            opponent.GetComponent<SpriteRenderer>().sprite = opponentAttack;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Guard_Counter"), transform) as GameObject;
            opponentIsActive = true;
            counterBreak = true;
        }

        //normal
        if (counterNormal == true)
            counterNormalFrame++;

        if (counterNormalFrame == 10)
        {
            GetComponent<SpriteRenderer>().sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Counter_Normal"), transform) as GameObject;
            strikeIsActive = true;
            counterNormal = false;
            counterNormalFrame = 0;
        }
        
        //break
        if (counterBreak == true)
            counterBreakFrame++;

        if (counterBreakFrame == 10)
        {
            GetComponent<SpriteRenderer>().sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Counter_Break"), transform) as GameObject;
            strikeIsActive = true;
            counterBreak = false;
            counterBreakFrame = 0;
        }
        
        
        //opponent
        if (opponentIsActive == true)
            opponentCooldown++;

        if (opponentCooldown == 15)
        {
            opponentIsActive = false;
            opponent.GetComponent<SpriteRenderer>().sprite = opponentStartUP;
            opponentCooldown = 0;
        }
    }
}
