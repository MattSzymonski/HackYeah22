/* 
NAME:
    Text Typer

DESCRIPTION:
    Script that creates effect of text typing letter by letter over time
   
USAGE:
   Add component to any object
   set text UI variable and call: StartTyping("Hello World!");

TODO:
   
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;



namespace Mighty
{
    public class TextTyper : MonoBehaviour
    {
        public Text text;
        public float typingSpeed = 10;
        [ReadOnly] [SerializeField] bool active;
        [ReadOnly] [SerializeField] [TextArea] string message;
        [SerializeField] float typingProgress;

        int currentCharacterIndex;
        float currentTime;


        void Start()
        {
            if (!text) { Debug.LogWarning("No text reference set in: " + this); }
        }

        void Update()
        {
            Type();
        }

        public void StartTyping(string message)
        {
            currentTime = 0;           
            this.message = message;
            text.text = "";
            currentCharacterIndex = 0;
            active = true;
        }

        public void Type()
        {
            if(active)
            {
                if (currentCharacterIndex < message.Length)
                {
                    if (currentTime < 0.1f)
                    {
                        currentTime += typingSpeed * Time.deltaTime;
                    }
                    else
                    {
                        currentCharacterIndex++;
                        text.text = message.Substring(0, currentCharacterIndex);
                        typingProgress = ((float)currentCharacterIndex / (float)(message.Length)) * 100;
                        currentTime = 0;
                    }
                }
                else
                {
                    active = false;
                    currentTime = 0;
                }
            }
        }

        public void PauseTyping()
        {
            active = false;
        }

        public void ResumeTyping()
        {
            active = true;
        }

        public void ClearText()
        {
            text.text = "";
        }

        public void StopAndClearText()
        {
            active = false;
            text.text = "";
        }

    }
}
