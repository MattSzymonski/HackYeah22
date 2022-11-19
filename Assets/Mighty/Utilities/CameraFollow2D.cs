using UnityEngine;
using System.Collections;
using NaughtyAttributes;

//Smoothly follows target and gently twists towards moving direction
namespace Mighty
{
    public class CameraFollow2D : MonoBehaviour
    {
        public Transform target;
        public float positionDampTime = 0.12f;
        public Vector3 offset;
        private Vector3 positionVelocity = Vector3.zero;

        public float maxInclination = 2f;
        public float maxCameraSpeed = 8f; //Should be the same as max horizontal (usually x) position velocity

        void LateUpdate()
        {
            if (target)
            {
                Vector3 destination = target.position + offset;
                Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, destination, ref positionVelocity, positionDampTime);
                transform.position = smoothedPosition;

                float cameraInclination = maxInclination * (-positionVelocity.x / maxCameraSpeed);
                transform.rotation = Quaternion.Euler(0, 0, cameraInclination);
            }
        }
    }
}