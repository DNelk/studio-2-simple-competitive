using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightRotating : MonoBehaviour
{
    private float oriZ;
    private int speedDeter;
    private float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        oriZ = GetComponent<RectTransform>().rotation.z;
        speedDeter = Random.Range(0, 2);
        if (speedDeter == 0)
            speed = Random.Range(-8f, -2f);
        else
        {
            speed = Random.Range(2f, 8f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().Rotate(0,0,Time.deltaTime*speed);

        if (GetComponent<RectTransform>().rotation.z >= (oriZ + 0.15f))
            speed = -speed;
        if (GetComponent<RectTransform>().rotation.z <= (oriZ - 0.15f))
            speed = -speed;
    }
}
