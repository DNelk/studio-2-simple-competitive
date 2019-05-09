using System.Collections;
using System.Collections.Generic;
using Rewired.Utils.Platforms;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    public Image header;
    public Image[] menu;
    public Text[] modeText;

    public Sprite[] headerSelected;
    public Sprite[] selected;
    public Sprite unselected;
    
    // Start is called before the first frame update
    void Start()
    {
        changeMenu();
    }

    public void changeMenu()
    {
        switch (MainMenu.Instance.currentMenuState)
        {
            case MainMenu.MenuState.VersusSelected:
                transition(0);
                break;
            case MainMenu.MenuState.ArcadeSelected:
                transition(1);
                break;
            case MainMenu.MenuState.QuitSelected:
                transition(2);
                break;
        }
    }

    private void transition(int menuIndex)
    {
        header.sprite = headerSelected[menuIndex];

        for (int i = 0; i < 3; i++)
        {
            if (i == menuIndex)
            {
                menu[i].sprite = selected[i];
                modeText[i].fontSize = 72;
            }
            else
            {
                menu[i].sprite = unselected;
                modeText[i].fontSize = 60;
            }
        }
    }
}
