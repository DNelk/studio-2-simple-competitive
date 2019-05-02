using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCaller : MonoBehaviour
{
    private GameObject player1;
    private GameObject player2;
    private PlayerView P1view;
    private PlayerView P2view;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        callCounter();
        callRecover();
        callOneDown();
        callTwoDown();
        callKO();
    }

    private void callCounter()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject counterP1 = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/CounterText"),transform) as GameObject;
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameObject counterP2 = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/CounterText"),transform) as GameObject;
        }

    }

    private void callRecover()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GameObject recoverP1 =Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/RecoverText"),transform) as GameObject;
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject recoverP2 =Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/RecoverText"),transform) as GameObject;
        }
    }

    private void callOneDown()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            GameObject OneDownP1 =Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/DownOneText"),transform) as GameObject;
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameObject OneDownP2 =Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/DownOneText"),transform) as GameObject;
        }
    }
    
    private void callTwoDown()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameObject TwoDownP1 =Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player1/DownTwoText"),transform) as GameObject;
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject TwoDownP2 =Instantiate(Resources.Load("Prefabs/PalmmyEffect/BattleEffect/Player2/DownTwoText"),transform) as GameObject;
        }
    }

    private void callKO()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject KO =Instantiate(Resources.Load("Prefabs/PalmmyEffect/KO"),transform.parent) as GameObject;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject Perfect =Instantiate(Resources.Load("Prefabs/PalmmyEffect/PERFECT"),transform.parent) as GameObject;
        }
    }

}
