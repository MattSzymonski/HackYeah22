using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    // Movement controls
    public GameObject movementTarget;
    public float speed = 5f;
    // Spawn attributes
    public float spawnRadius = 5.0f;
    public int spawnCount = 5;
    public float spawnSpacing = 1.0f;
    public int currentSpawnCount = 0;
    public List<GameObject> squadMembers = new List<GameObject>();
    public GameObject squadMemberPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, movementTarget.transform.position, step);
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
    }

    void onDrawGizmos()
    {
        DebugExtension.DebugWireSphere(transform.position, Color.yellow, spawnRadius);
    }
}
