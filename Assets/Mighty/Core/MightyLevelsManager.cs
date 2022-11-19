using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mighty
{
    public class MightyLevelsManager : MonoBehaviour
    {
        private const string NONE_LEVEL_NAME = "None";

        private static MightyLevelsManager instance;
        public static MightyLevelsManager Instance { get { return instance; } }

        [BoxGroup("Info")] [ReadOnly] public string currentLevelName = NONE_LEVEL_NAME;
        [BoxGroup("Info")] [ReadOnly] public string previousLevelName = NONE_LEVEL_NAME;
        [HideInInspector] public MightyLevel currentLevel;

        [BoxGroup("Settings")] public MightyLevelsPreset mightylevelsPreset;

        [BoxGroup("Fill Level Menu")] public bool fillLevelMenu;
        [BoxGroup("Fill Level Menu")] [ShowIf("fillLevelMenu")] public GridLayoutGroup gridLayoutGroup;
        [BoxGroup("Fill Level Menu")] [ShowIf("fillLevelMenu")] public GameObject levelButtonPrefab;

        private Dictionary<string, MightyLevel> levelMap;

        void Awake()
        {
            instance = this;

            levelMap = new Dictionary<string, MightyLevel>();
            foreach (var level in mightylevelsPreset.levels)
            {
                level.Initialize();
                levelMap.Add(level.name, level);
            }

            // Validate fill level menu
            if (fillLevelMenu)
            {
                if (gridLayoutGroup == null)
                {
                    Debug.LogError("[MightyLevelsManager] Grid Layout Group reference is not set");
                }

                if (levelButtonPrefab == null)
                {
                    Debug.LogError("[MightyLevelsManager] Level button prefab is not set");
                }
            }
        }

        void Start()
        {
            // Create level buttons
            if (fillLevelMenu)
            {
                foreach (MightyLevel level in mightylevelsPreset.levels)
                {
                    GameObject levelButton = GameObject.Instantiate(levelButtonPrefab, Vector3.zero, Quaternion.identity);
                    levelButton.transform.GetChild(0).transform.GetComponent<Text>().text = level.name; // Set text
                    Button button = levelButton.GetComponent<Button>();
                    button.onClick.AddListener(() => LoadLevel(level.name)); // Set action
                    // TODO Set game pad navigation
                    levelButton.GetComponent<RectTransform>().SetParent(gridLayoutGroup.GetComponent<RectTransform>()); // Set parent
                }
            }
        }

        public void LoadLevel(string levelName)
        {
            MightyLevel level = levelMap[levelName];
            if (level != null)
            {
                currentLevel.Unload();
                level.Load();
                previousLevelName = currentLevel.name;
                currentLevel = level;
                currentLevelName = level.name;
            }
            else
            {
                Debug.LogError(string.Format("[MightyLevelManager] Cannot load level \"{0}\", such level does not exist", levelName));
            }
        }
    }
}