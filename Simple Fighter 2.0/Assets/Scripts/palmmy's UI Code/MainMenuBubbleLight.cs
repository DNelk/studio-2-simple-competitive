using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rewired.Utils.Platforms;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBubbleLight : MonoBehaviour
{
    private float glowingSpeed;
    private GameObject spawner;
    private Color current_Color;
    private Color glowing = new Color(0,0,0,0);

    private Color blue;
    private Color purple;
    private Color red;
    private Color yellow;
    private Color white;
    private Color gray;
    

    public int colorDeter;
    
    // Start is called before the first frame update
    void Start()
    { 
        randomingColor();
        
        colorDeter = Random.Range(0, 2);
        switch (MainMenu.Instance.currentMenuState)
        {
            case MainMenu.MenuState.VersusSelected:
                if (colorDeter == 0)
                    GetComponent<Image>().color = blue; //blue
                if (colorDeter == 1)
                    GetComponent<Image>().color = purple; //purple
                break;
            case MainMenu.MenuState.ArcadeSelected:
                if (colorDeter == 0)
                    GetComponent<Image>().color = red; //blue
                if (colorDeter == 1)
                    GetComponent<Image>().color = yellow; //purple
                break;
            case MainMenu.MenuState.QuitSelected:
                if (colorDeter == 0)
                    GetComponent<Image>().color = white; //blue
                if (colorDeter == 1)
                    GetComponent<Image>().color = gray; //purple
                break;
        }

        current_Color = GetComponent<Image>().color;

        glowingSpeed = Random.Range(0.005f, 0.01f);

        spawner = GameObject.Find("BubbleLight");
    }

    // Update is called once per frame
    void Update()
    {
        Glowing();
        
        changingColor();
        
        resetColor();
    }
    
    //random the color set
    void randomingColor()
    {
        blue = new Color(0.3f,Random.Range(0.5f,0.8f),1f,0);
        purple = new Color(Random.Range(0.92f,1f),0.3f,Random.Range(0.8f,1f),0);
        red = new Color(1f, Random.Range(0f,0.1f), Random.Range(0f,0.1f), 0);
        yellow = new Color(1f,Random.Range(0.3f,0.5f), 0f, 0);
        white = new Color(1,1,1,0);
        gray = new Color(Random.Range(0.5f,0.7f), Random.Range(0.5f,0.7f), Random.Range(0.5f,0.7f), 0);
    }

    void changingColor()
    {   
        switch (MainMenu.Instance.currentMenuState)
        {
            case MainMenu.MenuState.VersusSelected:
                if (colorDeter == 0)
                    GetComponent<Image>().color = blue + glowing;
                if (colorDeter == 1)
                    GetComponent<Image>().color = purple + glowing;
                break;
            case MainMenu.MenuState.ArcadeSelected:
                if (colorDeter == 0)
                    GetComponent<Image>().color = red + glowing;
                if (colorDeter == 1)
                    GetComponent<Image>().color = yellow + glowing;
                break;
            case MainMenu.MenuState.QuitSelected:
                if (colorDeter == 0)
                    GetComponent<Image>().color = white + glowing;
                if (colorDeter == 1)
                    GetComponent<Image>().color = gray + glowing;
                break;
        }
    }

    void resetColor()
    {
        if (MainMenu.Instance.bubbleChange == true)
            current_Color = GetComponent<Image>().color;
    }

    void Glowing()
    {
        glowing.a += glowingSpeed;

        if (glowing.a >= 1)
            glowingSpeed = glowingSpeed * -1;

        if (glowing.a < 0)
        {
            spawner.GetComponent<MainMenuLight>().lightCount--;
            Destroy(gameObject);
        }
    }
}
