using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeWindow : MonoBehaviour
{
    public Text content; 
        
    // Start is called before the first frame update
    void Start()
    {
        switch (MainMenu.Instance.currentMenuState)
        {
            case MainMenu.MenuState.ArcadeSelected:
                content.text = "arcade MODE IS UNDER DEVELOPMENT";
                break;
            default:
                content.text = "ONLY versus MODE IS AVAILABLE AT THE TIME OF THIS DEMO";
                break;  
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void selfDestroy()
    {
        Destroy(gameObject);
    }
}
