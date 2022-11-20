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
        for (int i = 1; i < maxWaves; ++i)
        {
            waves[i].SetActive(false);
            dottedWaves[i].SetActive(false);
        }
        //waves[currentWave].GetComponent<SpriteRenderer>().
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceWave()
    {
        waves[currentWave].SetActive(false);
        dottedWaves[currentWave].SetActive(false);
        ++currentWave;
        if (currentWave > maxWaves)
        {
            mainGameManager.TriggerLosingBattle();
            return;
        }
        // animations etc for transitions
        waves[currentWave].SetActive(true); // TODO: fade in
        dottedWaves[currentWave].SetActive(true); // TODO: fade in
    }

    public void Restart()
    {

    }
}
