using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatlleMapManager : MonoBehaviour
{
    public GameObject playerSpawnZone;
    public GameObject enemySpawnZone;

    public GameObject[] enemyArmyContainer;
    public GameObject[] playerArmyContainer;

    public bool playerWon = false;
    public bool playerLost = false;


    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();        
    }

    // Update is called once per frame
    void Update()
    {
        // check the conditions for winning/losing a battle
        if (enemyArmyContainer.Length == 0)
        {
            playerWon = true;
        }
        else if (playerArmyContainer.Length == 0) // or player retreated ?
        {
            playerLost = true;
        }
    }

    void SpawnEnemies()
    {

    }
}
