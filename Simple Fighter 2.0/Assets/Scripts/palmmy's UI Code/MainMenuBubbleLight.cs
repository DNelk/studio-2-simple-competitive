using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBubbleLight : MonoBehaviour
{
    private float glowingSpeed;
    private float moveSpeed;
    private GameObject spawner;

    public int colorDeter;
    
    // Start is called before the first frame update
    void Start()
    {
        colorDeter = Random.Range(0, 2);
        if (colorDeter == 0)
            GetComponent<Image>().color = new Color(0.3f,Random.Range(0.5f,0.8f),1f,0); //blue
        if (colorDeter == 1)
            GetComponent<Image>().color = new Color(Random.Range(0.92f,1f),0.3f,Random.Range(0.8f,1f),0); //purple
        
        glowingSpeed = Random.Range(0.005f, 0.01f);
        moveSpeed = Random.Range(-0.2f, 0.2f);

        spawner = GameObject.Find("BubbleLight");
    }

    // Update is called once per frame
    void Update()
    {
        glowing();
        //moving();
    }

    void glowing()
    {
        Color glowing = GetComponent<Image>().color;

        glowing.a += glowingSpeed;
        GetComponent<Image>().color = glowing;

        if (glowing.a >= 1)
            glowingSpeed = glowingSpeed * -1;

        if (glowing.a < 0)
        {
            spawner.GetComponent<MainMenuLight>().lightCount--;
            Destroy(gameObject);
        }
    }

    void moving()
    {
        Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
        pos.x += moveSpeed;
        GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
