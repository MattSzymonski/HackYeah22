using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuicAnim : MonoBehaviour
{

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetBool(bool value)
    {
        Debug.Log("xxxx " + value);
        anim.SetBool("Anim", value);
    }
}
