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
    
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(Model.PlayerIndex);
        
        updateHP(Model.Health);
    }

    // Update is called once per frame
    void Update()
    {
        //this is placeholder for testing button and change in HP
        if (player.GetButtonDown("Strike"))
            getHit();
    }

    
    
    
    
    #region  UpdateHP
    void updateHP(float HP) //update HP
    {
        for (float i = 0; i <= HP; i++)
        {
            if (i / 2 == Mathf.CeilToInt(i / 2))
            {
                if (i / 2 != 0)
                    HPbar[Mathf.CeilToInt(i/2)].color = Color.white; //paint white on one bar that is still full
                    
                HPbar[Mathf.CeilToInt(i/2) + 1].color = new Color(0,0,0,0 ); //make one bar disappear
            }
            else
            {
                HPbar[Mathf.CeilToInt(i / 2)].color = Color.red; //paint red on bar with half left
            }
            Debug.Log(i/2);   
        }
    }
    
    #endregion

    #region  getHit

    void getHit()
    {
        Model.Health = Model.Health - 1;
        updateHP(Model.Health);
    }
    
    #endregion
}
