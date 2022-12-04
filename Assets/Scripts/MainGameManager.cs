using Mighty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MightyGameManager
{
    private static MainGameManager instance;
    public static MainGameManager Instance { get { return instance; } }

    public Text enter;

    MightyGameBrain brain;
    public GameObject cardPrefab;
    public GameObject battleCardPrefab;
    public Image villageBackground;
    public GameObject villageCardsHolder;
    public GameObject BattleCardsHolder;
    public GameObject MainMap;
    public GameObject Battle;
    public GameObject player;
    public GameObject nodeManager;
    public Battle currentBattle;

    public Text goldText;
    public GameObject goldTextVillageObject;
    public Text gameOverText;
    public Text movesText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        brain = MightyGameBrain.Instance;
        goldText = GameObject.Find("Gold").GetComponentInChildren<Text>();
        gameOverText = GameObject.Find("GameOverPanel").GetComponentInChildren<Text>();
        movesText = GameObject.Find("Moves").GetComponentInChildren<Text>();
    }

    void Update()
    {
        HandleInput();

        UpdateUI(); 
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    void HandleInput()
    {
       
    }

    void UpdateUI()
    {
        goldText.text = "" + player.GetComponent<Player>().gold;
        movesText.text = "" + nodeManager.GetComponent<NodeManager>().movesLeft;
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
        if (enteringGameState == "GameOver")
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, false));
        }

        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));
        
        if (enteringGameState == "Battle" )
        {
            Camera.main.orthographicSize = 8;
            MainMap.SetActive(false);
            Battle.SetActive(true);
            currentBattle.SpawnEnemies();
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, false));
        }
        
        if (enteringGameState == "Map")
        {
            Battle.SetActive(false);
            MainMap.SetActive(true);
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, false));
        }

        if (enteringGameState == "Village")
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, false));
        }

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(enteringGameState + "Panel", true, true));
    }

    // This is called by MightyGameBrain on every game state exit (you decide to handle it or not)
    public override IEnumerator OnGameStateExit(string exitingGameState, string enteringGameState)
    {
        if (exitingGameState == "MainMenu")
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, true));
        }

        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state{
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, false));
        }

        if (exitingGameState == "Battle") // Transition panel when leaving GameOver state{
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, false));
            Camera.main.orthographicSize = 8.3f;
        }


        if (exitingGameState == "Map")
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, true));
        }


        yield return new WaitForSeconds(1);

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(exitingGameState + "Panel", false, true));
    }

    public void TriggerLosingBattle(Node currentNode)
    {
        // if standing on potopped node, trigger losing battle and die
        if (currentNode.invaded)
        {
            Mighty.MightyUIManager.Instance.GetUIPanel("GameOverPanel").gameObject.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "You did not help polish troops besieged in Jasna Góra fortress";
            Mighty.MightyUIManager.Instance.GetUIPanel("GameOverPanel").gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Over";
            MightyGameBrain.Instance.TransitToNextGameState("GameOver");

            Debug.Log("YOU LOST!");
        }
        else
        {
            // win 
            gameOverText.text = "Gratulacje!"; // TODO: display some ending info about potop
        }
    }
}
