using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlagController : MonoBehaviour
{
    /*
    private Vector3 screenPoint; private Vector3 offset; private float _lockedZPosition;
    private UnitGroup unitGroup;

    private void Start()
    {
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
    }

    void OnMouseDown()
    {
        if (!unitGroup.selected)
            return;
        //screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position); // I removed this line to prevent centring 
        _lockedZPosition = screenPoint.z;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        Cursor.visible = false;
    }

    void OnMouseDrag()
    {
        if (!unitGroup.selected)
            return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        curPosition.z = _lockedZPosition;
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        if (!unitGroup.selected)
            return;
        Cursor.visible = true;
    }
    */
    private Vector3 offset;
    private float fixedZ;
    private UnitGroup unitGroup;

    private void Start()
    {
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
        fixedZ = transform.position.z;
    }

    void OnMouseDown()
    {
        unitGroup.selected = true;
        //if (!unitGroup.selected)
            //return;

        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }

    void OnMouseDrag()
    {
        if (!unitGroup.selected)
            return;

        Vector3 dst = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) + offset;
        transform.position = new Vector3(dst.x, dst.y, fixedZ);
    }

    void OnMouseUp()
    {
        if (!unitGroup.selected)
            return;
        Cursor.visible = true;
    }

}
