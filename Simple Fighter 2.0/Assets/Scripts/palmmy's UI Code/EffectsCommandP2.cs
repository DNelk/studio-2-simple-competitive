﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EffectsCommandP2 : MonoBehaviour
{
    private SpriteRenderer mySprite;
    public Sprite StartUP;
    public Sprite strikeActive;
    public Sprite kickActive;

    public SpriteRenderer opponent;
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
        mySprite = GetComponent<SpriteRenderer>();
        
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
        effectTechRp();
    }

    void effectAttack()
    {
        //Attack
        if (Input.GetKeyDown(KeyCode.K))
        {
            mySprite.sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Attacks_Normal"), transform) as GameObject;
            strikeIsActive = true;
        }
        
        //Attack_Break
        if (Input.GetKeyDown(KeyCode.M))
        {
            mySprite.sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Attacks_Break"), transform) as GameObject;
            strikeIsActive = true;
        }
        
        //Kick
        if (Input.GetKeyDown(KeyCode.J))
        {
            mySprite.sprite = kickActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Kick_Normal"), transform) as GameObject;
            kickIsActive = true;
        }
        
        //Kick_break
        if (Input.GetKeyDown(KeyCode.N))
        {
            mySprite.sprite = kickActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Kick_Break"), transform) as GameObject;
            kickIsActive = true;
        }        

        if (strikeIsActive == true || kickIsActive == true)
            coolDown++;

        if (coolDown == 15)
        {
            strikeIsActive = false;
            kickIsActive = false;
            mySprite.sprite = StartUP;
            coolDown = 0;
        }

    }


    void effectGuard()
    {
        //Guard
        if (Input.GetKeyDown(KeyCode.I))
        {
            opponent.sprite = opponentAttack;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Guard"), transform) as GameObject;
            opponentIsActive = true;
        }

        if (opponentIsActive == true)
            opponentCooldown++;

        if (opponentCooldown == 15)
        {
            opponentIsActive = false;
            opponent.sprite = opponentStartUP;
            opponentCooldown = 0;
        }
    }

    void effectCounter()
    {
        //counter
        if (Input.GetKeyDown(KeyCode.U))
        {
            opponent.sprite = opponentAttack;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Guard_Counter"), transform) as GameObject;
            opponentIsActive = true;
            counterNormal = true;
        }
        
        //counter_Break
        if (Input.GetKeyDown(KeyCode.H))
        {
            opponent.sprite = opponentAttack;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Guard_Counter"), transform) as GameObject;
            opponentIsActive = true;
            counterBreak = true;
        }

        //normal
        if (counterNormal == true)
            counterNormalFrame++;

        if (counterNormalFrame == 10)
        {
            mySprite.sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Counter_Normal"), transform) as GameObject;
            strikeIsActive = true;
            counterNormal = false;
            counterNormalFrame = 0;
        }
        
        //break
        if (counterBreak == true)
            counterBreakFrame++;

        if (counterBreakFrame == 10)
        {
            mySprite.sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/Counter_Break"), transform) as GameObject;
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
            opponent.sprite = opponentStartUP;
            opponentCooldown = 0;
        }
    }
    
    private void effectTechRp()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameObject TechRoll = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/TechRoll"), transform) as GameObject;
        }
    }
}
