/* 
NAME:
    Mighty UI Manager

DESCRIPTION:
    Manager responsible for handling UI transitions

USAGE:
    Add this script to a single object on the scene and specify UI panels in the list (uiPanels parameter)
    In the root, each panel should have Canvas Group component and animator with three states: "InitialClosed", "Open" and "Close", with single parameter "Open" driving the transitions (check the templates)
    Idealy these animator should be animating "alpha" value and "Blocks Raycasts" checkbox of Canvas Group component of the Panel

TODO:
   
*/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Mighty
{
    public enum UIControlMethod
    {
        Mouse,
        WASD,
        Arrows,
        Gamepad
    };

    public class MightyUIManager : MonoBehaviour
    {
        public static readonly string[] MOUSE_CONTROL_AXES_NAMES = { "Mouse X", "Mouse Y", "Mouse ScrollWheel" };
        public static readonly string[] MOUSE_CONTROL_BUTTON_NAMES = { "LMB", "RMB" };
        public static readonly string[] GAMEPAD_AXES_NAMES = { "ControllerAny Left Stick Horizontal UI Sensitivity", "ControllerAny Left Stick Vertical UI Sensitivity" };
        public static readonly string[] GAMEPAD_BUTTON_NAMES = { "ControllerAny A", "ControllerAny B", "ControllerAny X", "ControllerAny Y", "ControllerAny Start" };
        public static readonly KeyCode[] WASD_KEYS = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        public static readonly KeyCode[] ARROW_KEYS = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        private static MightyUIManager instance;
        public static MightyUIManager Instance { get { return instance; } }

        [BoxGroup("Info")] [ReadOnly] [Tooltip("Automatically detected currently used input method (Mouse, WASD, Arrows, Gamepad)")] [Label("UI Control Method")] public UIControlMethod uiControlMethod;
        [BoxGroup("Info")] [ReadOnly] [Tooltip("Currently selected object of UI")] public GameObject selectedUIObject;

        [BoxGroup("Settings")] public EventSystem eventSystem;
        [BoxGroup("Settings")] public bool disableButtonSounds;
        [BoxGroup("Settings")] [Tooltip("Display gamejam logo in main menu")] public bool displayGameJamLogo;

        [BoxGroup("Resolution Settings")] public bool enableResolutionChanging;
        [BoxGroup("Resolution Settings")] [ShowIf("enableResolutionChanging")] public Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [BoxGroup("Game Jam Logo")] [ShowIf("displayGameJamLogo")] public GameObject gameJamLogoObject;
        [BoxGroup("Game Jam Logo")] [ShowIf("displayGameJamLogo")] public Sprite gameJamLogoSprite;
        [BoxGroup("Game Jam Logo")] [ShowIf("displayGameJamLogo")] [Tooltip("Eg. http://unity3d.com/")] public string gameJamSiteURL;

        [BoxGroup("UI Panels")] [Label("UI Panels")] public List<MightyUIPanel> uiPanels;

        private Dictionary<string, MightyUIPanel> uiPanelMap;

        void Awake()
        {
            instance = this;

            uiPanelMap = new Dictionary<string, MightyUIPanel>();
            foreach (var uiPanel in uiPanels)
            {
                uiPanel.Initialize();
                uiPanelMap.Add(uiPanel.name, uiPanel);
            }

            SetUpResolutionOptions();

            MightyGameBrain.Instance.onExitGameStateFinishedEvent.AddListener(DeselectUI);
        }

        void Start()
        {
            // Validate event system reference
            if (eventSystem == null)
            {
                MightyGameBrain.Abort("[MightyUIManager] Event system reference is not set");
            }

            uiControlMethod = UIControlMethod.Mouse;
        }

        void Update()
        {
            UpdateUIPanels();
            DetectUIControlMethod();
            selectedUIObject = eventSystem.currentSelectedGameObject;
        }

        // Find available resolutions in player to set them to resolution dropdown in UI
        void SetUpResolutionOptions()
        {
            if (enableResolutionChanging)
            {
                resolutions = Screen.resolutions;
                resolutionDropdown.ClearOptions();

                int currentResolutionIndex = 0;
                List<string> options = new List<string>();
                for (int i = 0; i < resolutions.Length; i++)
                {
                    options.Add(resolutions[i].width + " x " + resolutions[i].height);
                    currentResolutionIndex = resolutions[i].Equals(Screen.currentResolution) ? i : 0;
                }

                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }
        }
        

        // --- Panel Handling ---	

        #region PanelHandling

        // This function works only for UI panels with just two states named "Open" and "Close", and one parameter driving the transition between them named "Open"
        public IEnumerator SwitchUIPanel(string uiPanelName, bool wait = false)
        {
            MightyUIPanel uiPanel = uiPanelMap[uiPanelName];
            if (uiPanel != null)
            {
                bool newState = !uiPanel.IsOpened();
                StartCoroutine(ToggleUIPanel(uiPanelName, newState, wait));
                yield return null;
            }
            else
            {
                Debug.LogError(string.Format("[MightyUIManager] Cannot switch UI panel \"{0}\", such UI panel does not exist", uiPanelName));
            }
        }

        public void CloseUIPanelForce(string uiPanelName)
        {
            MightyUIPanel uiPanel;
            if (uiPanelMap.TryGetValue(uiPanelName, out uiPanel))
            {
                uiPanel.TriggerTransition("Open", false);
                uiPanel.active = false;
            }
            else
            {
                Debug.LogError(string.Format("[MightyUIManager] Cannot toggle UI panel \"{0}\", such UI panel does not exist", uiPanelName));
            }
        }

        public IEnumerator ToggleUIPanel(string uiPanelName, bool open, bool wait = false)
        {
            MightyUIPanel uiPanel;
            if(uiPanelMap.TryGetValue(uiPanelName, out uiPanel))
            {
                uiPanel.TriggerTransition("Open", open);

                if (wait)
                {
                    string stateToWaitFor = open ? "Open" : "Close";
                    yield return new WaitUntil(() => uiPanel.IsTransitionCompleted(stateToWaitFor));
                }
                uiPanel.active = open;
            }
            else
            {
                Debug.LogError(string.Format("[MightyUIManager] Cannot toggle UI panel \"{0}\", such UI panel does not exist", uiPanelName));
            }
        }

        public void SetUIPanelParameter(string uiPanelName, string parameterName, bool parameterValue)
        {
            MightyUIPanel uiPanel = uiPanelMap[uiPanelName];
            if (uiPanel != null)
            {
                uiPanel.TriggerTransition(parameterName, parameterValue);
            }
            else
            {
                Debug.LogError(string.Format("[MightyUIManager] Cannot set UI panel parameter \"{0}\", such UI panel does not exist", uiPanelName));
            }
        }

        public bool IsUIPanelSwitched(string uiPanelName, string stateName)
        {
            MightyUIPanel uiPanel = uiPanelMap[uiPanelName];
            return uiPanel.IsTransitionCompleted(stateName);
        }

        void UpdateUIPanels()
        {
            foreach (MightyUIPanel uiPanel in uiPanels)
            {
                uiPanel.Update();
            }
        }

        #endregion PanelHandling

        // --- Misc ---	

        #region Misc

        void DetectUIControlMethod()
        {
            if (uiControlMethod != UIControlMethod.Mouse)
            {
                if (MOUSE_CONTROL_AXES_NAMES.Any(axisName => Input.GetAxis(axisName) != 0) || MOUSE_CONTROL_BUTTON_NAMES.Any(buttonName => Input.GetButton(buttonName)))
                {
                    uiControlMethod = UIControlMethod.Mouse;
                    return;
                }
            }

            if (uiControlMethod != UIControlMethod.Gamepad)
            {
                if (GAMEPAD_AXES_NAMES.Any(axisName => Input.GetAxis(axisName) != 0) || GAMEPAD_BUTTON_NAMES.Any(buttonName => Input.GetButton(buttonName)))
                {
                    uiControlMethod = UIControlMethod.Gamepad;
                    return;
                }
            }

            if (uiControlMethod != UIControlMethod.WASD)
            {
                if (WASD_KEYS.Any(key => Input.GetKey(key)))
                {
                    uiControlMethod = UIControlMethod.WASD; 
                    return;
                }
            }

            if (uiControlMethod != UIControlMethod.Arrows)
            {
                if (ARROW_KEYS.Any(key => Input.GetKey(key)))
                {
                    uiControlMethod = UIControlMethod.Arrows;
                    return;
                }
            }
        }

        public MightyUIPanel GetUIPanel(string uiPanelName)
        {
            MightyUIPanel uiPanel;
            uiPanelMap.TryGetValue(uiPanelName, out uiPanel);
            return uiPanel;
        }

        // Make sure that no button or any other UI element is selected (Prevent having selected "play" or "restart" button while in playing mode)
        public void DeselectUI() 
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Deselect run on OnExitGameStateFinished
        void DeselectUI(string exitingGameState, string enteringGameState)
        {
            DeselectUI();
        }

        public void SelectNoSound(Selectable uiObject)
        {
            if (disableButtonSounds)
            {
                uiObject.Select();
            }
            else
            {
                disableButtonSounds = true;
                uiObject.Select();
                disableButtonSounds = false;
            }
        }

        public void UIPlaySound(string soundName)
        {
            if (!disableButtonSounds)
            {
                MightyAudioManager.Instance.PlaySound(soundName);
            }
        }
       
        public void OpenGameJamSite()
        {
            if (!string.IsNullOrEmpty(gameJamSiteURL))
            {
                Application.OpenURL(gameJamSiteURL);
            }
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFullscreen(bool isFullscreen)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[MightyUIManager] Cannot set fullscreen in editor");
#else
            Screen.fullScreen = isFullscreen;   
#endif
        }

        #endregion Misc

        // --- Mighty API ---

        // UI components should use these proxy functions instead of calling these methods on referenced managers directly.
        // This allows to keep the references when saving whole UI hierarchy as one prefab (assuming that MightyUIManager is in this hierarchy too)
        #region MightyGameBrainAPI 

        public void TransitToNextGameState(string nextGameStateName)
        {
            MightyGameBrain.Instance.TransitToNextGameState(nextGameStateName);
        }

        public void TransitToPreviousGameState()
        {
            MightyGameBrain.Instance.TransitToNextGameState(MightyGameBrain.Instance.previousGameStateName);
        }

        public void QuitGame()
        {
            MightyGameBrain.QuitGame();
        }

        public void SetMusicMixerVolume(float volume)
        {
            MightyAudioManager.Instance.SetMusicMixerVolume(volume);
        }

        public void SetEffectsMixerVolume(float volume)
        {
            MightyAudioManager.Instance.SetEffectsMixerVolume(volume);
        }

        #endregion MightyGameBrainAPI
    }
}