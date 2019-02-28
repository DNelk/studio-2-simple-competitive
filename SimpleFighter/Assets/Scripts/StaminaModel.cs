using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class StaminaModel : MonoBehaviour
{
    public PlayerModel Model;
    public Player player;
    public Image[] StaminaBar;

    private int maxStamina;
    
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(Model.PlayerIndex); // to get which player is using this bar

        maxStamina = Model.Stamina;
        updateStamina(Model.Stamina);
    }

    // Update is called once per frame
    void Update()
    {
//        if (player.GetButtonDown("Strike"))
//            performHit(1);
    }

    public void updateStamina(int Stamina)
    {
        for (int i = 0; i <= Stamina; i++)
        {
            if (i != 0)
                StaminaBar[i-1].color = new Color(1f, 0.8f, 0f, 1f);
            
            if (i <= maxStamina-1)
                StaminaBar[i].color = new Color(0, 0, 0, 0);
        }
    }

    public void performHit(int Stamina)
    {
        Model.Stamina -= Stamina;
        updateStamina(Model.Stamina);
    }
}
