using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedTitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ShatteringTitle()
    {
        GameObject shatteredTitle = Instantiate(Resources.Load("Prefabs/PalmmyEffect/CrackedTitle"), transform.parent) as GameObject;
        Destroy(gameObject);
    }
}
