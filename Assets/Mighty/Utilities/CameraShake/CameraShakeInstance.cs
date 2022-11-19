using UnityEngine;

namespace Mighty
{
    public enum CameraShakeState { FadingIn, FadingOut, Sustained, Inactive }

    public class CameraShakeInstance
    {

        // The intensity of the shake. It is recommended that you use ScaleMagnitude to alter the magnitude of a shake.
        public float Magnitude;

        
        // Roughness of the shake. It is recommended that you use ScaleRoughness to alter the roughness of a shake.     
        public float Roughness;

        
        // How much influence this shake has over the local position axes of the camera.       
        public Vector3 PositionInfluence;

        
        // How much influence this shake has over the local rotation axes of the camera.      
        public Vector3 RotationInfluence;

        
        // Should this shake be removed from the CameraShakeInstance list when not active?      
        public bool DeleteOnInactive = true;


        float roughMod = 1, magnMod = 1;
        float fadeOutDuration, fadeInDuration;
        bool sustain;
        float currentFadeTime;
        float tick = 0;
        Vector3 amt;

        
        // Will create a new instance that will shake once and fade over the given number of seconds.
        public CameraShakeInstance(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
        {
            this.Magnitude = magnitude;
            fadeOutDuration = fadeOutTime;
            fadeInDuration = fadeInTime;
            this.Roughness = roughness;
            if (fadeInTime > 0)
            {
                sustain = true;
                currentFadeTime = 0;
            }
            else
            {
                sustain = false;
                currentFadeTime = 1;
            }

            tick = Random.Range(-100, 100);
        }

        
        // Will create a new instance that will start a sustained shake.
        public CameraShakeInstance(float magnitude, float roughness)
        {
            this.Magnitude = magnitude;
            this.Roughness = roughness;
            sustain = true;

            tick = Random.Range(-100, 100);
        }

        public Vector3 UpdateShake()
        {
            amt.x = Mathf.PerlinNoise(tick, 0) - 0.5f;
            amt.y = Mathf.PerlinNoise(0, tick) - 0.5f;
            amt.z = Mathf.PerlinNoise(tick, tick) - 0.5f;

            if (fadeInDuration > 0 && sustain)
            {
                if (currentFadeTime < 1)
                    currentFadeTime += Time.deltaTime / fadeInDuration;
                else if (fadeOutDuration > 0)
                    sustain = false;
            }

            if (!sustain)
                currentFadeTime -= Time.deltaTime / fadeOutDuration;

            if (sustain)
                tick += Time.deltaTime * Roughness * roughMod;
            else
                tick += Time.deltaTime * Roughness * roughMod * currentFadeTime;

            return amt * Magnitude * magnMod * currentFadeTime;
        }

        
        // Starts a fade out over the given number of seconds.
        public void StartFadeOut(float fadeOutTime)
        {
            if (fadeOutTime == 0)
                currentFadeTime = 0;

            fadeOutDuration = fadeOutTime;
            fadeInDuration = 0;
            sustain = false;
        }

        
        // Starts a fade in over the given number of seconds.
        public void StartFadeIn(float fadeInTime)
        {
            if (fadeInTime == 0)
                currentFadeTime = 1;

            fadeInDuration = fadeInTime;
            fadeOutDuration = 0;
            sustain = true;
        }

        
        // Scales this shake's roughness while preserving the initial Roughness.        
        public float ScaleRoughness
        {
            get { return roughMod; }
            set { roughMod = value; }
        }

        
        // Scales this shake's magnitude while preserving the initial Magnitude.     
        public float ScaleMagnitude
        {
            get { return magnMod; }
            set { magnMod = value; }
        }

        
        // A normalized value (about 0 to about 1) that represents the current level of intensity.     
        public float NormalizedFadeTime
        { get { return currentFadeTime; } }

        bool IsShaking
        { get { return currentFadeTime > 0 || sustain; } }

        bool IsFadingOut
        { get { return !sustain && currentFadeTime > 0; } }

        bool IsFadingIn
        { get { return currentFadeTime < 1 && sustain && fadeInDuration > 0; } }

        
        // Gets the current state of the shake.      
        public CameraShakeState CurrentState
        {
            get
            {
                if (IsFadingIn)
                    return CameraShakeState.FadingIn;
                else if (IsFadingOut)
                    return CameraShakeState.FadingOut;
                else if (IsShaking)
                    return CameraShakeState.Sustained;
                else
                    return CameraShakeState.Inactive;
            }
        }
    }
}