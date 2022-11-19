using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualization : MonoBehaviour
{
    Animator anim;
    public Village village;
    public int index;
    public bool isBattleCardSelected;

    private Vector3 cardPosition;

    public void Start()
    {
        anim = transform.GetComponent<Animator>();
    }

    public void Update()
    {
        if (isBattleCardSelected)
        {
            transform.position = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("PRessed X");
                // Cancel action and return card to original position
                isBattleCardSelected = false;
                GetComponent<RectTransform>().localScale = new Vector3(0.5f,0.5f,0.5f);
                Cursor.visible = true;
                transform.position = cardPosition;
            }
        }
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

    public void BattleSelect(bool value)
    {
        if (value)
        {
            isBattleCardSelected = true;
            GetComponent<RectTransform>().localScale = new Vector3(0.2f,0.2f,0.2f);
            Cursor.visible = false;
            // Store last card position to snap back
            cardPosition = transform.position;
        }
        else
        {
            if (!isBattleCardSelected)
            {
                return;
            }
            isBattleCardSelected = false;
            GetComponent<RectTransform>().localScale = new Vector3(0.5f,0.5f,0.5f);
            Cursor.visible = true;
            // Place card
            PlaceUnit();
        }
    }

    public void PlaceUnit()
    {
        Debug.Log("Placed unit");
    }
}
