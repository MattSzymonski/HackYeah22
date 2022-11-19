using NaughtyAttributes;
using System;
using UnityEngine;

namespace Mighty
{
    [Serializable]
    public enum JuicerType
    {
        Relative,
        Absolute
    }

    [Serializable]
    public abstract class MightyJuicer
    {
        [Header("Info")]
        [ReadOnly] public bool active;
        [ReadOnly] public float progress;
        [ReadOnly] public float value;
       
        [Header("Settings")]
        public string name;
        [Tooltip("Should juicer work overwrite current values or add to them")] public JuicerType type;
        [Tooltip("Curve that describes animated parameter value over time")] public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        [Tooltip("Animation evaluation script")] public float speed = 1.0f;
        public bool looping;
        public bool playOnAwake;

        public void Play()
        {
            active = true;
        }

        public void Restart()
        {
            active = true;
            progress = 0.0f;
            value = animationCurve.Evaluate(progress);
        }

        public void Reset()
        {
            progress = 0.0f;
        }

        public void Stop()
        {
            active = false;
        }

        public void Update()
        {
            if (active)
            {
                if (progress < 1.0f)
                {
                    progress += speed * Time.deltaTime;
                    value = animationCurve.Evaluate(progress);
                }
                else
                {
                    if (looping)
                    {
                        progress = 0.0f;
                    }
                    else
                    {
                        progress = 1.0f;
                        active = false;
                    }
                    value = animationCurve.Evaluate(progress);
                }

                Evaluate(value);
            }
        }

        public abstract void Initialize(GameObject gameObject);
        public abstract void Evaluate(float value);
    }

    // --- Transform


    [Serializable]
    public class MightyJuicerPosition : MightyJuicer
    {
        [Header("Position Juicer")]
        public Vector3 axisMultiplier;
        Transform transform;
        Vector3 startingValues;

        public MightyJuicerPosition(string name) { base.name = name; }

        public override void Initialize(GameObject gameObject)
        {
            transform = gameObject.GetComponent<Transform>();
            startingValues = transform.position;
        }

        public override void Evaluate(float value)
        {
            transform.position = (startingValues * (1 - (int)JuicerType.Relative)) + axisMultiplier * value;
        }
    }

    [Serializable]
    public class MightyJuicerRotation : MightyJuicer
    {
        [Header("Rotation Juicer")]
        public Vector3 axisMultiplier;
        Transform transform;
        Vector3 startingValues;

        public MightyJuicerRotation(string name) { base.name = name; }

        public override void Initialize(GameObject gameObject)
        {
            transform = gameObject.GetComponent<Transform>();
            startingValues = transform.localEulerAngles;
        }

        public override void Evaluate(float value)
        {
            transform.localEulerAngles = (startingValues * (1 - (int)JuicerType.Relative)) + axisMultiplier * value;
        }
    }

    [Serializable]
    public class MightyJuicerScale : MightyJuicer
    {
        [Header("Scale Juicer")]
        public Vector3 axisMultiplier;
        Transform transform;
        Vector3 startingValues;

        public MightyJuicerScale(string name) { base.name = name; }

        public override void Initialize(GameObject gameObject)
        {
            transform = gameObject.GetComponent<Transform>();
            startingValues = transform.localScale;
        }

        public override void Evaluate(float value)
        {
            transform.localScale = (startingValues * (1 - (int)JuicerType.Relative)) + axisMultiplier * value;
        }
    }


    //// --- Rect Transform

    [Serializable]
    public class MightyJuicerRectScale : MightyJuicer
    {
        [Header("Scale Juicer")]
        public Vector3 axisMultiplier;
        RectTransform rectTransform;
        Vector3 startingValues;

        public MightyJuicerRectScale(string name) { base.name = name; }

        public override void Initialize(GameObject gameObject)
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            startingValues = rectTransform.localScale;
        }

        public override void Evaluate(float value)
        {
            rectTransform.localScale = (startingValues * (1 - (int)JuicerType.Relative)) + axisMultiplier * value;
        }
    }

    //public class RectScaleJuicer : MightyJuicer
    //{

    //}

    //public class RectRotationJuicer : MightyJuicer
    //{

    //}

    //// --- UI

    //public class UIImageColorJuicer : MightyJuicer
    //{

    //}

    //// --- Post Processing

    //public class ChromaticAberrationJuicer : MightyJuicer
    //{

    //}

    //public class BloomJuicer : MightyJuicer
    //{

    //}

    //public class LensDistortionJuicer : MightyJuicer
    //{

    //}

    //public class VignetteJuicer : MightyJuicer
    //{

    //}

    //// --- Other

    //public class ScreenShakeJuicer : MightyJuicer
    //{

    //}
}

