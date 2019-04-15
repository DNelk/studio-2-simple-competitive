using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleLightEffect : MonoBehaviour
{
    private float spawnTime;
    public int lightCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Random.Range(30, 120);
    }

    // Update is called once per frame
    void Update()
    {
        if (lightCount < 20)
        {
            spawnTime--;

            if (spawnTime <= 0)
            {
                StartCoroutine(callLight(Random.Range(2, 5)));
                spawnTime = Random.Range(60, 240);
            }
        }
    }

    private IEnumerator callLight(int lightNumber)
    {
        for (int i = 0; i < lightNumber; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-Screen.width,Screen.width), Random.Range(-Screen.height,Screen.height));
            
            GameObject floatingLight = Instantiate(Resources.Load("Prefabs/PalmmyEffect/BubbleLight"), transform) as GameObject;
            floatingLight.GetComponent<RectTransform>().anchoredPosition= randomPosition;
            floatingLight.transform.localScale = floatingLight.transform.localScale * Random.Range(0.4f, 1.6f);
            floatingLight.GetComponent<Image>().color = new Color(1,1,1,0);
            lightCount++;
            yield return new WaitForSeconds(Random.Range(0.3f, 2f));
        }
        
    }

}
