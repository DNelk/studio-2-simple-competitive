using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRound : MonoBehaviour
{
    public int winningPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void endRound()
    {
        GameManager.Instance.EndRound(winningPlayer);
    }
}
