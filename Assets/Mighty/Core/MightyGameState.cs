using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mighty
{
    [Serializable]
    public class MightyGameState
    {
        [AllowNesting] [BoxGroup("Options")] public string name;

        [AllowNesting] [BoxGroup("Options")] [Tooltip("Defines when game state changes when transiting from this state to the next one")] 
        public MightyGameStateTransitionStage transitionStage;

        [AllowNesting] [BoxGroup("Connections")] public List<string> enterGameStateNames;
        [AllowNesting] [BoxGroup("Connections")] public List<string> exitGameStateNames;
        [NonSerialized] private List<MightyGameState> enterGameStates;
        [NonSerialized] private List<MightyGameState> exitGameStates;

        public void Initialize()
        {
            // Validate name
            if (string.IsNullOrEmpty(name))
            {
                MightyGameBrain.Abort("[MightyGameState] Name of game state cannot be empty");
            }

            // Reference enter game states in MightyGameBrain
            enterGameStates = new List<MightyGameState>();
            foreach (string enterGameStateName in enterGameStateNames)
            {
                MightyGameState enterGameState = MightyGameBrain.Instance.mightyStatesPreset.gameStates.FirstOrDefault(x => x.name == enterGameStateName);
                if (enterGameState != null)
                {
                    enterGameStates.Add(enterGameState);
                    continue;
                }

                MightyGameBrain.Abort(string.Format("[MightyGameState: \"{0}\"] EnterGameStateName \"{1}\" does not exist in MightyGameBrain", name, enterGameStateName));
            }

            // Reference exit game states in MightyGameBrain
            exitGameStates = new List<MightyGameState>();
            foreach (string exitGameStateName in exitGameStateNames)
            {
                MightyGameState exitGameGameState = MightyGameBrain.Instance.mightyStatesPreset.gameStates.FirstOrDefault(x => x.name == exitGameStateName);
                if (exitGameGameState != null)
                {
                    exitGameStates.Add(exitGameGameState);
                    continue;
                }

                MightyGameBrain.Abort(string.Format("[MightyGameState: \"{0}\"] ExitGameStateName \"{1}\" does not exist in MightyGameBrain", name, exitGameStateName));
            }

            // Validate if enter state has corresponding entry state
            foreach (MightyGameState enterGameState in enterGameStates)
            {
                if (!enterGameState.exitGameStateNames.Contains(name))
                {
                    MightyGameBrain.Abort(string.Format("[MightyGameState] MightyGameState \"{0}\" does not have corresponding ExitGameState \"{1}\"", enterGameState.name, name));
                }
            }

            // Validate if exit state has corresponding entry state
            foreach (MightyGameState exitGameState in exitGameStates)
            {
                if (!exitGameState.enterGameStateNames.Contains(name)) {
                    MightyGameBrain.Abort(string.Format("[MightyGameState] MightyGameState \"{0}\" does not have corresponding EnterGameState \"{1}\"", exitGameState.name, name));
                }
            }
        }

        public bool HasEnterGameState(string enterStateName)
        {
            return enterGameStateNames.Contains(enterStateName);
        }

        public bool HasExitGameState(string exitStateName)
        {
            return exitGameStateNames.Contains(exitStateName);
        }
    }
}
