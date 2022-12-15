using Mighty;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotopManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] waves;
    public GameObject[] dottedWaves;
    public int currentWave = 0;
    public int maxWaves = 5;

    private MainGameManager mainGameManager;

    bool advanceInProgress = false;
    Node advanceNode;
    List<GameObject> nodeLevels;
    int waveProgression;

    void Start()
    {
        mainGameManager = GameObject.Find("GameManager").GetComponent<MainGameManager>();
        dottedWaves[0].GetComponent<Animator>().SetBool("WaveStart", true);
    }

    // Update is called once per frame
    void Update()
    {
        Mighty.MightyUIManager men = Mighty.MightyUIManager.Instance;

        // Advance with delay
        if (advanceInProgress && 
            men.GetUIPanel("InfoPanel").active == false &&
            men.GetUIPanel("VillagePanel").active == false &&
            men.GetUIPanel("BattlePanel").active == false
        )
        {
            Advance();
            advanceInProgress = false;
        }
    }

    void Advance()
    {
        ++currentWave;
        if (currentWave > maxWaves)
        {
            mainGameManager.TriggerLosingBattle(advanceNode);
            return;
        }
        // animations etc for transitions
        waves[currentWave].GetComponent<Animator>().SetBool("WaveStart", true);
        dottedWaves[currentWave].GetComponent<Animator>().SetBool("WaveStart", true);
        dottedWaves[currentWave - 1]?.GetComponent<Animator>().SetBool("WaveStart", false);

        foreach (Transform node in nodeLevels[waveProgression].transform)
        {
            node.gameObject.GetComponent<Node>().invaded = true;
            node.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.0f, 0.0f);

            // if player is in invaded node
            if (node == advanceNode.transform)
            {
                IEnumerator Delay()
                {
                    yield return new WaitForSeconds(10);
                    Mighty.MightyUIManager.Instance.GetUIPanel("GameOverPanel").gameObject.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "Your troops have been captured by the advancing Swedish army";
                    Mighty.MightyUIManager.Instance.GetUIPanel("GameOverPanel").gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Game Over";
                    MightyGameBrain.Instance.TransitToNextGameState("GameOver");
                }

                StartCoroutine(Delay());

                //Mighty.MightyUIManager.Instance.GetUIPanel("GameOverPanel").gameObject.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "Your troops have been captured by advancing Swedish army";
                //Mighty.MightyUIManager.Instance.GetUIPanel("GameOverPanel").gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Game Over";
                //MightyGameBrain.Instance.TransitToNextGameState("GameOver");
                return;
            }
        }
    }

    public void AdvanceWave(Node currentnode, List<GameObject> NodeLevels, int waveProgres)
    {
        waveProgression = waveProgres;
        nodeLevels = NodeLevels;
        advanceNode = currentnode;
        advanceInProgress = true;
    }

    public void Restart()
    {

    }
}
