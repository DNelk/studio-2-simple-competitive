using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class HPModel : MonoBehaviour
{

    public PlayerModel Model;
    public Player player;
    public Image[] HPbar;

    private int maxHP;
    
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(Model.PlayerIndex); // to get which player is using this bar

        maxHP = Model.Health;
        updateHP(Model.Health);
    }


    
    
    
    
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
                    HPbar[Mathf.CeilToInt(i/2)].color = new Color(0,0,0,0 ); //make one bar disappear
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
            Model.State = PlayerState.Ko;
            print(Model.State);
        }
    }
    
    #endregion
}
