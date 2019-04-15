using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleFloatingLight : MonoBehaviour
{
    private float glowingSpeed;
    private float moveSpeed;
    private GameObject spawner;
    
    // Start is called before the first frame update
    void Start()
    {
        glowingSpeed = Random.Range(0.002f, 0.005f);
        moveSpeed = Random.Range(-1f, 1f);

        spawner = GameObject.Find("LightEffect");
    }

    // Update is called once per frame
    void Update()
    {
        glowing();
        moving();
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
            spawner.GetComponent<TitleLightEffect>().lightCount--;
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
