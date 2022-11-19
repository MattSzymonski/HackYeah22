
// Copyright(C) Mateusz Szymonski 2019 - All Rights Reserved
// Unauthorized sharing of this file, via any medium is strictly prohibited
// Written by Mateusz Szymonski <matt.szymonski @gmail.com>

//Advanced free-flying camera with inertia effects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSmoothCamera : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("How fast changes of position will be applied"), Range(0.001f, 1f)] public float positionSmoothing = 0.8f;
    [Space(5)]
    public float keyboardMovementSpeed = 6;
    public float keyboardBoostValue = 3;
    [Space(5)]
    public float gamepadMovementSpeed = 6;
    public float gamepadBoostValue = 2;
    private float movementSpeedIncrease = 1;

    [Header("Rotation")]
    [Tooltip("How fast changes of rotation will be applied"), Range(0.001f, 1f)] public float rotationSmoothing = 0.4f;
    [Space(5)]
    [Tooltip("True - camera will be rotated only if mouseDrag button is clicked. False - cursor will be locked and hidden like in first person shooters")] public bool mouseDragging = true;
    public float mouseSensitivity = 1.5f;
    [Tooltip("How rotation speed is influenced by mouse dragging speed")] public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 1.0f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
    [Space(5)]
    public float gamepadSensitivity = 1f;
    [Tooltip("How rotation speed is influenced by gamepad stick deflection")] public AnimationCurve gamepadSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
    
    
    [Header("Inertia")]
    [Tooltip("How fast changes of inertia will be applied (Inertia of inertia ... really)"), Range(0.001f, 1f)] public float inertiaSmoothing = 0.6f;
    [Space(5)]
    [Tooltip("How far pitch, yaw and roll rotations can deflect")] public Vector3 pitchYawRollInertiaLimits = new Vector3(10, 10, 20);  
    [Tooltip("Inertia of camera when rotating using mouse dragging")] public float rotateInertiaStrength = 35.0f;
    [Tooltip("How powerful inertia during movement is")] public Vector3 pitchYawRollInertiaStrength = new Vector3(4, 4.5f, 6);
    [Tooltip("How fast more inertia is being added during movement (depending on current inertia value)")] public AnimationCurve inertiaApplyingSpeed = new AnimationCurve(new Keyframe(0.0f, 1.0f, 0.5f, 0.75f), new Keyframe(1.0f, 0.25f, -2.0f, 0.0f));
    [Space(5)]
    [Tooltip("How fast inertia will be damped, so how fast rotations will return to their base positions")] public Vector3 pitchYawRollInertiaDampingSpeed = new Vector3(10, 10, 18);
    [Tooltip("How fast inertia is being damped during movement (depending on current inertia value)")] public AnimationCurve inertiaDampingSpeed = new AnimationCurve(new Keyframe(0.0f, 2.0f, 0f, 0f), new Keyframe(1.0f, 0.25f, -8.0f, 0.0f));

    [Header("Input")]
    [Tooltip("Used when mouseDragging is true")] public string mouseDragButton = "RMB";
    public string mouseAxisX = "Mouse X";
    public string mouseAxisY = "Mouse Y";
    public bool mouseInvertYAxis = false;
    [Space(5)]
    public string keyboardForwardBackwardAxis = "Vertical";
    public string keyboardRightLeftAxis = "Horizontal";
    public string keyboardUpDownAxis = "UpDown";
    public string keyboardBoostButton = "Shift";
    [Space(5)]
    public string gamepadForwardBackwardAxis = "Controller1 Left Stick Vertical";
    public string gamepadRightLeftAxis = "Controller1 Left Stick Horizontal";
    public string gamepadUpButton = "Controller1 Right Bumper";
    public string gamepadDownButton = "Controller1 Left Bumper";
    public string gamepadPitchAxis = "Controller1 Right Stick Vertical";
    public string gamepadYawAxis = "Controller1 Right Stick Horizontal";
    public string gamepadBoostAxis = "Controller1 Triggers";
    public bool gamepadInvertYAxis = false;

    private float pitch, yaw, roll;
    private float x, y, z;

    private float yawTarget, pitchTarget, rollTarget;
    private float xTarget, yTarget, zTarget;

    private float pitchInertia, yawInertia, rollInertia;
    private float pitchInertiaTarget, yawInertiaTarget, rollInertiaTarget;

    void Start()
    {
        SetUpStartingValues();
    }

    void Update()
    {
        CameraMovement();
    }

    void SetUpStartingValues()
    {
        pitch = pitchTarget = transform.eulerAngles.x;
        yaw = yawTarget = transform.eulerAngles.y;
        roll = rollTarget = transform.eulerAngles.z;
        x = xTarget = transform.position.x;
        y = yTarget = transform.position.y;
        z = zTarget = transform.position.z;
    }

    void CameraMovement()
    {
        //Cursor locking
        if (mouseDragging)
        {
            Cursor.visible = true;
        }
        else
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            } 
        }

        //Rotation mouse
        if (Input.GetAxis(mouseAxisX) != 0 || Input.GetAxis(mouseAxisY) != 0)
        {
            if ((mouseDragging && Input.GetButton(mouseDragButton)) || !mouseDragging)
            {
                Vector2 mouseMovement = new Vector2(Input.GetAxis(mouseAxisX), Input.GetAxis(mouseAxisY) * (mouseInvertYAxis ? -1 : 1));
                yawTarget += mouseMovement.x * mouseSensitivityCurve.Evaluate(mouseMovement.magnitude) * mouseSensitivity;
                pitchTarget += mouseMovement.y * mouseSensitivityCurve.Evaluate(mouseMovement.magnitude) * mouseSensitivity;

                rollInertiaTarget += -mouseMovement.x * rotateInertiaStrength * Time.deltaTime;
                pitchInertiaTarget += mouseMovement.y * rotateInertiaStrength * Time.deltaTime;
            }     
        }

        //Rotation gamepad
        if (Input.GetAxis(gamepadPitchAxis) != 0 || Input.GetAxis(gamepadYawAxis) != 0)
        {
            Vector2 stickDeflection = new Vector2(-Input.GetAxis(gamepadPitchAxis), Input.GetAxis(gamepadYawAxis) * (gamepadInvertYAxis ? -1 : 1));
            pitchTarget += stickDeflection.x * gamepadSensitivityCurve.Evaluate(stickDeflection.magnitude) * gamepadSensitivity * 8;
            yawTarget += stickDeflection.y * gamepadSensitivityCurve.Evaluate(stickDeflection.magnitude) * gamepadSensitivity * 10;

            pitchInertiaTarget += -stickDeflection.x * rotateInertiaStrength * 10 * Time.deltaTime;
            rollInertiaTarget += stickDeflection.y * rotateInertiaStrength * 10 * Time.deltaTime;
        }

        //Get input
        Vector3 translation = GetTranslationDirection() * Time.deltaTime;
        float heightTranslation = GetHeightTranslationDirection() * Time.deltaTime;

        //Speed Increase (for mouse and keyboard input only)
        movementSpeedIncrease += Input.mouseScrollDelta.y * 0.1f;
        translation *= movementSpeedIncrease;

        //Boost
        if (Input.GetButton(keyboardBoostButton))
        {
            translation *= keyboardBoostValue;
            heightTranslation *= keyboardBoostValue;
        }

        if (Input.GetAxis(gamepadBoostAxis) < 0)
        { 
            translation *= 1 + -Input.GetAxis(gamepadBoostAxis) * gamepadBoostValue * 4.0f;
            heightTranslation *= 1 + -Input.GetAxis(gamepadBoostAxis) * gamepadBoostValue * 4.0f;
        }

        //Translate
        Vector3 rotatedTranslation = Quaternion.Euler(pitchTarget, yawTarget, rollTarget) * translation;
        xTarget += rotatedTranslation.x;
        yTarget += rotatedTranslation.y;
        zTarget += rotatedTranslation.z;
        yTarget += heightTranslation;

        //Inertia adding
        pitchInertiaTarget += translation.z * pitchYawRollInertiaStrength.x * inertiaApplyingSpeed.Evaluate(Mathf.Abs(pitchInertiaTarget) / pitchYawRollInertiaLimits.x);
        pitchInertiaTarget += heightTranslation * pitchYawRollInertiaStrength.x * inertiaApplyingSpeed.Evaluate(Mathf.Abs(pitchInertiaTarget) / pitchYawRollInertiaLimits.x);
        yawInertiaTarget += translation.y * pitchYawRollInertiaStrength.y * inertiaApplyingSpeed.Evaluate(Mathf.Abs(yawInertiaTarget) / pitchYawRollInertiaLimits.y);
        rollInertiaTarget += -translation.x * pitchYawRollInertiaStrength.z * inertiaApplyingSpeed.Evaluate(Mathf.Abs(rollInertiaTarget) / pitchYawRollInertiaLimits.y);

        //Inertia limits
        pitchInertiaTarget = Mathf.Clamp(pitchInertiaTarget, -pitchYawRollInertiaLimits.x, pitchYawRollInertiaLimits.x);
        yawInertiaTarget = Mathf.Clamp(yawInertiaTarget, -pitchYawRollInertiaLimits.y, pitchYawRollInertiaLimits.y);
        rollInertiaTarget = Mathf.Clamp(rollInertiaTarget, -pitchYawRollInertiaLimits.z, pitchYawRollInertiaLimits.z);

        //Inertia damping
        if (pitchInertiaTarget > 0) { pitchInertiaTarget -= pitchYawRollInertiaDampingSpeed.x * inertiaDampingSpeed.Evaluate(1 - Mathf.Abs(pitchInertiaTarget) / pitchYawRollInertiaLimits.x) * Time.deltaTime; }
        if (pitchInertiaTarget < 0) { pitchInertiaTarget += pitchYawRollInertiaDampingSpeed.x * inertiaDampingSpeed.Evaluate(1 - Mathf.Abs(pitchInertiaTarget) / pitchYawRollInertiaLimits.x) * Time.deltaTime; }
        if (yawInertiaTarget > 0) { yawInertiaTarget -= pitchYawRollInertiaDampingSpeed.y * inertiaDampingSpeed.Evaluate(1 - Mathf.Abs(yawInertiaTarget) / pitchYawRollInertiaLimits.y) * Time.deltaTime; }
        if (yawInertiaTarget < 0) { yawInertiaTarget += pitchYawRollInertiaDampingSpeed.y * inertiaDampingSpeed.Evaluate(1 - Mathf.Abs(yawInertiaTarget) / pitchYawRollInertiaLimits.y) * Time.deltaTime; }
        if (rollInertiaTarget > 0) { rollInertiaTarget -= pitchYawRollInertiaDampingSpeed.z * inertiaDampingSpeed.Evaluate(1 - Mathf.Abs(rollInertiaTarget) / pitchYawRollInertiaLimits.z) * Time.deltaTime; }
        if (rollInertiaTarget < 0) { rollInertiaTarget += pitchYawRollInertiaDampingSpeed.z * inertiaDampingSpeed.Evaluate(1 - Mathf.Abs(rollInertiaTarget) / pitchYawRollInertiaLimits.z) * Time.deltaTime; }

        //Smoothing
        float positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionSmoothing) * Time.deltaTime);
        float rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationSmoothing) * Time.deltaTime);
        float inertiaLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / inertiaSmoothing) * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, pitchTarget, rotationLerpPct);
        yaw = Mathf.Lerp(yaw, yawTarget, rotationLerpPct);
        roll = Mathf.Lerp(roll, rollTarget, rotationLerpPct);
        x = Mathf.Lerp(x, xTarget, positionLerpPct);
        y = Mathf.Lerp(y, yTarget, positionLerpPct);
        z = Mathf.Lerp(z, zTarget, positionLerpPct);
        pitchInertia = Mathf.Lerp(pitchInertia, pitchInertiaTarget, positionLerpPct);
        yawInertia = Mathf.Lerp(yawInertia, yawInertiaTarget, positionLerpPct);
        rollInertia = Mathf.Lerp(rollInertia, rollInertiaTarget, positionLerpPct);

        //Applying new position and rotation
        transform.eulerAngles = new Vector3(pitch + pitchInertia, yaw + yawInertia, roll + rollInertia);
        transform.position = new Vector3(x, y, z);
    }

    Vector3 GetTranslationDirection()
    {
        Vector3 direction = new Vector3();

        //Keyboard
        direction += Vector3.forward * Input.GetAxis(keyboardForwardBackwardAxis) * keyboardMovementSpeed;
        direction += Vector3.right * Input.GetAxis(keyboardRightLeftAxis) * keyboardMovementSpeed;

        //Gamepad
        direction -= Vector3.forward * Input.GetAxis(gamepadForwardBackwardAxis) * gamepadMovementSpeed * 10;
        direction += Vector3.right * Input.GetAxis(gamepadRightLeftAxis) * gamepadMovementSpeed * 10;

        return direction;
    }

    float GetHeightTranslationDirection()
    {
        float direction = 0;

        //Keyboard
        direction += 1 * Input.GetAxis(keyboardUpDownAxis) * keyboardMovementSpeed;

        //Gamepad
        if (Input.GetButton(gamepadUpButton))
        {
            direction += 1 * gamepadMovementSpeed * 0.75f;
        }
        if (Input.GetButton(gamepadDownButton))
        {
            direction -= 1 * gamepadMovementSpeed * 0.75f;
        }

        return direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
    }
}
