using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLight : MonoBehaviour
{
    private float spawnTime;
    public float lightCount;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Random.Range(15, 60);
        
        StartCoroutine(callLight(Random.Range(10, 20)));
    }

    // Update is called once per frame
    void Update()
    {
        if (lightCount <= 100)
        {
            spawnTime--;

            if (spawnTime <= 0)
            {
                StartCoroutine(callLight(Random.Range(7, 12)));
                spawnTime = Random.Range(15, 60);
            }   
        }
    }

    private IEnumerator callLight(int lightNumber)
    {
        for (int i = 0; i < lightNumber; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-Screen.width*1.2f,Screen.width*1.2f), Random.Range((-Screen.height*0.7f),Screen.height));
            
            GameObject floatingLight = Instantiate(Resources.Load("Prefabs/PalmmyEffect/MainMenuBubbleLight"), transform) as GameObject;
            floatingLight.GetComponent<RectTransform>().anchoredPosition= randomPosition;
            
            if ((randomPosition.x/Screen.width) >= -0.3)
                floatingLight.transform.localScale = floatingLight.transform.localScale * Random.Range(0.5f, 1f)*((randomPosition.x+Screen.width)/Screen.width);
            else
            {
                floatingLight.transform.localScale = floatingLight.transform.localScale * Random.Range(0.5f, 1f) * 0.7f;
            }
            floatingLight.GetComponent<Image>().color = new Color(1,1,1,0);
            lightCount++;
            yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
        }
        
    }

}
