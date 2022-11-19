/* 
NAME:
    Camera Shaker

DESCRIPTION:
    Absolutely amazing camera shakes

USAGE:
    Create hierarchy: Camera object (with CameraContoller) -> Shaker (with CameraShaker) -> MainCamera (with Camera component)
    Camera.main.transform.parent.GetComponent<Mighty.CameraShaker>().ShakeOnce(3.0f, 1f, 1f, 1.25f);

TODO:
   There is bug when playing random sound multiple are being played (one by one?)
*/


using UnityEngine;
using System.Collections.Generic;




namespace Mighty
{
    [AddComponentMenu("EZ Camera Shake/Camera Shaker")]
    public class CameraShaker : MonoBehaviour
    {
        // The single instance of the CameraShake in the current scene. Do not use if you have multiple instances.
        public static CameraShaker Instance;
        static Dictionary<string, CameraShaker> instanceList = new Dictionary<string, CameraShaker>();

        // The default position influcence of all shakes created by this shaker.
        public Vector3 DefaultPosInfluence = new Vector3(0.15f, 0.15f, 0.15f);

        // The default rotation influcence of all shakes created by this shaker.
        public Vector3 DefaultRotInfluence = new Vector3(1, 1, 1);

        Vector3 posAddShake, rotAddShake;

        List<CameraShakeInstance> cameraShakeInstances = new List<CameraShakeInstance>();

        void Awake()
        {
            Instance = this;
            instanceList.Add(gameObject.name, this);
        }

        void Update()
        {
            posAddShake = Vector3.zero;
            rotAddShake = Vector3.zero;

            for (int i = 0; i < cameraShakeInstances.Count; i++)
            {
                if (i >= cameraShakeInstances.Count)
                    break;

                CameraShakeInstance c = cameraShakeInstances[i];

                if (c.CurrentState == CameraShakeState.Inactive && c.DeleteOnInactive)
                {
                    cameraShakeInstances.RemoveAt(i);
                    i--;
                }
                else if (c.CurrentState != CameraShakeState.Inactive)
                {
                    posAddShake += MultiplyVectors(c.UpdateShake(), c.PositionInfluence);
                    rotAddShake += MultiplyVectors(c.UpdateShake(), c.RotationInfluence);
                }
            }

            transform.localPosition = posAddShake;
            transform.localEulerAngles = rotAddShake;
        }

        // Gets the CameraShaker with the given name, if it exists.
        public static CameraShaker GetInstance(string name)
        {
            CameraShaker c;

            if (instanceList.TryGetValue(name, out c))
                return c;

            Debug.LogError("CameraShake " + name + " not found!");

            return null;
        }

        // Starts a shake using the given preset.
        public CameraShakeInstance Shake(CameraShakeInstance shake)
        {
            cameraShakeInstances.Add(shake);
            return shake;
        }

        // Shake the camera once, fading in and out  over a specified durations.
        public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
            shake.PositionInfluence = DefaultPosInfluence;
            shake.RotationInfluence = DefaultRotInfluence;
            cameraShakeInstances.Add(shake);

            //Handheld.Vibrate(); // Vibration

            return shake;
        }


        // Shake the camera once, fading in and out over a specified durations.
        public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime, Vector3 posInfluence, Vector3 rotInfluence)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
            shake.PositionInfluence = posInfluence;
            shake.RotationInfluence = rotInfluence;
            cameraShakeInstances.Add(shake);

            return shake;
        }

        // Start shaking the camera.
        public CameraShakeInstance StartShake(float magnitude, float roughness, float fadeInTime)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness);
            shake.PositionInfluence = DefaultPosInfluence;
            shake.RotationInfluence = DefaultRotInfluence;
            shake.StartFadeIn(fadeInTime);
            cameraShakeInstances.Add(shake);
            return shake;
        }

        // Start shaking the camera.
        public CameraShakeInstance StartShake(float magnitude, float roughness, float fadeInTime, Vector3 posInfluence, Vector3 rotInfluence)
        {
            CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness);
            shake.PositionInfluence = posInfluence;
            shake.RotationInfluence = rotInfluence;
            shake.StartFadeIn(fadeInTime);
            cameraShakeInstances.Add(shake);
            return shake;
        }

        // Gets a copy of the list of current camera shake instances.
        public List<CameraShakeInstance> ShakeInstances
        { get { return new List<CameraShakeInstance>(cameraShakeInstances); } }

        void OnDestroy()
        {
            instanceList.Remove(gameObject.name);
        }



        // Utils
        // Smoothes a Vector3 that represents euler angles.
        public static Vector3 SmoothDampEuler(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            Vector3 v;

            v.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, smoothTime);
            v.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, smoothTime);
            v.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, smoothTime);

            return v;
        }


        // Multiplies each element in Vector3 v by the corresponding element of w.    
        public static Vector3 MultiplyVectors(Vector3 v, Vector3 w)
        {
            v.x *= w.x;
            v.y *= w.y;
            v.z *= w.z;

            return v;
        }

    }
}