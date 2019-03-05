using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatus : MonoBehaviour
{
    public PlayerModel Model;
    public Player player;
    public Image[] HPbar; //for HP
    public Image[] StaminaBar; //for Stamina

    private int maxHP;
    private int HPmultiplier;
    private int maxStamina;
    private int staminaMultiplier;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(Model.PlayerIndex); // to get which player is using this bar

        //start with initial HP
        maxHP = Model.Health;
        updateHP(maxHP);

        //start with initial Stamina
        maxStamina = Model.Stamina;
        updateStamina(maxStamina);
        
        //hook into game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    
    
    //HP
    
    #region  UpdateHP
    public void updateHP(float HP) //update HP
    {
        for (float i = 0; i <= HP; i++)
        {
            if (i / 2 == Mathf.CeilToInt(i / 2))
            {
                if (i / 2 != 0)
                    HPbar[Mathf.CeilToInt(i/2) - 1].color = Color.white; //paint white on one bar that is still full
                
                if (i <= maxHP - 1)
                    HPbar[Mathf.CeilToInt(i/2)].color = new Color(0,0,0,0.4f ); //make one bar disappear
            }
            else
            {
                HPbar[Mathf.CeilToInt(i / 2) -1 ].color = Color.red; //paint red on bar with half left
            }
            //Debug.Log(i/2);
        }
    }
    
    #endregion

    #region  getHit

    public void getHit(int Damage)
    {
        Model.Health -= Damage;
        updateHP(Model.Health);

        if (Model.Health <= 0)
        {
            gameManager.RoundEnd(Model.PlayerIndex);
            Model.State = PlayerState.Ko;
            
            print(Model.State);
        }
    }
    
    #endregion
    
    #region HPRecovery

    public void HPRecovery()
    {
        if (Model.Health < maxHP && Model.Health % 2 != 0)
        {
            HPmultiplier++;
            if (HPmultiplier >= 180)
            {
                HPmultiplier = 0;
                Model.Health++;
                updateHP(Model.Health);
                Debug.Log("RecoverHealth");
            }
        }
    }
    
    #endregion
    
    //Stamina
    
    #region UpdateStamina

    public void updateStamina(int stamina)
    {
        for (int i = 0; i <= stamina; i++)
        {
            if (i != 0)
                StaminaBar[i-1].color = new Color(1f, 0.8f, 0f, 1f);
               
            if (i <= maxStamina-1)
                StaminaBar[i].color = new Color(0, 0, 0, 0.4f);
        }
    }
    
    #endregion
    
    #region PerformingAction

    public void performAction(int stamina)
    {
        Model.Stamina -= stamina;
        updateStamina(Model.Stamina);
    }
    
    #endregion
    
    #region StaminaRecovery

    public void StaminaRecovery()
    {
        if (Model.Stamina < maxStamina)
        {
            staminaMultiplier++;

            if (staminaMultiplier == 60) //change this condition to adjust how fast stamina recovery
            {
                staminaMultiplier = 0;
                Model.Stamina++;
                updateStamina(Model.Stamina);
                Debug.Log("RecoverStamina");
            }
        }
    }

    #endregion
}
