using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mighty
{
    [Serializable]
    public class MightyUIPanel
    {
        public string name;
        public GameObject gameObject;
        public Selectable defaultSelectObject; // UI element to select when gamepad, WASD or arrow navigation is detected and this panel is active

        [ReadOnly] public bool active;
        private Animator animator;
        private MightyUIManager mightyUIManager;

        public void Initialize()
        {
            // Validate name
            if (string.IsNullOrEmpty(name))
            {
                MightyGameBrain.Abort("[MightyUIPanel] Name is not set");
            }

            // Validate gameObject reference
            if (gameObject == null)
            {
                MightyGameBrain.Abort("[MightyUIPanel: \"{0}\"] Gameobject reference is not set");
            }

            gameObject.SetActive(true);
            animator = gameObject.GetComponent<Animator>();

            mightyUIManager = MightyUIManager.Instance;
        }

        public void Update()
        { 
            // Select UI element when gamepad, WASD or arrow navigation is detected, nothing else is selected and this panel is active 
            if (defaultSelectObject != null && active && mightyUIManager.uiControlMethod != UIControlMethod.Mouse && mightyUIManager.selectedUIObject == null)
            {
                mightyUIManager.SelectNoSound(defaultSelectObject);
            }
        }

        public bool IsTransitionCompleted(string stateName)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(stateName) && state.normalizedTime >= 1;
        }

        public bool IsOpened()
        {
            return animator.GetBool("Opened");
        }

        public void TriggerTransition(string parameterName, bool parameterValue)
        {
            if (animator)
            {
                animator.SetBool(parameterName, parameterValue);
            }
            else
            {
                Debug.LogError(string.Format("[MightyUIPanel: \"{0}\"] Cannot trigger UI panel transition, no animator detected in UI panel gameobject \"{1}\"", name, gameObject.name));
            }
        }
    }
}

