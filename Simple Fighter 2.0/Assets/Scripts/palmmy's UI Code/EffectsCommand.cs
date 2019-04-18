using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsCommand : MonoBehaviour
{
    public Sprite StartUP;
    public Sprite Active;

    private bool isActive;

    private int coolDown;

    public TestHPbar opponentHP;
    
    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
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
            GetComponent<Image>().sprite = Active;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Attacks1"), transform) as GameObject;
            Attack.GetComponent<RectTransform>().anchoredPosition = new Vector2(133f,128f);
            isActive = true;
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            GetComponent<Image>().sprite = Active;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Attacks2"), transform) as GameObject;
            Attack.GetComponent<RectTransform>().anchoredPosition = new Vector2(133f,128f);
            isActive = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GetComponent<Image>().sprite = Active;
            string attackPrefab;
            if (opponentHP.characterHP / 2 == Mathf.Ceil(opponentHP.characterHP / 2))
            {
                attackPrefab = "Prefabs/PalmmyEffect/Attacks_Break";
            }
            else
            {
                attackPrefab = "Prefabs/PalmmyEffect/Attacks3";
            }
            
            GameObject Attack = Instantiate(Resources.Load(attackPrefab), transform) as GameObject;
            Attack.GetComponent<RectTransform>().anchoredPosition = new Vector2(190f,128f);
            isActive = true;
        }
        
/*
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<Image>().sprite = Active;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Attacks_Break"), transform) as GameObject;
            Attack.GetComponent<RectTransform>().anchoredPosition = new Vector2(190f,128f);
            isActive = true;
        }
*/
        

        if (isActive == true)
            coolDown++;

        if (coolDown == 15)
        {
            isActive = false;
            GetComponent<Image>().sprite = StartUP;
            coolDown = 0;
        }

    }
}
