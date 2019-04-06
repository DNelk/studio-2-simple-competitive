using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject[] Children = new GameObject[3];

    public int PlayerIndex;
    
    private int currentChild;
    
    private int cachedHealth;
    
  
    private void Start()
    {
        currentChild = 0;
    }

    public void UpdateHealth(int newHealth)
    {
        
        if (newHealth % 2 == 0) //If our health is even we either just went down a tier or went back up from pending damage
        {
            if (newHealth > cachedHealth) //Rebuild the bar, damage not pending
            {
                RebuildChild();
            }
            else //Destroy the bar
            {
                DestroyChild();
            }
        }
        else//If our new health is odd we are in a "pending damage state"
        {
            DamageChild();
        }

        cachedHealth = newHealth;
    }

    private void RebuildChild()
    {
        //Right now we just change the sprite.. probably not final
        Image childImage = Children[currentChild].GetComponent<Image>();
        childImage.sprite = Resources.Load<Sprite>("Textures/HPBar/p" + (PlayerIndex + 1) + "full");
    }
    
    private void DamageChild()
    {
        //Right now we just change the sprite.. probably not final
        Image childImage = Children[currentChild].GetComponent<Image>();
        Debug.Log("Textures/HPBar/p" + (PlayerIndex + 1) + "hurt");
        childImage.sprite = Resources.Load<Sprite>("Textures/HPBar/p" + (PlayerIndex + 1) + "hurt");
    }
    
    private void DestroyChild()
    {
        //Right now we just change the sprite.. probably not final
        Image childImage = Children[currentChild].GetComponent<Image>();
        childImage.sprite = Resources.Load<Sprite>("Textures/HPBar/p" + (PlayerIndex + 1) + "empty");
        currentChild++;
    }
}
