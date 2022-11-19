using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlagController : MonoBehaviour
{
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
