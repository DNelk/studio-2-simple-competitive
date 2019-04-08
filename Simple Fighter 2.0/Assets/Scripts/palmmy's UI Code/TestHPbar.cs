using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TestHPbar : MonoBehaviour
{
    public KeyCode damagingKey;//set what key press to deal damage to this character
    public float characterHP = 6;//set currentHP for character
    public float maxHP = 6; // set maxHp for character
    public HPbarEffect[] HPbar; //assign bars to this character

    public int recoveryTime = 240;

    // Update is called once per frame
    void Update()
    {
        pressToDamage();
        recoverHP();
        updateHP();
    }

    void pressToDamage() //when pressing damageingKey, Hp get reduced
    {
        if (Input.GetKeyDown(damagingKey) && characterHP > 0)
            characterHP--;
    }

    void recoverHP()
    {
        if (characterHP / 2 != Mathf.CeilToInt(characterHP / 2))
        {
            if (recoveryTime > 0)
                recoveryTime--;     
            else
            {
                characterHP++;
                recoveryTime = 240;
            }
        }
        else
        {
            recoveryTime = 240;
        }
    } //recover 1 HP when 240 frame passed

    void updateHP()
    {
        if (characterHP / 2 == Mathf.Ceil(characterHP / 2)) //if HP is an even number
        {
            if (characterHP != 0) //if HP is not 0, make that bar full
                HPbar[Mathf.CeilToInt(characterHP / 2) - 1].Recover();
            
            if (characterHP <= maxHP - 2) //if HP is not full, make the higher bar shattered
                HPbar[Mathf.CeilToInt(characterHP / 2)].Shatter();
        }
        else //if HP is an odd number, make that bar cracked
        {
            HPbar[Mathf.CeilToInt(characterHP / 2) - 1].Crack();
        }
    }
}
