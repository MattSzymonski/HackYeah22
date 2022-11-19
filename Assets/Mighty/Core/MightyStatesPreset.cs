using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mighty
{
    [CreateAssetMenu(fileName = "MightyStatesPreset", menuName = "Mighty/MightyStatesPreset", order = 1)]
    public class MightyStatesPreset : ScriptableObject
    {
        [BoxGroup("")] [ResizableTextArea] public string info; 
        [BoxGroup("Settings")] public string startingGameStateName;
        [BoxGroup("GameStates")] public List<MightyGameState> gameStates;
    }
}

