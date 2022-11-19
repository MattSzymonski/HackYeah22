using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Mighty
{
    [Serializable] public class OnEnterGameStateStartedEvent : UnityEvent<string, string> { } // enteringGameState and exitingGameState
    [Serializable] public class OnEnterGameStateFinishedEvent : UnityEvent<string, string> { } // enteringGameState and exitingGameState

    [Serializable] public class OnExitGameStateStartedEvent : UnityEvent<string, string> { } // exitingGameState and enteringGameState
    [Serializable] public class OnExitGameStateFinishedEvent : UnityEvent<string, string> { } // exitingGameState and enteringGameState

    public abstract class MightyGameManager : MonoBehaviour
    {
        // This is called on every game state enter (you decide to handle it or not)
        public abstract IEnumerator OnGameStateEnter(string enteringGameState, string exitingGameState);

        // This is called on every game state exit (you decide to handle it or not)
        public abstract IEnumerator OnGameStateExit(string exitingGameState, string enteringGameState);
    }

    public enum MightyGameStateTransitionStage
    {
        Immediate,
        AfterOnExit,
        AfterOnEnter,
    }

    [DefaultExecutionOrder(0)]
    public class MightyGameBrain : MonoBehaviour
    {
        private const string NONE_STATE_NAME = "None";

        private static MightyGameBrain instance;
        public static MightyGameBrain Instance { get { return instance; } }

        [BoxGroup("Info")] [ReadOnly] public string currentGameStateName = NONE_STATE_NAME; 
        [BoxGroup("Info")] [ReadOnly] public string previousGameStateName = NONE_STATE_NAME;
        [BoxGroup("Settings")] public MightyGameManager mightyGameManager;
        [BoxGroup("Settings")] public MightyStatesPreset mightyStatesPreset; 
        [BoxGroup("Debug")] public bool displayDebugInfo;

        private MightyGameState currentGameState;
        private Dictionary<string, MightyGameState> gameStateMap;
        private GUIStyle guiStyle = new GUIStyle();
        private (string exitingGameState, string enteringGameState)? currentTransition;

        [HideInInspector] public OnEnterGameStateStartedEvent onEnterGameStateStartedEvent;
        [HideInInspector] public OnEnterGameStateFinishedEvent onEnterGameStateFinishedEvent;
        [HideInInspector] public OnExitGameStateStartedEvent onExitGameStateStartedEvent;
        [HideInInspector] public OnExitGameStateFinishedEvent onExitGameStateFinishedEvent;

        void Awake()
        {
            instance = this;

            gameStateMap = new Dictionary<string, MightyGameState>();
            foreach (var gameState in mightyStatesPreset.gameStates)
            {
                gameState.Initialize();
                gameStateMap.Add(gameState.name, gameState);
            }

            onEnterGameStateStartedEvent = new OnEnterGameStateStartedEvent();
            onEnterGameStateFinishedEvent = new OnEnterGameStateFinishedEvent();
            onExitGameStateStartedEvent = new OnExitGameStateStartedEvent();
            onExitGameStateFinishedEvent = new OnExitGameStateFinishedEvent();
        }

        void Start()
        {
            // Validate mighty game manager reference
            if (mightyGameManager == null)
            {
                Abort("[MightyGameBrain] Mighty Game Manager reference is not set");
            }

            // Validate mighty states preset
            if (mightyStatesPreset == null)
            {
                Abort("[MightyGameBrain] Mighty States Preset is not set");
            }

            // Validate starting game state name
            if (string.IsNullOrEmpty(mightyStatesPreset.startingGameStateName))
            {
                Abort("[MightyGameBrain] Starting Game State Name is not set");
            }

            Initialize();
        }
 
#if UNITY_EDITOR
        void OnGUI()
        {
            if (displayDebugInfo)
            {
                guiStyle.fontSize = 9;
                guiStyle.normal.textColor = Color.white;

                // Game state info
                GUI.Label(new Rect(5, 5, 200, 20), "Game states:", guiStyle);
                GUI.Label(new Rect(15, 15, 200, 20), "Current: " + currentGameStateName, guiStyle);
                GUI.Label(new Rect(15, 25, 200, 20), "Previous: " + previousGameStateName, guiStyle);

                string currentTransitionText = currentTransition.HasValue ? string.Format("{0} -> {1}", currentTransition.Value.exitingGameState, currentTransition.Value.enteringGameState) : "None";
                GUI.Label(new Rect(5, 35, 300, 20), "Current transition: " + currentTransitionText, guiStyle);

                // Level info
                if (MightyLevelsManager.Instance != null)
                {
                    GUI.Label(new Rect(5, 55, 200, 20), "Current level: " + MightyLevelsManager.Instance.currentLevelName, guiStyle);
                }

                // Switch game state
                int offset = MightyUIManager.Instance != null ? 20 : 5;
                for (int i = 0; i < mightyStatesPreset.gameStates.Count; i++)
                {
                    MightyGameState gameState = mightyStatesPreset.gameStates[i];
                   
                    int yPosition = Screen.height - 15 * (mightyStatesPreset.gameStates.Count - i) - offset;

                    GUI.enabled = !currentTransition.HasValue && currentGameState.exitGameStateNames.Contains(gameState.name) && currentGameState != gameState;
                    if (GUI.Button(new Rect(5, yPosition - 1, 12, 12), ""))
                    {
                        TransitToNextGameState(gameState.name);
                    }
                    GUI.enabled = true;
                    GUI.Label(new Rect(20, yPosition, 200, 20), "◄ " + gameState.name, guiStyle);
                }

                // Selected UI element
                if (MightyUIManager.Instance != null)
                {
                    GameObject selectedUIObject = MightyUIManager.Instance.selectedUIObject;
                    GUI.Label(new Rect(5, Screen.height - 15, 200, 20), "Selected UI object: " + (selectedUIObject != null ? selectedUIObject.name : "None"), guiStyle);
                }
            }
        }
#endif

        void Initialize()
        {
            // Set starting game state
            MightyGameState startingGameState = gameStateMap[mightyStatesPreset.startingGameStateName];
            if (startingGameState != null)
            {
                StartCoroutine(TransitToNextGameStateInternal(mightyStatesPreset.startingGameStateName));
            }
            else
            {
                Abort(String.Format("[MightyGameBrain] Cannot set starting game state to \"{0}\", there is no game state named like this", mightyStatesPreset.startingGameStateName));
            }
        }

        public void TransitToNextGameState(string nextGameStateName)
        {
            if (!currentTransition.HasValue)
            {
                StartCoroutine(TransitToNextGameStateInternal(nextGameStateName));
            }
        }

        void ChangeCurrentState(MightyGameState exitingGameState, MightyGameState enteringGameState)
        {
            currentGameState = enteringGameState;
            currentGameStateName = enteringGameState.name;
            previousGameStateName = exitingGameState != null ? exitingGameState.name : NONE_STATE_NAME;
            currentTransition = null;
        }

        IEnumerator TransitToNextGameStateInternal(string nextGameStateName)
        {
            if (currentGameStateName == NONE_STATE_NAME) // Initial transition to start state happening initializaion of the game
            {
                MightyGameState enteringGameState = gameStateMap[nextGameStateName];
                currentTransition = (NONE_STATE_NAME, enteringGameState.name);
                ChangeCurrentState(null, enteringGameState);
                StartCoroutine(mightyGameManager.OnGameStateEnter(enteringGameState.name, NONE_STATE_NAME));
            }
            else
            {
                if (currentGameState.HasExitGameState(nextGameStateName))
                {
                    MightyGameState exitingGameState = currentGameState;
                    MightyGameState enteringGameState = gameStateMap[nextGameStateName];
                    MightyGameStateTransitionStage transitionStage = exitingGameState.transitionStage;
                    if (enteringGameState != null)
                    {
                        currentTransition = (exitingGameState.name, enteringGameState.name);

                        if (transitionStage == MightyGameStateTransitionStage.Immediate)
                        {
                            ChangeCurrentState(exitingGameState, enteringGameState);
                        }

                        onExitGameStateStartedEvent.Invoke(exitingGameState.name, enteringGameState.name);
                        yield return StartCoroutine(mightyGameManager.OnGameStateExit(exitingGameState.name, enteringGameState.name));
                        onExitGameStateFinishedEvent.Invoke(exitingGameState.name, enteringGameState.name);

                        if (transitionStage == MightyGameStateTransitionStage.AfterOnExit)
                        {
                            ChangeCurrentState(exitingGameState, enteringGameState);
                        }

                        onEnterGameStateStartedEvent.Invoke(enteringGameState.name, exitingGameState.name);
                        yield return StartCoroutine(mightyGameManager.OnGameStateEnter(enteringGameState.name, exitingGameState.name));
                        onEnterGameStateFinishedEvent.Invoke(enteringGameState.name, exitingGameState.name);

                        if (transitionStage == MightyGameStateTransitionStage.AfterOnEnter)
                        {
                            ChangeCurrentState(exitingGameState, enteringGameState);
                        }
                    }
                    else
                    {
                        Debug.LogError(string.Format("[MightyGameBrain] Cannot transit to game state \"{0}\", such game state does not exist", nextGameStateName));
                    }
                }
                else
                {
                    Debug.LogWarning(string.Format("[MightyGameBrain] Cannot transit to game state \"{0}\", current game state (\"{1}\") has not such exit game state set", nextGameStateName, currentGameStateName));
                }
            } 
        }

        public MightyGameState GetGameState(string gameStateName)
        {
            MightyGameState gameState;
            gameStateMap.TryGetValue(gameStateName, out gameState);
            return gameState;
        }

        public static void Abort(string message = "")
        {
            Debug.LogError(message);
            QuitGame();
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}