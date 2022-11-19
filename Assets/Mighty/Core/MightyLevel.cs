using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mighty
{
    public enum LevelType
    {
        Prefab,
        Scene,
    }

    [Serializable]
    public class MightyLevel
    {
        public string name;
        public LevelType type;

        [AllowNesting] [BoxGroup("Settings")] [ShowIf("isPrefabType")] public GameObject prefab;
        [HideInInspector] public GameObject loadedPrefab;

        [AllowNesting] [BoxGroup("Settings")] [ShowIf("isSceneType")] public string sceneName;
        [AllowNesting] [BoxGroup("Settings")] [ShowIf("isSceneType")] public LoadSceneMode loadSceneMode;
        [HideInInspector] public Scene? loadedScene;

        [ReadOnly] public bool active;

        bool isPrefabType => type == LevelType.Prefab;
        bool isSceneType => type != LevelType.Prefab;

        public void Initialize()
        {
            // Validate name
            if (string.IsNullOrEmpty(name))
            {
                MightyGameBrain.Abort("[MightyLevel] Name of level cannot be empty");
            }
        }

        public void Load()
        {
            if (type == LevelType.Prefab)
            {
                LoadPrefab();
            }
            else
            {
                LoadScene();
            }
        }

        async void LoadScene()
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene.name, loadSceneMode);
            do
            {
                await Task.Delay(100);
            } while (loadOperation.progress < 0.9f);

            loadedScene = scene;
            active = true;
        }

        void LoadPrefab()
        {
            GameObject levelPrefab = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            loadedPrefab = levelPrefab;
            active = true;
        }



        public void Unload()
        {
            active = false;
            if (type == LevelType.Prefab)
            {
                UnloadPrefab();
            }
            else
            {
                UnloadScene();
            }
        }

        void UnloadPrefab()
        {
            active = false;
            GameObject.Destroy(loadedPrefab);
            active = false;
        }

        async void UnloadScene()
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadedScene.Value.name);
            do
            {
                await Task.Delay(100);
            } while (unloadOperation.progress < 0.9f);

            loadedScene = null;
            active = false;
        }
    }
}