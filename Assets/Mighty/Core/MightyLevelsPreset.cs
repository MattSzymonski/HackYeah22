using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mighty
{
    // Levels preset
    [CreateAssetMenu(fileName = "MightyLevelsPreset", menuName = "Mighty/MightyLevelsPreset", order = 2)]
    public class MightyLevelsPreset : ScriptableObject
    {
        [BoxGroup("")] [ResizableTextArea] public string info;
        [BoxGroup("Levels")] public List<MightyLevel> levels;
    }
}
