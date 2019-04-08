using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningAnnouncer : MonoBehaviour
{
    public KeyCode[] winner; // 0 = player1, 1 = player2 
    public Animator winningAnnouncer;

    public int P1winNumber;
    public int P2winNumber;

    public GameObject[] P1WinBar;
    public GameObject[] P2WinBar;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MakePlayerWin();
    }

    void MakePlayerWin()
    {
        if (Input.GetKeyDown(winner[0]))
        {
            if (P1winNumber < 3)
            {
                winningAnnouncer.SetBool("P1Win", true);
                P1winNumber++;
            }
        }

        if (Input.GetKeyDown(winner[1]))
        {
            if (P2winNumber < 3)
            {
                winningAnnouncer.SetBool("P2Win",true);
                P2winNumber++;
            }
        }
    }

    void ResetBool()
    {
        winningAnnouncer.SetBool("P1Win", false);
        winningAnnouncer.SetBool("P2Win", false);
    }

    void P1Win()
    {
        Instantiate(Resources.Load("Prefabs/PalmmyEffect/Winpoint"), P1WinBar[P1winNumber - 1].transform);
    }

    void P2Win()
    {
        Instantiate(Resources.Load("Prefabs/PalmmyEffect/Winpoint"), P2WinBar[P2winNumber - 1].transform);
    }
}
