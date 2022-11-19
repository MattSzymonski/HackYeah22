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
    public Image villageBackground;
    public GameObject villageCardsHolder;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        brain = MightyGameBrain.Instance;
    }

    void Update()
    {
        HandleInput();


    }

    void HandleInput()
    {
       
    }

    public void CloseInfoPanel()
    {
        MightyUIManager.Instance.ToggleUIPanel("InfoPanel", false, false);
    }

    // --- MightyGameBrain callbacks ---

    // This is called by MightyGameBrain on every game state enter (you decide to handle it or not)
    public override IEnumerator OnGameStateEnter(string enteringGameState, string exitingGameState)
    {
     

        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));

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
}
