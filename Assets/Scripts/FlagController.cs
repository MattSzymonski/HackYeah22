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
    public float patrollingSpeed = 0.7f;
    public float circlePatrollingRadius = 2.0f;
    private Vector2 startPosition;

    public delegate void PatrolMethod();
    PatrolMethod patrolMethod;

    private void Start()
    {
        isEnemy = transform.parent.tag == "Enemy"; 
        startPosition = transform.position;
        unitGroup = gameObject.GetComponentInParent<UnitGroup>();
        fixedZ = transform.position.z;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined; // apparently does not work on Mac OS
        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
                patrolMethod = PatrolLR;
                break;

            case 1:
                patrolMethod = PatrolUD;
                break;

            case 2:
                patrolMethod = PatrolCircle;
                break;

            case 3:
                patrolMethod = PatrolOblique;
                break;

            default:
                Debug.LogError("No such function!");
                break;
        }
    }

    void Update()
    {
        if (unitGroup.IsInCombat())
        {
            return;
        }
        if (isEnemy)
        {
            patrolMethod();
        }
    }

    void PatrolLR()
    {
        transform.position = startPosition + new Vector2(Mathf.PingPong(Time.time * patrollingSpeed, 3.0f), 0.0f);
    }
    
    void PatrolUD()
    {
        transform.position = startPosition + new Vector2(0.0f, Mathf.PingPong(Time.time * patrollingSpeed, 3.0f));
    }
    void PatrolCircle()
    {
        float x = Mathf.Sin(Time.time * patrollingSpeed) * circlePatrollingRadius;
        float y = Mathf.Cos(Time.time * patrollingSpeed) * circlePatrollingRadius;
        transform.position = startPosition + new Vector2(x, y);
    }
    void PatrolOblique()
    {
        transform.position = startPosition + new Vector2(Mathf.PingPong(Time.time * patrollingSpeed, 3.0f), Mathf.PingPong(Time.time * patrollingSpeed, 3.0f));
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
        if (isEnemy)
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
        if (isEnemy)
        {
            return;
        }
        if (!unitGroup.selected)
            return;

        Cursor.visible = true;
    }

}
