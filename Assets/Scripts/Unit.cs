using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement controls
    public float speed = 5f;
    public float enemyDetectionRadius = 1f;
    private UnitGroup unitGroup;

    public Transform formationSlot;
    public GameObject chosenEnemy;

    public float maxDistanceFromFormation = 2.0f;
    public bool inCombat = false;
    //private float fixedZ;
    //private Rigidbody2D rb;
    private float distanceToFormationSlot;

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
        FlagFollowLogic();
        distanceToFormationSlot = Vector2.Distance(transform.position, formationSlot.position);
        CombatLogic();
    }

    private void CombatLogic()
    {
        if (distanceToFormationSlot > maxDistanceFromFormation)
        {

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
        Debug.Log(enemies);
        ////if (enemy)
        //foreach(var enemy in enemies)
        //{
        //    if (enemy.tag != Utils.ENEMY_TAG || enemy == this)
        //        continue;

        //    // whole unit in combat
        //    unitGroup.selected = false;

        //    unitGroup.inCombat = true;
        //    chosenEnemy = enemy.gameObject;

        //    // choose this unit as enemy's target if don't have any yet
        //    var enemyUnit = enemy.GetComponent<Unit>();
        //    if (!enemyUnit.chosenEnemy)
        //        enemyUnit.chosenEnemy = this.gameObject;
        //}
    }

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
