using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EffectAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EffectAnimation()
    {
        yield return StartCoroutine(WaitFor.Frames(7));
        Destroy(gameObject);
    }
}
