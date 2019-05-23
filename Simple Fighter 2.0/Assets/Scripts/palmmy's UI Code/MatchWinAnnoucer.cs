using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchWinAnnoucer : MonoBehaviour
{
    public int playerWin;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CallEndMatchMenu()
    {
        GameObject EndMatchMenu = Instantiate(Resources.Load("Prefabs/PalmmyEffect/EndMatchMenu"), transform.parent) as GameObject;
        EndMatchMenu.GetComponent<EndMatchMenu>().playerwin = playerWin;
    }
}
