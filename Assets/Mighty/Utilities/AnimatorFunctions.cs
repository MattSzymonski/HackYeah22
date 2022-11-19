using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script that makes it super easy to trigger various functions, effects, sounds, etc from animations
//Add this script to the GameObject in which animator component is added, in animation used by this animator click "Add Event" and functions in this file will be available to choose for that event
//Functions will be triggered when animation will approach the time in which event is set
namespace Mighty
{
    public class AnimatorFunctions : MonoBehaviour
    {
        //!!! ADD PROJECT FUNCTIONS YOU WANT TO TRIGGER FROM ANIMATIONS HERE !!!

        //Exemplary usage: (this will print "Hello" each time this funtion will be called from animation)
        //public void Hello()
        //{
        //    Debug.Log("Hello");
        //}
    }
}
