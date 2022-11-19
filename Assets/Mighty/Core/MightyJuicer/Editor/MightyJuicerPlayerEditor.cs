using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Mighty
{
    [CustomEditor(typeof(MightyJuicerPlayer))]
    public class MightyJuicerPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MightyJuicerPlayer targetPlayer = (MightyJuicerPlayer)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Add Position Juicer"))
            {
                targetPlayer.juicers.Add(new MightyJuicerPosition("Position"));
            }

            if (GUILayout.Button("Add Rotation Juicer"))
            {
                targetPlayer.juicers.Add(new MightyJuicerRotation("Rotation"));
            }

            if (GUILayout.Button("Add Scale Juicer"))
            {
                targetPlayer.juicers.Add(new MightyJuicerScale("Scale"));
            }

            if (GUILayout.Button("Add Rect Scale Juicer"))
            {
                targetPlayer.juicers.Add(new MightyJuicerRectScale("Rect Scale"));
            }
        }
    }
}
