using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement controls
    public float speed = 5f;
    public float enemyDetectionRadius = 2f;
    private UnitGroup unitGroup;

    public Transform formationSlot;
    public GameObject chosenEnemy;

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
        if (unitGroup.inCombat && !unitGroup.exitingCombat)
        {
            // damage
            CombatLogic();
        }
        else
        {
            FlagFollowLogic();
        }
    }

    private void CombatLogic()
    {
        // formation transform is the weighted centre of all units
        // find enemy that has less than 3 other guys attacking them and is closest, move towards them avoiding friendly units
        if (!chosenEnemy)
        {
            Debug.Log("Unit " + gameObject.name + " does not have an enemy assigned");
            return; // TODO: for now
        }
        
        Debug.Log("Unit " + gameObject.name + " enemy: " + chosenEnemy.name);

        // if close enough to the enemy do nothing
        //if (transform.position.x )
        var step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, chosenEnemy.transform.position, step);

        // check if we pressed mouse in combat, then perform FlagFollowLogic (TODO: enemy will still be damaging us?) and trigger this unAttackable for a time
        if (unitGroup.selected)
        {
            unitGroup.inCombat = false;
            unitGroup.exitingCombat = true;
        }

    }

    private void FlagFollowLogic()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, formationSlot.position, step);

        // only for Player units for now TODO: change
        if (tag == Utils.ENEMY_TAG)
            return;
        
        // check if enemy unit in some radius (triggerBox for formationSlot)
        var enemies = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRadius);//, Utils.ENEMY_LAYER); // TODO: layer does not work?
        //if (enemy)
        foreach(var enemy in enemies)
        {
            if (enemy.tag != Utils.ENEMY_TAG || enemy == this)
                continue;

            // whole unit in combat
            unitGroup.selected = false;

            unitGroup.inCombat = true;
            chosenEnemy = enemy.gameObject;

            // choose this unit as enemy's target if don't have any yet
            var enemyUnit = enemy.GetComponent<Unit>();
            if (!enemyUnit.chosenEnemy)
                enemyUnit.chosenEnemy = this.gameObject;
        }
    }

    /*
    // what about mobile TODO: requires sophisticated tools in unity
    private void OnMouseDown()
    {
        if (!unitGroup.selected)
            unitGroup.selected = true; // TODO: deselect old one implement it

        // TODO: deselect all other guys
    }
    */

    // TODO: implement damaging
    // for now simple collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (unitGroup.exitingCombat) //ignore when exiting combat
            return;

        if (!CompareTag(collision.gameObject.tag)) // for now every other object is enemy
        {
            Debug.Log("HitEnemy");
            // TODO: implement killing logic
            //collision.gameObject.GetComponent<Unit>().Die(); // for now both of them die
            collision.gameObject.GetComponentInParent<UnitGroup>().inCombat = true;
        } 
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
