using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

namespace Mighty 
{
    [System.Serializable]
    public class VFX
    {
        public string name;
        public GameObject prefab;
    }

    public class VFXDestroyer : MonoBehaviour
    {
        [ReadOnly] public float timeToDestroy;
        [ReadOnly] public float timer;

        void Update()
        {
            if(timer < timeToDestroy)
            {
                timer += Time.unscaledDeltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class MightyVFXManager : MonoBehaviour
    {
        private static MightyVFXManager instance;
        public static MightyVFXManager Instance { get { return instance; } }

        [Label("VFXes")] [ReorderableList] public VFX[] VFXes;
        [ReadOnly] [Label("Active VFXes")] public List<GameObject> activeVFXes;

        float cleanActiveVFXListTimer;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            CleanActiveVFXList();
        }

        public void SpawnVFX(Vector3 position, Quaternion rotation, float timeToDestroy, float spawnDelay, string vfxName)
        {
            VFX vfx = Array.Find(VFXes, x => x.name == vfxName);
            if (vfxName == null)
            {
                Debug.LogWarning("[MightyVFXManger] VFX: " + vfxName + " not found!");
                return;
            }

            StartCoroutine(Spawn(vfx, position, rotation, timeToDestroy, spawnDelay));
        }

        public void SpawnRandomVFX(Vector3 position, Quaternion rotation, float timeToDestroy, float spawnDelay, params string[] vfxNames)
        {
            string vfxName = vfxNames[UnityEngine.Random.Range(0, vfxNames.Length)];

            VFX vfx = Array.Find(VFXes, x => x.name == vfxName);
            if (vfxName == null)
            {
                Debug.LogWarning("[MightyVFXManger] Randomized VFX: " + vfxName + " not found!");
                return;
            }

            StartCoroutine(Spawn(vfx, position, rotation, timeToDestroy, spawnDelay));
        }


        IEnumerator Spawn(VFX vfx, Vector3 position, Quaternion rotation, float timeToDestroy, float spawnDelay)
        {
            yield return new WaitForSeconds(spawnDelay);
            GameObject vfxObject = Instantiate(vfx.prefab, position, rotation) as GameObject;
            VFXDestroyer particleEffectDestroyer = vfxObject.AddComponent<VFXDestroyer>();
            particleEffectDestroyer.timeToDestroy = timeToDestroy;

            vfxObject.transform.parent = gameObject.transform;
            activeVFXes.Add(vfxObject);
        }

        public void DestroyAllVFXes()
        {
            for (int i = activeVFXes.Count - 1; i >= 0; i--)
            {
                GameObject objectToDestroy = activeVFXes[i];
                activeVFXes.RemoveAt(i);
                if(objectToDestroy != null)
                {
                    Destroy(objectToDestroy);
                }
            }
        }

        void CleanActiveVFXList()
        {
            if (cleanActiveVFXListTimer < 3)
            {
                cleanActiveVFXListTimer += Time.deltaTime;
                for (int i = activeVFXes.Count - 1; i >= 0; i--)
                {
                    if (activeVFXes[i] == null)
                    {
                        activeVFXes.RemoveAt(i);
                    }
                }
                cleanActiveVFXListTimer = 0;
            }
        }
    }
}
