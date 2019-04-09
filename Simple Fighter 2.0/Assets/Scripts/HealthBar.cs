using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HPbarEffect[] Children = new HPbarEffect[3];

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
        Children[currentChild].Recover();
    }
    
    private void DamageChild()
    {
        Children[currentChild].Crack();
    }
    
    private void DestroyChild()
    {
        Children[currentChild].Shatter();
        currentChild++;
    }
}
