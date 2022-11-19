using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement controls
    public float speed = 5f;
    private UnitGroup unitGroup;

    public bool inCombat = false;

    public Transform formationSlot;

    //private float fixedZ;
    //private Rigidbody2D rb;

    //private GameObject movementTarget;
    // Start is called before the first frame update
    void Start()
    {
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
        //rb = gameObject.GetComponent<Rigidbody2D>(); TODO: move one by one
        //movementTarget = unitGroup.moveTarget.gameObject;
        //fixedZ = transform.position.z;
    }

    private void Update()
    {
        if (inCombat)
        {
            // damage
        }
        else
        {
            var step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, formationSlot.position, step);
        }
    }

    // Update is called once per frame
    /*
    void Update()
    {
        var step = speed * Time.deltaTime;
        Vector3 dest = Vector3.MoveTowards(transform.position, movementTarget.transform.position, step);
        transform.position = new Vector3(dest.x, dest.y, fixedZ);
        //rb.velocity = new Vector3(dest.x, dest.y, 0);
    }
    */

    // what about mobile TODO: requires sophisticated tools in unity
    private void OnMouseDown()
    {
        if (!unitGroup.selected)
            unitGroup.selected = true; // TODO: deselect old one implement it

        // TODO: deselect all other guys
    }

    // TODO: implement damaging
    // for now simple collision
    private void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log("Coll");
        if (!CompareTag(collision.gameObject.tag)) // for now every other object is enemy
        {
            Debug.Log("HitEnemy");
            // TODO: implement killing logic
            //collision.gameObject.GetComponent<Unit>().Die(); // for now both of them die
            inCombat = true;
            collision.gameObject.GetComponent<Unit>().inCombat = true;
            
        } 
    }

    private void Die()
    {
        gameObject.SetActive(false);
        inCombat = false;
    }
}
