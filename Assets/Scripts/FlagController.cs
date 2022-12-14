using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlagController : MonoBehaviour
{
    private Vector3 offset;
    private float fixedZ;
    private UnitGroup unitGroup;
    private bool isEnemy = false;
    public Vector2 patrolLength = new Vector2(10.0f, 0.0f);
    private Vector2 startPosition;

    private void Start()
    {
        isEnemy = transform.parent.tag == "Enemy"; 
        startPosition = transform.position;
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
        fixedZ = transform.position.z;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined; // apparently does not work on Mac OS
    }

    void Update()
    {
        if (unitGroup.inCombat)
        {
            return;
        }
        if (isEnemy)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        transform.position = startPosition + new Vector2(Mathf.PingPong(Time.time, 10.0f), 0.0f);

    }
    

    void OnMouseDown()
    {
        if (isEnemy)
        {
            return;
        }
        unitGroup.selected = true;
        //if (!unitGroup.selected)
            //return;
        
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }

    void OnMouseDrag()
    {
        if(isEnemy)
        {
            return;
        }
        if (!unitGroup.selected)
            return;

        Vector3 dst = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) + offset;
        transform.position = new Vector3(dst.x, dst.y, fixedZ);
    }

    void OnMouseUp()
    {
        if(isEnemy)
        {
            return;
        }
        if (!unitGroup.selected)
            return;

        Cursor.visible = true;
    }

}
