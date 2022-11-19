using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour
{
    private float fixedZ;

    // Movement controls
    public float speed = 5f;

    // Spawn attributes
    public float spawnRadius = 5.0f;
    public int spawnCount = 5;
    public float spawnSpacing = 1.0f;
    public int currentSpawnCount = 0;
    public List<GameObject> squadMembers = new List<GameObject>(); // TODO: randomize
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
        Vector3 dest = Vector3.MoveTowards(army.position, moveTarget.transform.position, step);
        army.position = new Vector3(dest.x, dest.y, fixedZ);
        //rb.velocity = new Vector3(dest.x, dest.y, 0);
    }
    public void Spawn()
    {
        while (currentSpawnCount < spawnCount)
        {
            Vector3 spawnPosition = army.position + Random.insideUnitSphere * spawnRadius;
            GameObject newSquadMember = Instantiate(squadMemberPrefab, spawnPosition, Quaternion.identity) as GameObject;
            newSquadMember.transform.parent = army;
            newSquadMember.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, army.position.z);
            //newSquadMember.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
            newSquadMember.tag = gameObject.tag;
            squadMembers.Add(newSquadMember);
            currentSpawnCount++;
        }

        /* TOOD: bannerman or just a flag to be drageed around
        // choose a bannerman (change the sprite)
        int rand = Random.Range(0, squadMembers.Count + 1);
        bannerman = squadMembers[rand];
        //bannerman.GetComponent<Sprite>() // swap it
        */
    }

    void onDrawGizmos()
    {
        DebugExtension.DebugWireSphere(army.position, Color.yellow, spawnRadius);
    }

}
