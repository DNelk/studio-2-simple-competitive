using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    public bool what;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<Animator>().SetBool("punch", true);
            GetComponent<Animator>().SetBool("donePunch",false);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            GetComponent<Animator>().SetBool("donePunch", true);
            GetComponent<Animator>().SetBool("punch",false);
        }
}


}
