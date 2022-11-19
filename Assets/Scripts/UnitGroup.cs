using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour
{
    private float fixedZ;

    // Movement controls
    public float speed = 5f;
    public float rotationSpeed = 0.5f;

    public float combatExitProtectionTime = 2.0f;

    // Spawn attributes
    //public float spawnRadius = 5.0f;
    public int spawnCount = 10;
    //public float spawnSpacing = 1.0f;
    public int currentSpawnCount = 0;
    public List<GameObject> squadMembers = new List<GameObject>(); // TODO: randomizes
    public GameObject formation;

    public GameObject squadMemberPrefab;

    private Transform army;

    public FlagController moveTarget;

    public bool selected = false;
    public bool inCombat = false;
    public bool exitingCombat = false;
    public bool isEnemy = false;

    public Mighty.MightyTimer combatExitTimer;

    // Start is called before the first frame update
    void Start()
    {
        army = gameObject.transform.Find("Army");
        moveTarget = GetComponentInChildren<FlagController>();
        combatExitTimer = new Mighty.MightyTimer("ExitingCombatTimer", combatExitProtectionTime, 1.0f, false, true);
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (combatExitTimer.finished)
        {
            exitingCombat = false;
        }

        if (exitingCombat)
        {
            combatExitTimer.RestartTimer();
            combatExitTimer.PlayTimer();
        }

        if (selected)
        {
            //Debug.Log("Selected!");
        }
        var step = speed * Time.deltaTime;
        formation.transform.position = Vector2.MoveTowards(formation.transform.position, moveTarget.transform.position, step);
        // rotation
        //Quaternion targetRotation = Quaternion.LookRotation(moveTarget.transform.position);
        //formation.transform.rotation = Quaternion.RotateTowards(formation.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (formation.transform.position.x != moveTarget.transform.position.x && formation.transform.position.y != moveTarget.transform.position.y)
        {
            Vector3 relativePos = moveTarget.transform.position - formation.transform.position;
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg + -90.0f;
            formation.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        //rb.velocity = new Vector3(dest.x, dest.y, 0);

        // update UnitGroups status
        UpdateUnitDeath();
    }
    public void Spawn()
    {
        foreach (Transform formationTrans in formation.transform)
        {
            GameObject newSquadMember = Instantiate(squadMemberPrefab, formationTrans.position, Quaternion.identity) as GameObject;
            newSquadMember.transform.parent = army;
            newSquadMember.transform.position = new Vector3(formationTrans.position.x, formationTrans.position.y, army.position.z);
            newSquadMember.layer = Utils.ENEMY_LAYER;
            newSquadMember.GetComponent<Unit>().formationSlot = formationTrans;
            if (isEnemy)
                newSquadMember.tag = Utils.ENEMY_TAG;
        }

        /* TOOD: bannerman or just a flag to be drageed around
        // choose a bannerman (change the sprite)
        int rand = Random.Range(0, squadMembers.Count + 1);
        bannerman = squadMembers[rand];
        //bannerman.GetComponent<Sprite>() // swap it
        */
    }

    private void UpdateUnitDeath()
    {
        if (squadMembers.TrueForAll(g => g.activeSelf == false))
        {
           // Debug.Log("All Dead");
            //gameObject.SetActive(false);
        }
    }

    void onDrawGizmos()
    {
        //DebugExtension.DebugWireSphere(army.position, Color.yellow, spawnRadius);
    }

}
