using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class TitleExplosion : MonoBehaviour
{
    private float gravityScale = 800f;
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 CenterModifier = new Vector3(0, -50f, -50f);

        rb.useGravity = false;
        rb.AddExplosionForce(2000f, transform.parent.position + CenterModifier, 1000f, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gravity = Vector3.down * gravityScale;
        //Vector3 pushForce = Vector3.forward * 500f;
        rb.AddForce(gravity,ForceMode.Acceleration);
        
        GetComponent<Image>().color += new Color(0,0,0,-0.01f);
        if (GetComponent<Image>().color.a <= 0f)
            Destroy(transform.parent.gameObject);
    }
}
