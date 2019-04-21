using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EffectsCommand : MonoBehaviour
{
    public Sprite StartUP;
    public Sprite strikeActive;
    public Sprite kickActive;

    private bool strikeIsActive;
    private bool kickIsActive;

    private int coolDown;

    public TestHPbar opponentHP;
    
    // Start is called before the first frame update
    void Start()
    {
        strikeIsActive = false;
        coolDown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        effectAttack();
    }

    void effectAttack()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<SpriteRenderer>().sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Attacks_Break"), transform) as GameObject;
            strikeIsActive = true;
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            GetComponent<SpriteRenderer>().sprite = strikeActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Attacks_Normal"), transform) as GameObject;
            strikeIsActive = true;
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<SpriteRenderer>().sprite = kickActive;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/Kick_Normal"), transform) as GameObject;
            kickIsActive = true;
        }
        
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
}
