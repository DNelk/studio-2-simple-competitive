using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelector_CenterRotation: MonoBehaviour
{
    public Image[] modeSelector1;
    public Image[] modeSelector2;
    public Image[] modeSelector3;

    private MainMenu.MenuState GoToState;

    public Sprite[] versus;
    public Sprite[] arcade;
    public Sprite[] quit;
        
    // Start is called before the first frame update
    void Start()
    {
        swapDamper();
    }

    private void Update()
    {
        if (MainMenu.Instance.currentMenuState == MainMenu.MenuState.ToVersus)
        {
            GetComponent<Animator>().SetBool("toVersus",true);
            transform.SetAsLastSibling();
        }
    }

    public void circlingDown(MainMenu.MenuState goToState)
    {
       GetComponent<Animator>().SetBool("circlingDown",true);
       
       switch (goToState)
       {
           case MainMenu.MenuState.VersusSelected:
               modeSelector2[0].sprite = versus[0];
               modeSelector2[1].sprite = versus[1];
               break;
           case MainMenu.MenuState.ArcadeSelected:
               modeSelector2[0].sprite = arcade[0];
               modeSelector2[1].sprite = arcade[1];
               break;
           case MainMenu.MenuState.QuitSelected:
               modeSelector2[0].sprite = quit[0];
               modeSelector2[1].sprite = quit[1];
               break;
       }

       GoToState = goToState;
    }
    
    public void circlingUp(MainMenu.MenuState goToState)
    {
        GetComponent<Animator>().SetBool("circlingUp",true);
       
        switch (goToState)
        {
            case MainMenu.MenuState.VersusSelected:
                modeSelector3[0].sprite = versus[0];
                modeSelector3[1].sprite = versus[1];
                break;
            case MainMenu.MenuState.ArcadeSelected:
                modeSelector3[0].sprite = arcade[0];
                modeSelector3[1].sprite = arcade[1];
                break;
            case MainMenu.MenuState.QuitSelected:
                modeSelector3[0].sprite = quit[0];
                modeSelector3[1].sprite = quit[1];
                break;
        }

        GoToState = goToState;
    }

    void swapSelector()
    {
        GetComponent<Animator>().SetBool("circlingDown", false);
        GetComponent<Animator>().SetBool("circlingUp", false);
        MainMenu.Instance.currentMenuState = GoToState;
    }

    void swapDamper()
    {
        switch (GoToState)
        {
            case MainMenu.MenuState.VersusSelected:
                modeSelector1[0].sprite = versus[0];
                modeSelector1[1].sprite = versus[1];
                break;
            case MainMenu.MenuState.ArcadeSelected:
                modeSelector1[0].sprite = arcade[0];
                modeSelector1[1].sprite = arcade[1];
                break;
            case MainMenu.MenuState.QuitSelected:
                modeSelector1[0].sprite = quit[0];
                modeSelector1[1].sprite = quit[1];
                break;
        }
    }

    private void transitionToVersus()
    {
        transform.parent = transform.parent.transform.parent;
        Destroy(MainMenu.Instance.gameObject);
    }

    private void selfDestroy()
    {
        Destroy(gameObject);
    }

    private void callVersusScreen()
    {
        Instantiate(Resources.Load("Prefabs/PalmmyEffect/VersusScreen"), transform.parent);
    }
}