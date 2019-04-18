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
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            GetComponent<Image>().sprite = Active;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Attacks3"), transform) as GameObject;
            Attack.GetComponent<RectTransform>().anchoredPosition = new Vector2(133f,128f);
            isActive = true;
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<Image>().sprite = Active;
            GameObject Attack = Instantiate(Resources.Load("Prefabs/PalmmyEffect/Attacks_Break"), transform) as GameObject;
            Attack.GetComponent<RectTransform>().anchoredPosition = new Vector2(133f,128f);
            isActive = true;
        }
        

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
