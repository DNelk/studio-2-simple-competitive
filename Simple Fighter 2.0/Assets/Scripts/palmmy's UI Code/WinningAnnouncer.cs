using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningAnnouncer : MonoBehaviour
{
    private Animator myAnim;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    public void PlayerWin(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                myAnim.SetBool("P1Win", true);
                break;
            case 1:
                myAnim.SetBool("P2Win", true);
                break;
        }
    }

    public void ResetBool()
    {
        myAnim.SetBool("P1Win", false);
        myAnim.SetBool("P2Win", false);
    }

    private void toNewRound()
    {
        GameManager.Instance.StartRound();
    }
}
