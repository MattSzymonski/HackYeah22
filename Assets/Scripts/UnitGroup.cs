using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour
{
    private float fixedZ;

    // Movement controls
    public float speed = 5f;

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

    // Start is called before the first frame update
    void Start()
    {
        army = gameObject.transform.Find("Army");
        moveTarget = GetComponentInChildren<FlagController>();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            //Debug.Log("Selected!");
        }
        var step = speed * Time.deltaTime;
        formation.transform.position = Vector2.MoveTowards(formation.transform.position, moveTarget.transform.position, step);
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
            newSquadMember.GetComponent<Unit>().formationSlot = formationTrans;
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
