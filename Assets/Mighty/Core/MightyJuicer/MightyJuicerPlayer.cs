using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mighty 
{
    public class MightyJuicerPlayer : MonoBehaviour
    {
        [SerializeReference] public List<MightyJuicer> juicers = new List<MightyJuicer>();
        private Dictionary<string, MightyJuicer> juicerMap;
      
        void Start()
        {
            juicerMap = new Dictionary<string, MightyJuicer>();
            foreach (var juicer in juicers)
            {
                // Validate name
                if (string.IsNullOrEmpty(juicer.name))
                {
                    MightyGameBrain.Abort("[MightyLevel] Name of juicer cannot be empty");
                }

                juicer.Initialize(gameObject);
                juicerMap.Add(juicer.name, juicer);
                if (juicer.playOnAwake)
                {
                    juicer.Play();
                }
            }
        }

        void Update()
        {
            UpdateJuicers();
        }

        public void PlayJuicer(string juicerName)
        {
            GetJuicer(juicerName)?.Play();
        }

        public void StopJuicer(string juicerName)
        {
            GetJuicer(juicerName)?.Stop();
        }

        public void RestartJuicer(string juicerName)
        {
            GetJuicer(juicerName)?.Restart();
        }

        public void ResetJuicer(string juicerName)
        {
            GetJuicer(juicerName)?.Reset();
        }

        MightyJuicer GetJuicer(string juicerName)
        {
            MightyJuicer juicer = juicerMap[juicerName];
            if (juicer != null)
            {
                return juicer;
            }

            Debug.LogError(string.Format("[MightyJuicerPlayer] Cannot find MightyJuicer \"{0}\"", juicerName));
            return null;
        }

        void UpdateJuicers()
        {
            foreach (MightyJuicer juicer in juicers)
            {
                juicer.Update();
            }
        }
    }
}
