using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbarEffect : MonoBehaviour
{ 
    //assign this script to HPbar object
    public Animator HPanim;

    private void Start()
    {
        HPanim = transform.GetComponent<Animator>();
    }

    public void Crack()
    {
        HPanim.SetBool("recover", false);
        HPanim.SetBool("cracking", true);
        HPanim.SetBool("lost", false);
    }

    public void Shatter()
    {
        HPanim.SetBool("recover", false);
        HPanim.SetBool("cracking", false);
        HPanim.SetBool("lost", true);
    }

    public void Recover()
    {
        HPanim.SetBool("recover", true);
        HPanim.SetBool("cracking", false);
        HPanim.SetBool("lost", false);

    }
}
