using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingLight : MonoBehaviour
{
    private bool lighthening;
    
    // Use this for initialization
    void Start () {
        GetComponent<Image>().color += new Color(0,0,0,Random.Range(-1,1));
    }
     
    // Update is called once per frame
    void Update ()
    {

        if (GetComponent<Image>().color.a >= 1)
            lighthening = false;
        
        if (GetComponent<Image>().color.a <= 0)
            lighthening = true;
        
        if (lighthening == true)
            GetComponent<Image>().color += new Color(0,0,0,Random.Range(0.001f,0.01f));
        else
        {
            GetComponent<Image>().color += new Color(0,0,0, Random.Range(-0.01f,-0.001f));
        }

    }
}
