using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelecter_BackCircling : MonoBehaviour
{
    public int clockSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().Rotate(0,0,Time.deltaTime*clockSpeed);
    }
}
