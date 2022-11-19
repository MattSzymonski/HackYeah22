using UnityEngine;
using System.Collections;
using NaughtyAttributes;


namespace Mighty
{
    [AddComponentMenu("RTS Camera")]
    public class CameraRTS : MonoBehaviour
    {
        public bool drawGizmos;

        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(0, yaw, 0) * translation;

                x += rotatedTranslation.x;
                y = rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }

        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement")]
        //Movement

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;
        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Space(5)]
        //Height
        [Tooltip("During height calculations take into consideration objects below the camera")] public bool autoHeight = true; 
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        //public float heightDampening = 5f;
        public float zoom = 0; //value in range (0, 1) used as t in Matf.Lerp

        [Space(5)]
        //Map limits
        public bool limitMap = true;
        [ShowIf("limitMap")] public Vector2 mapCenter;
        [ShowIf("limitMap")] public Vector2 mapLimit = new Vector2(50f, 50f);

        [Space(5)]
        //Targeting  
        public GameObject followTarget; //target to follow
        public bool followingTarget { get { return followTarget != null; } }
        public Vector3 targetOffset;
        public float followingSpeed = 5f;

        [Header("Input")]
        //Input
        public bool useGamepadInput = false;   
        [ShowIf("useGamepadInput")] public float gamepadMovementSpeed = 5f;
        [ShowIf("useGamepadInput")] public string gamepadHorizontalAxis = "Controller1 Left Stick Horizontal";
        [ShowIf("useGamepadInput")] public string gamepadVerticalAxis = "Controller1 Left Stick Vertical";
        [ShowIf("useGamepadInput")] public bool gamepadRotating = false;
        [ShowIf(EConditionOperator.And, "useGamepadInput", "gamepadRotating")] public string gamepadRotateRightKey = "Controller1 Right Bumper";
        [ShowIf(EConditionOperator.And, "useGamepadInput", "gamepadRotating")] public string gamepadRotateLeftKey = "Controller1 Left Bumper";
        [ShowIf("useGamepadInput")] public bool gamepadZooming = false;
        [ShowIf(EConditionOperator.And, "useGamepadInput", "gamepadZooming")] public string gamepadZoomingAxis = "Controller1 Right Stick Vertical";
        [ShowIf(EConditionOperator.And, "useGamepadInput", "gamepadZooming")] public bool gamepadZoomingInvert = false;

        [Space(5)]
        public bool useKeyboardInput = false;
        [ShowIf("useKeyboardInput")] public float keyboardMovementSpeed = 5f;
        [ShowIf("useKeyboardInput")] public string keyboardHorizontalAxis = "Horizontal";
        [ShowIf("useKeyboardInput")] public string keyboardVerticalAxis = "Vertical";
        [ShowIf("useKeyboardInput")] public bool keyboardRotating = false;
        [ShowIf(EConditionOperator.And, "useKeyboardInput", "keyboardRotating")] public float keyboardRotationSpeed = 3f;
        [ShowIf(EConditionOperator.And, "useKeyboardInput", "keyboardRotating")] public string keyboardRotateRightKey = "Rotate Right";
        [ShowIf(EConditionOperator.And, "useKeyboardInput", "keyboardRotating")] public string keyboardRotateLeftKey = "Rotate Left";
        [ShowIf("useKeyboardInput")] public bool keyboardZooming = false;
        [ShowIf(EConditionOperator.And, "useKeyboardInput", "keyboardZooming")] public float keyboardZoomingSensitivity = 2f;
        [ShowIf(EConditionOperator.And, "useKeyboardInput", "keyboardZooming")] public string keyboardZoomInKey = "Up";
        [ShowIf(EConditionOperator.And, "useKeyboardInput", "keyboardZooming")] public string keyboardZoomOutKey = "Down";

        [Space(5)]
        public bool useMouseInput = false;
        [ShowIf("useMouseInput")] public bool mouseScreenEdgeScrolling = false;
        [ShowIf(EConditionOperator.And, "useMouseInput", "mouseScreenEdgeScrolling")] public float screenEdgeMovementSpeed = 3f;
        [ShowIf(EConditionOperator.And, "useMouseInput", "mouseScreenEdgeScrolling")] public float screenEdgeBorder = 25f;
        [ShowIf("useMouseInput")] public bool mouseRotating = false;
        [ShowIf(EConditionOperator.And, "useMouseInput", "mouseRotating")] public float mouseRotatingSpeed = 10f;
        [ShowIf(EConditionOperator.And, "useMouseInput", "mouseRotating")] public string mouseRotatingKey = "LMB";
        [ShowIf("useMouseInput")] public bool mousePanning = false;
        [ShowIf(EConditionOperator.And, "useMouseInput", "mousePanning")] public string mouseAxisX = "Mouse X";
        [ShowIf(EConditionOperator.And, "useMouseInput", "mousePanning")] public string mouseAxisY = "Mouse Y";
        [ShowIf(EConditionOperator.And, "useMouseInput", "mousePanning")] public float mousePanningSpeed = 10f;
        [ShowIf(EConditionOperator.And, "useMouseInput", "mousePanning")] public string mousePanningKey = "RMB";
        [ShowIf("useMouseInput")] public bool scrollwheelZooming = false;
        [ShowIf(EConditionOperator.And, "useMouseInput", "scrollwheelZooming")] public float scrollWheelZoomingSensitivity = 25f;
        [ShowIf(EConditionOperator.And, "useMouseInput", "scrollwheelZooming")] public string scrollwheelZoomingAxis = "Mouse ScrollWheel";
        [ShowIf(EConditionOperator.And, "useMouseInput", "scrollwheelZooming")] public bool scrollwheelZoomingInvert = false;


        private void Start()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        private void Update()
        {
            CameraUpdate();
        }

        private void CameraUpdate()
        {
            //if (gameManager?.gameState == GameState.Playing || !gameManager)
            {
                if (followingTarget)
                {
                    FollowTarget();
                }
                else
                {
                    Move();
                }
                //HeightCalculation();
                Rotation();
                LimitPosition();
            }
        }

        private void Move()
        {
            //Keyboard
            if (useKeyboardInput)
            {
                if (keyboardZooming)
                {
                    int zoomDir = 0;
                    bool zoomIn = Input.GetButton(keyboardZoomInKey);
                    bool zoomOut = Input.GetButton(keyboardZoomOutKey);
                    if (zoomIn && zoomOut) { zoomDir = 0; } else if (zoomIn && !zoomOut) { zoomDir = 1; } else if (!zoomIn && zoomOut) { zoomDir = -1; } else { zoomDir = 0; }
                    zoom += zoomDir * Time.deltaTime * keyboardZoomingSensitivity;
                }

                Vector3 translation = new Vector3(Input.GetAxis(keyboardHorizontalAxis) * keyboardMovementSpeed * Time.deltaTime, zoom, Input.GetAxis(keyboardVerticalAxis) * keyboardMovementSpeed * Time.deltaTime) ;
                m_TargetCameraState.Translate(translation);
                var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
                var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

                m_InterpolatingCameraState.UpdateTransform(transform);
            }

            //Gamepad
            if (useGamepadInput)
            {
                Vector3 desiredMove = new Vector3(Input.GetAxis(gamepadHorizontalAxis), 0, Input.GetAxis(gamepadVerticalAxis)) * gamepadMovementSpeed * Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = transform.InverseTransformDirection(desiredMove);
                transform.Translate(desiredMove, Space.Self);
            }

            //Border scrolling
            if (useMouseInput && mouseScreenEdgeScrolling)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                Vector3 mouseInput = Input.mousePosition;
                desiredMove.x = leftRect.Contains(mouseInput) ? -1 : rightRect.Contains(mouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(mouseInput) ? 1 : downRect.Contains(mouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = transform.InverseTransformDirection(desiredMove);

                transform.Translate(desiredMove, Space.Self);
            }

            //Mouse panning
            if (useMouseInput && mousePanning && Input.GetButton(mousePanningKey))
            {
                Vector3 desiredMove = new Vector3(-Input.GetAxis(mouseAxisX), 0, -Input.GetAxis(mouseAxisY)) * mousePanningSpeed * Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = transform.InverseTransformDirection(desiredMove);
                transform.Translate(desiredMove, Space.Self);
            }
        }

        private float HeightCalculation(float zoom)
        {

            float distanceToGround = DistanceToGround();
            /*
            if (useMouseInput && scrollwheelZooming)
            {
                int invert = scrollwheelZoomingInvert ? -1 : 1;
                zoomPos += invert * Input.GetAxis(scrollwheelZoomingAxis) * Time.deltaTime * scrollWheelZoomingSensitivity;
            }
                
            if (useKeyboardInput && keyboardZooming)
            {
                int zoomDir = 0;
                bool zoomIn = Input.GetButton(keyboardZoomInKey);
                bool zoomOut = Input.GetButton(keyboardZoomOutKey);
                if (zoomIn && zoomOut) { zoomDir = 0; } else if (zoomIn && !zoomOut) { zoomDir = 1; } else if (!zoomIn && zoomOut) { zoomDir = -1; } else { zoomDir = 0; }
                zoomPos += zoomDir * Time.deltaTime * keyboardZoomingSensitivity;
            }

            if (useGamepadInput && gamepadZooming)
            {
                int invert = gamepadZoomingInvert ? -1 : 1;
                //TODO: Add gamepad zooming with buttons
                //int zoomDir = 0;
                float zoom = Input.GetAxis(gamepadZoomingAxis);
                zoomPos += invert * zoom * Time.deltaTime * keyboardZoomingSensitivity;
            }
            */
            zoom = Mathf.Clamp01(zoom);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoom);
            float difference = 0;

            if (distanceToGround != targetHeight && autoHeight)
            {
                difference = targetHeight - distanceToGround;
            }

            //return targetHeight + difference;
            return zoom;
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetHeight + difference, transform.position.z), Time.deltaTime * heightDampening);
        }

        private void Rotation()
        {
            if (useKeyboardInput && keyboardRotating) //Keyboard
            {
                int rotateDir = 0;
                bool right = Input.GetButton(keyboardRotateRightKey);
                bool left = Input.GetButton(keyboardRotateLeftKey);
                if (left && right) { rotateDir = 0; } else if (left && !right) { rotateDir = -1; } else if (!left && right) { rotateDir = 1; } else { rotateDir = 0; }
                transform.Rotate(Vector3.up, rotateDir * Time.deltaTime * keyboardRotationSpeed, Space.World);
            }

            if (useMouseInput && mouseRotating && Input.GetButton(mouseRotatingKey)) //Mouse
            {
                transform.Rotate(Vector3.up, -Input.GetAxis(mouseAxisX) * Time.deltaTime * mouseRotatingSpeed, Space.World);
            }

            if (useGamepadInput && gamepadRotating) //Gamepad
            {
                int rotateDir = 0;
                bool right = Input.GetButton(gamepadRotateRightKey);
                bool left = Input.GetButton(gamepadRotateLeftKey);
                if (left && right) { rotateDir = 0; } else if (left && !right) { rotateDir = -1; } else if (!left && right) { rotateDir = 1; } else { rotateDir = 0; }
                transform.Rotate(Vector3.up, rotateDir * Time.deltaTime * mouseRotatingSpeed, Space.World);
            }
        }

        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(followTarget.transform.position.x, transform.position.y, followTarget.transform.position.z) + targetOffset;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        private void LimitPosition()
        {
            if (!limitMap)
            {
                return;
            }
                
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, mapCenter.x - mapLimit.x, mapCenter.x + mapLimit.x), transform.position.y, Mathf.Clamp(transform.position.z, mapCenter.y - mapLimit.y, mapCenter.y + mapLimit.y));
        }

        public void SetTarget(GameObject target)
        {
            followTarget = target;
        }

        private float DistanceToGround()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, groundMask.value))
            {
                return (hit.point - transform.position).magnitude;
            }
            return 0f;
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + DistanceToGround() * Vector3.down);
                Gizmos.DrawSphere(transform.position + DistanceToGround() * Vector3.down, 0.2f);

                if (limitMap)
                {
                    Gizmos.DrawWireCube(new Vector3(mapCenter.x, (maxHeight + minHeight) / 2, mapCenter.y), new Vector3(mapLimit.x*2, 0, mapLimit.y*2));
                }

            }
        }
    }
}