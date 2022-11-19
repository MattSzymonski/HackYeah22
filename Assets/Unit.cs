using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement controls
    public float speed = 5f;
    private UnitGroup unitGroup;

    private GameObject movementTarget;
    // Start is called before the first frame update
    void Start()
    {
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
        movementTarget = unitGroup.moveTarget.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, movementTarget.transform.position, step);
    }

    // what about mobile TODO: requires sophisticated tools in unity
    private void OnMouseDown()
    {
        if (!unitGroup.selected)
            unitGroup.selected = true; // TODO: deselect old one implement it
    }
}
