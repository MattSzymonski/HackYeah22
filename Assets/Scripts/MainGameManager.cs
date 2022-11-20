using Mighty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MightyGameManager
{
    private static MainGameManager instance;
    public static MainGameManager Instance { get { return instance; } }



    MightyGameBrain brain;
    public GameObject cardPrefab;
    public GameObject battleCardPrefab;
    public Image villageBackground;
    public GameObject villageCardsHolder;
    public GameObject BattleCardsHolder;
    public GameObject MainMap;
    public GameObject Battle;
    public GameObject player;

    public Text goldText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        brain = MightyGameBrain.Instance;
        goldText = GameObject.Find("Gold").GetComponentInChildren<Text>();
    }

    void Update()
    {
        HandleInput();

        UpdateUI(); 
    }

    void HandleInput()
    {
       
    }

    void UpdateUI()
    {
        goldText.text = "Z?oto: " + player.GetComponent<Player>().gold;
    }

    public void CloseInfoPanel()
    {
        StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("InfoPanel", false, false));
    }

    public void OpenInfoPanel()
    {
        StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("InfoPanel", true, true));
    }

    // --- MightyGameBrain callbacks ---

    // This is called by MightyGameBrain on every game state enter (you decide to handle it or not)
    public override IEnumerator OnGameStateEnter(string enteringGameState, string exitingGameState)
    {
     

        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));
        
        if (enteringGameState == "Battle" )
        {
            MainMap.SetActive(false);
            Battle.SetActive(true);
        }
        
        if (enteringGameState == "Map")
        {
            Battle.SetActive(false);
            MainMap.SetActive(true);
        }

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(enteringGameState + "Panel", true, true));
    }

    // This is called by MightyGameBrain on every game state exit (you decide to handle it or not)
    public override IEnumerator OnGameStateExit(string exitingGameState, string enteringGameState)
    {
        if (exitingGameState == "MainMenu")
        {
            // TODO New game setup here!
        }

        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, false));


        yield return new WaitForSeconds(1);

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(exitingGameState + "Panel", false, true));
    }

    public void TriggerLosingBattle()
    {
        Debug.Log("You Lose!");
        // if standing on potopped node, trigger losing battle and die
        // else win
    }
}
