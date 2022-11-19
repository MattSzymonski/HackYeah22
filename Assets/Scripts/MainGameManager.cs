using Mighty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainGameManager : MightyGameManager
{
    MightyGameBrain brain;

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
        if (Input.GetButtonDown("Escape"))
        {
            MightyAudioManager.Instance.PlaySound("UI_Button_Click");

            if (brain.currentGameStateName == "Playing")
                brain.TransitToNextGameState("Pause");

            if (brain.currentGameStateName == "Pause")
                brain.TransitToNextGameState("Playing");

            if (brain.currentGameStateName == "Options")
                brain.TransitToNextGameState("Pause");
        }

        if (Input.GetButtonDown("ControllerAny Start"))
        {
            MightyAudioManager.Instance.PlaySound("UI_Button_Click");

            if (brain.currentGameStateName == "Playing")
                brain.TransitToNextGameState("Pause");

            if (brain.currentGameStateName == "Pause")
                brain.TransitToNextGameState("Playing");
        }
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
        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, false));

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(exitingGameState + "Panel", false, true));
    }
}
