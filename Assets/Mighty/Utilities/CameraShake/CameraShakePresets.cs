using UnityEngine;

namespace Mighty
{

    public static class CameraShakePresets
    {
        
        // [One-Shot] A high magnitude, short, yet smooth shake.        
        public static CameraShakeInstance Bump
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(2.5f, 4, 0.1f, 0.75f);
                c.PositionInfluence = Vector3.one * 0.15f;
                c.RotationInfluence = Vector3.one;
                return c;
            }
        }

        
        // [One-Shot] An intense and rough shake.       
        public static CameraShakeInstance Explosion
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(5f, 10, 0, 1.5f);
                c.PositionInfluence = Vector3.one * 0.25f;
                c.RotationInfluence = new Vector3(4, 1, 1);
                return c;
            }
        }

        
        // [Sustained] A continuous, rough shake.       
        public static CameraShakeInstance Earthquake
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(0.6f, 3.5f, 2f, 10f);
                c.PositionInfluence = Vector3.one * 0.25f;
                c.RotationInfluence = new Vector3(1, 1, 4);
                return c;
            }
        }

        
        // [Sustained] A bizarre shake with a very high magnitude and low roughness.        
        public static CameraShakeInstance BadTrip
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(10f, 0.15f, 5f, 10f);
                c.PositionInfluence = new Vector3(0, 0, 0.15f);
                c.RotationInfluence = new Vector3(2, 1, 4);
                return c;
            }
        }

        
        // [Sustained] A subtle, slow shake.        
        public static CameraShakeInstance HandheldCamera
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(1f, 0.25f, 5f, 10f);
                c.PositionInfluence = Vector3.zero;
                c.RotationInfluence = new Vector3(1, 0.5f, 0.5f);
                return c;
            }
        }

        
        // [Sustained] A very rough, yet low magnitude shake.       
        public static CameraShakeInstance Vibration
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(0.4f, 20f, 2f, 2f);
                c.PositionInfluence = new Vector3(0, 0.15f, 0);
                c.RotationInfluence = new Vector3(1.25f, 0, 4);
                return c;
            }
        }

        
        // [Sustained] A slightly rough, medium magnitude shake.       
        public static CameraShakeInstance RoughDriving
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(1, 2f, 1f, 1f);
                c.PositionInfluence = Vector3.zero;
                c.RotationInfluence = Vector3.one;
                return c;
            }
        }
    }
}