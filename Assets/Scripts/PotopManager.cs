using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotopManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] waves;
    public GameObject[] dottedWaves;
    public int currentWave = 0;
    public int maxWaves = 5;

    private MainGameManager mainGameManager;

    void Start()
    {
        mainGameManager = GameObject.Find("GameManager").GetComponent<MainGameManager>();
        dottedWaves[0].GetComponent<Animator>().SetBool("WaveStart", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceWave(Node currentnode)
    {
        ++currentWave;
        if (currentWave > maxWaves)
        {
            mainGameManager.TriggerLosingBattle(currentnode);
            return;
        }
        // animations etc for transitions
        waves[currentWave].GetComponent<Animator>().SetBool("WaveStart", true);
        dottedWaves[currentWave].GetComponent<Animator>().SetBool("WaveStart", true);
    }

    public void Restart()
    {

    }
}
