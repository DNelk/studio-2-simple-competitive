using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HPbarEffect[] Children = new HPbarEffect[3];
    
    public GameObject[] WinCounter = new GameObject[3];
    
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
                DestroyChild(newHealth);
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
        Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/RecoverText"), transform.parent);
    }
    
    private void DamageChild()
    {
        Children[currentChild].Crack();
    }
    
    private void DestroyChild(int newHealth)
    {
        Children[currentChild].Shatter();
        currentChild++;

        if (newHealth == 4)
            Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/DownOneText"), transform.parent);
        if (newHealth == 2)
            Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player" + (PlayerIndex + 1) + "/DownTwoText"), transform.parent);
            AudioManager.Instance.PlayAudio(AudioManager.Instance.HPShatterAudioClips); // HPShatter Audio
    }
}
