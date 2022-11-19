using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement controls
    public float speed = 5f;
    private UnitGroup unitGroup;

    private float fixedZ;
    //private Rigidbody2D rb;

    private GameObject movementTarget;
    // Start is called before the first frame update
    void Start()
    {
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
        //rb = gameObject.GetComponent<Rigidbody2D>(); TODO: move one by one
        movementTarget = unitGroup.moveTarget.gameObject;
        fixedZ = transform.position.z;
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
}
