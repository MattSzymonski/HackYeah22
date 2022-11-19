/* 
NAME:
    Mighty Audio Manager

DESCRIPTION:


USAGE:
    Look under the code below 

TODO:
   There is bug when playing random sound multiple are being played (one by one?)
*/

using UnityEngine.Audio;
using UnityEngine;
using System;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;

namespace Mighty
{
    public enum SoundType
    {
        Effect,
        Music
    };

    [Serializable]
    public class MightySound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1.0f;
        [Range(0.1f, 3f)] public float pitch = 1.0f;

        public bool playOnAwake;
        public bool loop;

        public SoundType soundType;

        MightyAudioManager mightyAudioManager;

        public void Initialize()
        {
            mightyAudioManager = MightyAudioManager.Instance;
        }
             
        public void Play(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.pitch = pitch;
                audioSource.loop = loop;

                if (soundType == SoundType.Effect)
                {
                    audioSource.outputAudioMixerGroup = mightyAudioManager.effectsMixerGroup;
                }
                else if (soundType == SoundType.Music)
                {
                    audioSource.outputAudioMixerGroup = mightyAudioManager.musicMixerGroup;
                }

                audioSource.Play();
            }
        }

        public void Stop()
        {
            foreach (AudioSource audioSource in MightyAudioManager.Instance.audioSourcePool)
            {
                if (audioSource.clip.name == name)
                {
                    audioSource.Stop();
                }
            }
        }

        public bool IsPlaying()
        {
            foreach (AudioSource audioSource in MightyAudioManager.Instance.audioSourcePool)
            {
                if (audioSource.clip.name == name)
                {
                    return audioSource.isPlaying;
                }
            }

            return false;
        }
    }

    public class MightyAudioManager : MonoBehaviour
    {
        private static MightyAudioManager instance;
        public static MightyAudioManager Instance { get { return instance; } }

        [BoxGroup("Settings")] public int maxConcurrentSounds;
        [BoxGroup("Settings")] public AudioMixerGroup musicMixerGroup;
        [BoxGroup("Settings")] public AudioMixerGroup effectsMixerGroup;
        public List<MightySound> sounds;

        [HideInInspector] public List<AudioSource> audioSourcePool;

        void Awake()
        {
            instance = this;

            if (musicMixerGroup == null || effectsMixerGroup == null)
            {
                Debug.LogError("[MightyAudioManager] Sound mixer groups are not set");
            }
        }

        void Start()
        {
            // Create pool
            GameObject pool = new GameObject("AudioSourcePool");
            pool.transform.parent = transform;
            for (int i = 0; i < maxConcurrentSounds; i++)
            {
                audioSourcePool.Add(pool.AddComponent<AudioSource>());
            }

            // Initialize mighty sounds
            foreach (MightySound sound in sounds)
            {
                sound.Initialize();

                if (sound.playOnAwake)
                {
                    sound.Play(GetFreeAudioSource());     
                }
            }
        }

        public void PlaySound(string soundName)
        {
            GetSound(soundName)?.Play(GetFreeAudioSource());
        }

        public void StopSound(string soundName)
        {
            GetSound(soundName)?.Stop();
        }

        public void PlayRandomSound(params string[] soundNames)
        {
            string soundName = soundNames[UnityEngine.Random.Range(0, soundNames.Length)];
            MightySound sound = GetSound(soundName);
            sound?.Play(GetFreeAudioSource());
        }


        AudioSource GetFreeAudioSource()
        {
            AudioSource freeAudioSource = audioSourcePool.FirstOrDefault(audioSource => !audioSource.isPlaying);
            if (freeAudioSource == null)
            {
                Debug.LogWarning("[MightyAudioManager] Cannot play sound, concurrent playing sound limit reached");
                return null;
            }

            return freeAudioSource;
        }

        MightySound GetSound(string soundName)
        {
            MightySound sound = sounds.FirstOrDefault(sonud => sonud.name == soundName);
            if (sound == null)
            {
                Debug.LogWarning("[MightyAudioManager] Sound: " + soundName + " not found!");
                return null;
            }

            return sound;
        }


        public void SetMusicMixerVolume(float sliderValue)
        {
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void SetEffectsMixerVolume(float sliderValue)
        {
            effectsMixerGroup.audioMixer.SetFloat("EffectsVolume", Mathf.Log10(sliderValue) * 20);
        }
    }
}

/* 
USAGE:
    Add this component to GameObject in Hierarchy
    Add sounds to it in inspector
    audioManager.PlaySound("SoundName"); // Play sound named "SoundName" in inspector
    audioManager.StopSound("SoundName"); // Stop playing sound named "SoundName" in inspector
    audioManager.PlayRandomSound("Sound1", "Sound2", "Sound3",) // Randomly choose and play one of sounds passed as parameter
    
*/
