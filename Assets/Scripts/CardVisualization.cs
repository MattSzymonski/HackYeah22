using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualization : MonoBehaviour
{
    Animator anim;
    public Village village;
    public int index;

    public void Start()
    {
        anim = transform.GetComponent<Animator>();
    }

    public void Instantiate(Card card)
    {

    }

    public void Select(bool value)
    {
        anim.SetBool("Selected", value);
    }

    public void Buy()
    {
        bool bought = village.Buy(index);
        if (bought)
        {
            anim.SetBool("Bought", true);
        }
    }
}
