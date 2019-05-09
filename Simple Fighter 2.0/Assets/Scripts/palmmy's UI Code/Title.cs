using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class Title : MonoBehaviour
{
    private Rewired.Player rewiredPlayer1;
    private Rewired.Player rewiredPlayer2;
    
    // Start is called before the first frame update
    void Start()
    {
        rewiredPlayer1 = ReInput.players.GetPlayer(0);
        rewiredPlayer2 = ReInput.players.GetPlayer(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (rewiredPlayer1.GetButtonDown("Confirm") || rewiredPlayer2.GetButtonDown("Confirm"))
        {
            Instantiate(Resources.Load("Prefabs/PalmmyEffect/TitleAnimation"), transform.parent);
            Destroy(gameObject);
        }
    }
}
