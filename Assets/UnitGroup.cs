using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour
{
    // Spawn attributes
    public float spawnRadius = 5.0f;
    public int spawnCount = 5;
    public float spawnSpacing = 1.0f;
    public int currentSpawnCount = 0;
    public List<GameObject> squadMembers = new List<GameObject>(); // TODO: randomize
    public GameObject squadMemberPrefab;

    public FlagController moveTarget;

    public bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
        moveTarget = GetComponentInChildren<FlagController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            Debug.Log("Selected!");
        }
    }
    public void Spawn()
    {
        while (currentSpawnCount < spawnCount)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.z = transform.position.z;
            GameObject newSquadMember = Instantiate(squadMemberPrefab, spawnPosition, Quaternion.identity) as GameObject;
            newSquadMember.transform.parent = gameObject.transform;
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
        DebugExtension.DebugWireSphere(transform.position, Color.yellow, spawnRadius);
    }

}
