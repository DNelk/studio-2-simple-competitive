using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelector_ScreenChange : MonoBehaviour
{
    public Image mcScreen;
    public Image KDScreen;

    public Sprite[] MCselected;
    public Sprite[] KDselected;
    

    // Start is called before the first frame update
    void Start()
    {
        changeModeScreen(MainMenu.MenuState.VersusSelected);        
    }

    
    public void changeModeScreen(MainMenu.MenuState goToState)
    {
        switch (goToState)
        {
            case MainMenu.MenuState.VersusSelected:
                colorChange(0);
                break;
            case MainMenu.MenuState.ArcadeSelected:
                colorChange(1);
                break;
            case MainMenu.MenuState.QuitSelected:
                colorChange(2);
                break;
        }
    }
    
    private void colorChange(int menuIndex)
    {
        mcScreen.sprite = MCselected[menuIndex];
        KDScreen.sprite = KDselected[menuIndex];
    }
}
