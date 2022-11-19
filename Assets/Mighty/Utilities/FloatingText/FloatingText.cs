/* 
NAME:
    Floating Text

DESCRIPTION:
    Script that spawn floating texts. Can be used for example as damage points popup or people shouting effect
    Add component to any object, set floatingTextPrefab (you can edit it or create your onw) and call: SpawnFloatingText("Hello World!");

USAGE:
    Look under the code below 

TODO:
    Object pooling can be added
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mighty
{
    class FloatingTextObject
    {
        public GameObject gameObject;
        public float currentLifetime = 0;
        public float lifetimeLength = 0;
    }


    public class FloatingText : MonoBehaviour
    {
        public GameObject floatingTextPrefab;
        public Vector3 spawnPositionOffset = new Vector3(0, 2, 0);
        public Vector3 spawnPositionRandomize = new Vector3(0.5f, 0.3f, 0);
        List<FloatingTextObject> aliveFloatingTextObjects;

        void Start()
        {
            aliveFloatingTextObjects = new List<FloatingTextObject>();
        }

        void Update()
        {
            UpdateSpawnFloatingTextObjects();
        }

        public void SpawnFloatingText(string message, float lifetimeLength)
        {
            if (floatingTextPrefab)
            {
                FloatingTextObject newFloatingTextObject = new FloatingTextObject();
                newFloatingTextObject.gameObject = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform) as GameObject;
                newFloatingTextObject.gameObject.GetComponent<TextMesh>().text = message;
                newFloatingTextObject.gameObject.transform.localPosition += spawnPositionOffset;
                newFloatingTextObject.gameObject.transform.localPosition += new Vector3(Random.Range(-spawnPositionRandomize.x, spawnPositionRandomize.x), Random.Range(-spawnPositionRandomize.y, spawnPositionRandomize.y), Random.Range(-spawnPositionRandomize.z, spawnPositionRandomize.z));       
                newFloatingTextObject.lifetimeLength = lifetimeLength;
                aliveFloatingTextObjects.Add(newFloatingTextObject);
            }
            else
            {
                Debug.LogError("Cannot spawn floating text because floating text prefab is not assigned in: " + this);
            }
        }

        void UpdateSpawnFloatingTextObjects()
        {
            for (int i = aliveFloatingTextObjects.Count - 1; i >= 0; i--)
            {
                if(aliveFloatingTextObjects[i].currentLifetime > aliveFloatingTextObjects[i].lifetimeLength)
                {
                    Destroy(aliveFloatingTextObjects[i].gameObject);
                    aliveFloatingTextObjects.RemoveAt(i);
                }
                else
                {
                    aliveFloatingTextObjects[i].currentLifetime += Time.deltaTime;
                }
            }
        }
    }
}

/* 
USAGE:
    GetComponent<FloatingText>().SpawnFloatingText("MyText", 3);
    if (Input.GetKeyDown(KeyCode.I)) { GameObject.Find("GameManager").GetComponent<FloatingText>().SpawnFloatingText("Hello", 3); }
*/
