﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

using NaughtyAttributes;

public class IndicatorTarget : MonoBehaviour
{
    [Header("Color")]
    [Tooltip("The color of the indicator.")]
    public Color Color = Color.red;
    [Tooltip("Use a random color. Will override 'NewColor'")]
    public bool RandomColor = false;


    [Header("Distance Display")]
    public bool distanceDisplay;

    [Tooltip("The custom font used for the distance indicator. If left empty, the default Arial font will be used instead.")]
    [ShowIf("distanceDisplay")] public Font CustomFont;
    [Tooltip("Does the distance show on the On-Screen indicator?")]
    [ShowIf("distanceDisplay")] public bool ShowOnScreen = true;
    [Tooltip("Does the distance show on the Off-Screen indicator?")]
    [ShowIf("distanceDisplay")] public bool ShowOffScreen = true;
    [Tooltip("Offset position for the on-screen indicator.")]
    [ShowIf("distanceDisplay")] public Vector2 OnScreenPosOffset = new Vector2(0, 65);
    [Tooltip("Offset position for the off-screen indicator.")]
    [ShowIf("distanceDisplay")] public Vector2 OffScreenPosOffset = new Vector2(0, -50);
    [Tooltip("The suffix text that displays after the distance text. Represents distance units such as Meters.")]
    [ShowIf("distanceDisplay")] public string distanceSuffix = "m";
    [Tooltip("The farthest decimal point the distance will display.")]
    [ShowIf("distanceDisplay")] public Decial DecimalPoint;
    public enum Decial { None, Tenths, Hundredths, Thousandths, TenThousandths }
    [Tooltip("The font size of distancetext.")]
    [ShowIf("distanceDisplay")] public int FontSize = 24;

    //  Variables
    private GameObject distanceIndicator;
    private Text distanceText;
    private bool isOnScreen = true;




    //Target Indicator variables
    private IndicatorViewer viewer;
    private IndicatorPanel IPanel;
    private bool isVisable;                     //  Check if the viewer is on-screen or off-screen
    private bool isOnScreenEnabled;             //  Check if onscreen is enabled
    private bool isOffScreenEnabled;            //  Check if offscreen is enabled.
    private float distanceFromViewer;           //  Distance from the viewer
    //private float previousZposition;            //  The last Z position value of the scaling axis. (used for scaling the indicator)
    //private Vector3 previousScale;              //  The last local scale value of the indicator. (used for scaling the indicator) 

    void Awake()
    {
        //  Find and assign references
        viewer = FindObjectOfType<IndicatorViewer>();
    }

    void Start()
    {
        //  Create the indicator for this target
        InitializeIndicator();
        ChangeColor(Color, RandomColor);
        if (distanceDisplay)
        {
            StartCoroutine(CoCreateDistanceTracker());
        }
    }

    //  OnEnable/OnDisable used for pooling objects
    void OnEnable()
    {
        //  Add the target the the viewer's target list so viewer can track this target.
        if (!IndicatorViewer.Targets.Contains(this) && IPanel != null)
            IndicatorViewer.Targets.Add(this);

        //  Disable the panel so viewer does not see it ingame
        if (IPanel != null)
            IPanel.gameObject.SetActive(true);

        if (distanceDisplay)
        {
            if (distanceIndicator != null)
                distanceIndicator.SetActive(true);
        }
        
    }
    void OnDisable()
    {
        //  Remove the target from the viewer's list so viewer no longer tracks this target.
        IndicatorViewer.Targets.Remove(this);

        //  Enable the panel so viewer can see it ingame
        if (IPanel != null)
            IPanel.gameObject.SetActive(false);

        if (distanceDisplay)
        {
            if (distanceIndicator != null)
                distanceIndicator.SetActive(false);
        }
    }

    void LateUpdate()
    {
        //  if the viewer is currently tracking...
        if (IPanel != null && IndicatorViewer.IsTracking)
            IPanel.gameObject.SetActive(true);

        //  else if the viewer is not tracking...
        else if (IPanel != null)
            IPanel.gameObject.SetActive(false);

        if (distanceDisplay)
        {
            UpdateDistanceTracker();
        }
    }

    #region Initial set-up of indicator
    //  Create & set-up the indicator panel
    private void InitializeIndicator()
    {
        //  If customIndicatorPanel is still null because there is no viewer default, throw error 
        try
        {
            //  Create the runtime indicator object and assign it under the canvas. 
            GameObject panel = Instantiate(viewer.IndicatorPanelPrefab, new Vector2(-100, -100), Quaternion.identity) as GameObject;
            panel.name = name + " indicator";
            panel.transform.SetParent(viewer.IndicatorCanvas.transform);
            IPanel = panel.GetComponent<IndicatorPanel>();


            //  Assign the initial z position && scale value
            //previousZposition = IPanel.transform.position.z;
            //previousScale = IPanel.transform.localScale;

            //  Add this target to the list of targets if not already
            if (!IndicatorViewer.Targets.Contains(this))
                IndicatorViewer.Targets.Add(this);

            //  initial indicator toggle

            if(OnScreen(viewer.ViewerCamera.WorldToScreenPoint(transform.position), viewer.ViewerCamera))
            {
                if (IPanel.OnScreen != null)
                    ToggleOnScreen(true);
                if (IPanel.OffScreen != null)
                    ToggleOffScreen(false);
            }
            else
            {
                if (IPanel.OnScreen != null)
                    ToggleOnScreen(false);
                if (IPanel.OffScreen != null)
                    ToggleOffScreen(true);
            }

        }
        catch
        {   Debug.LogError("The 'DefaultIndicatorPanel' requires a UnityUI Panel object!", viewer);    }
    }
    #endregion

    #region Update the position & rotation of the indicator panel of this target.
    //  Called from 'IndicatorViewer' script.
    public void UpdateIndicator()
    {
        //  Get the target's world position in screen coordinate position
        Vector3 targetPosOnScreen = viewer.ViewerCamera.WorldToScreenPoint(transform.position);

        //  if the target is visable on screen...
        if (OnScreen(targetPosOnScreen, viewer.ViewerCamera))
        {

            //  Disable the off-screen indicator if it exist
            if (IPanel.OffScreen != null && isOffScreenEnabled)
                ToggleOffScreen(false);

            //  if the viewer allows indicators to show when the target is on-screen
            if (IndicatorViewer.TrackOnScreen && IPanel.OnScreen != null)
            {
                //  Set target to visable.
                isVisable = true;

                //  Get distance from this target and viewer
                distanceFromViewer = GetDistance(transform.position, viewer.ViewerObject.transform.position);

                //  If the on-screen indicator is within distance threshold, then do stuff...  
                if (CheckDisableOnDistance(distanceFromViewer) == false)
                {
                    //  Enable the on-screen indicator
                    if (!isOnScreenEnabled)
                        ToggleOnScreen(true);

                    //  set the UI panel position to the target's position
                    IPanel.transform.position = targetPosOnScreen;
                    //IPanel.transform.position = transform.position + new Vector3(onScreenOffset.x, onScreenOffset.y, transform.position.z);

                    //  If OnScreen exist && scaling is enabled && if the target's indicator axis towards the camera has changed...
                    if (viewer.AutoScale) //(IPanel.transform.position.z != previousZposition || IPanel.transform.localScale != previousScale))
                    {
                        UpdateScale(distanceFromViewer);

                        //  Record the new axis position & local scale of the indicator panel
                        //previousZposition = IPanel.transform.position.z;
                        //previousScale = IPanel.transform.localScale;
                    }
                }
                else if(isOnScreenEnabled)
                {
                    ToggleOnScreen(false);
                }
            }
            else if (IPanel.OnScreen != null && isOnScreenEnabled)
            {
                ToggleOnScreen(false);
            }
        }
        //  Else, target is Off-Screen...
        else
        {
            // Set target as invisable.
            isVisable = false;

            //  Disables the on screen indicator if it exists
            if (IPanel.OnScreen != null && isOnScreenEnabled)
                ToggleOnScreen(false);

            if (IndicatorViewer.TrackOffScreen && IPanel.OffScreen != null)
            {
                //  Get distance from this target and viewer
                distanceFromViewer = GetDistance(transform.position, viewer.ViewerObject.transform.position);

                //  If the off-screen indicator is within distance threshold, then do stuff...  
                if (CheckDisableOnDistance(distanceFromViewer) == false)
                {
                    //  Enables the off screen indicator
                    if (!isOffScreenEnabled)
                        ToggleOffScreen(true);

                    //  Update the indicator positon and rotation based on the target's position on the screen
                    UpdateOffScreen(targetPosOnScreen);

                    //  If scaling is enabled... then perform the update indicator method
                    if (viewer.AutoScale)
                        UpdateScale(distanceFromViewer);
                }
                else if (isOffScreenEnabled)
                {
                    ToggleOffScreen(false);
                }
            }
            else if(IPanel.OffScreen != null && isOffScreenEnabled)
            {
                ToggleOffScreen(false);
            }     
        }
    }
    #endregion

    #region Method that returns true if target is within the camera screen boundaries and it's near clipping plane.
    private bool OnScreen(Vector3 pos, Camera camera)
    {
        if (pos.x < (Screen.width * viewer.TargetEdgeDistance) && pos.x > (Screen.width - Screen.width * viewer.TargetEdgeDistance) && 
            pos.y < (Screen.height * viewer.TargetEdgeDistance) && pos.y > (Screen.height - Screen.height * viewer.TargetEdgeDistance) &&
            pos.z > camera.nearClipPlane && pos.z < camera.farClipPlane)
            return true;
        return false;
    }
    #endregion

    #region  Method that returns false if indicator does not need to be disabled because either DisableOnDistance is false or distance is within threshold. 
    private bool CheckDisableOnDistance(float distance)
    {
        //  Check if indicator will disable on distance and check if distance is over the distance threshold
        if (viewer.DisableOnDistance && distance > viewer.DisableDistance)
            return true;
        return false;
    }
    #endregion

    #region Method that updates scale
    //  Updates the scaling of the UI based on distance provided
    private void UpdateScale(float distance)
    {
        //  Create a scaling factor based on distance
        float newScaleSize = viewer.ScalingFactor / distance;

        //  Set the scale based on the scaling factor
        IPanel.transform.localScale = new Vector2(newScaleSize, newScaleSize);

        //  If the indicator scale is lower then or equal to the minimum size... Then, set the scale to the minimum size.
        //  Else if the indicator scale is greater or equal to the maximum size... Then, set the scale to the maximum size.
        if (IPanel.transform.localScale.x <= viewer.MinScaleSize || IPanel.transform.localScale.y <= viewer.MinScaleSize)
            IPanel.transform.localScale = new Vector2(viewer.MinScaleSize, viewer.MinScaleSize);
        else if (IPanel.transform.localScale.x >= viewer.MaxScaleSize || IPanel.transform.localScale.y >= viewer.MaxScaleSize)
            IPanel.transform.localScale = new Vector2(viewer.MaxScaleSize, viewer.MaxScaleSize);

        //Debug.Log("Scaling...");
    }
    #endregion

    #region Calculate OffScreen Position, Rotation, and Size
    //  Updates the indicator based on the off-screen target
    private void UpdateOffScreen(Vector3 targetPosOnScreen)
    {
        //  Create a variable for the center position of the screen.
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

        //  Set newIndicatorPos anchor to the center of the screen instead of bottom left
        Vector3 newIndicatorPos = targetPosOnScreen - screenCenter;

        //  Flip the newIndicatorPos to correct the calculations for indicators behind the camera.
        if (newIndicatorPos.z < 0)
            newIndicatorPos *= -1;

        //  Get angle from center of screen to target position
        float angle = Mathf.Atan2(newIndicatorPos.y, newIndicatorPos.x);
        angle -= 90 * Mathf.Deg2Rad;

        //  y = mx + b (intercept forumla)
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float m = cos / sin;

        //  Create the screen boundaries that the indicators reside in.
        Vector3 screenBounds = new Vector3(screenCenter.x * viewer.IndicatorEdgeDistance, screenCenter.y * viewer.IndicatorEdgeDistance);

        //  Check which screen side the target is currently in.
        //  Check top & bottom
        if (cos > 0)
            newIndicatorPos = new Vector2(-screenBounds.y / m, screenBounds.y);
        else
            newIndicatorPos = new Vector2(screenBounds.y / m, -screenBounds.y);

        //  Check left & right
        if (newIndicatorPos.x > screenBounds.x)
            newIndicatorPos = new Vector2(screenBounds.x, -screenBounds.x * m);
        else if (newIndicatorPos.x < -screenBounds.x)
            newIndicatorPos = new Vector2(-screenBounds.x, screenBounds.x * m);

        //  Reset the newIndicatorPos anchor back to bottom left corner.
        newIndicatorPos += screenCenter;

        //  Assign new position
        IPanel.transform.position = new Vector3(newIndicatorPos.x, newIndicatorPos.y, targetPosOnScreen.z);

        //  Assign new rotation
        if (viewer.OffScreenRotates)
            IPanel.OffScreen.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        else
            IPanel.OffScreen.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    #endregion

    #region that returns the distance magnitude between two vector3 positions. (Faster then built-in Vector3.Distance function)
    private float GetDistance(Vector3 PosA, Vector3 PosB)
    {
        Vector3 heading;

        //  Subtracting from both vectors returns the magnitude
        heading.x = PosA.x - PosB.x;
        heading.y = PosA.y - PosB.y;
        heading.z = PosA.z - PosB.z;

        //  Return the sqaure root of the sum of each sqaured vector axises.
        return Mathf.Sqrt((heading.x * heading.x) + (heading.y * heading.y) + (heading.z * heading.z));
    }
    #endregion

    #region OnScreen Tranisitions

    //  Toggle On-Screen indicator
    private void ToggleOnScreen(bool enable)
    {
        //  Set its enabled state, enable/disable its gameobject, then determine which animation to use based on viewer settings
        if (enable)
        {
            isOnScreenEnabled = true;
            IPanel.OnScreen.SetActive(true);

            switch (viewer.OnScreenEnableTransition)
            {
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OnScreen.transform, Vector2.zero, Vector2.zero, viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OnScreen.transform, 1, viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Rotate:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 0), viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.RotateReverse:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 0), viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OnScreen.transform, Vector3.zero, Vector3.one, viewer.TransitionDuration, false);
                    break;
            }
        }
        else
        {
            isOnScreenEnabled = false;

            switch (viewer.OnScreenDisableTransition)
            {
                case IndicatorViewer.Transitions.None:
                    IPanel.OnScreen.SetActive(false);
                    break;
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OnScreen.transform, Vector2.zero, Vector2.zero, viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OnScreen.transform, 0, viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Rotate:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.RotateReverse:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 90), viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OnScreen.transform, Vector3.one, Vector3.zero, viewer.TransitionDuration, true);
                    break;
            }
        }
    }

    #endregion

    #region OffScreen Transitions
    //  //  Toggle Off-Screen indicator
    private void ToggleOffScreen(bool enable)
    {
        // Set its enabled state, enable/ disable its gameobject, then determine which animation to use based on viewer settings
        if (enable)
        {
            isOffScreenEnabled = true;
            IPanel.OffScreen.SetActive(true);
            
            switch (viewer.OffScreenEnableTransition)
            {
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OffScreen.transform, new Vector3(0, 50, 0), Vector3.zero, viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OffScreen.transform, 1, viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OffScreen.transform, Vector3.zero, Vector3.one, viewer.TransitionDuration, false);
                    break;
            }
        }
        else
        {
            isOffScreenEnabled = false;

            switch (viewer.OffScreenDisableTransition)
            {
                case IndicatorViewer.Transitions.None:
                    IPanel.OffScreen.SetActive(false);
                    break;
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OffScreen.transform, Vector3.zero, new Vector3(0, 50, 0), viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OffScreen.transform, 0, viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OffScreen.transform, Vector3.one, Vector3.zero, viewer.TransitionDuration, true);
                    break;
            }
        }
    }

    #endregion

    #region OnDestory()
    void OnDestroy()
    {
        DestroyIndicator();
        //print("object was destroyed");
    }
    #endregion

    #region Method that handles destroying of indicator
    public void DestroyIndicator()
    {
        if (IPanel != null)
            Destroy(IPanel.gameObject);       
        Destroy(this);
    }
    #endregion

    #region Getters/Setters
    public IndicatorPanel IndicatorPanel
    {
        get { return IPanel; }
    }
    public bool IsVisable
    {
        get { return isVisable; }
    }
    public float DistanceFromViewer
    {
        get { return distanceFromViewer; }
    }
    #endregion

//Color module

    #region Change Color
    public void ChangeColor(Color newColor, bool random)
    {

        if (random)
        {
            newColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }


        if (GetComponent<IndicatorTarget>() != null)
        {
            IndicatorPanel IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

            //  Changes all graphic colors
            Graphic[] graphics = IPanel.GetComponentsInChildren<Graphic>(true);
            if (graphics.Length > 0)
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = newColor;
                }
            }
        }
    }
    #endregion

    //Distance tracker module

    #region Update position & scale of the distance tracker
    void UpdateDistanceTracker()
    {
        if ((ShowOnScreen || ShowOffScreen) && IPanel != null && distanceIndicator != null)
        {
            //  Runs once during on-screen & off-screen transition
            if (IsVisable && IPanel.OnScreen != null && !isOnScreen)
            {
                if (ShowOnScreen)
                {
                    distanceIndicator.SetActive(true);
                    distanceIndicator.transform.SetParent(IPanel.OnScreen.transform);
                    distanceIndicator.transform.localPosition = OnScreenPosOffset;
                }
                else
                    distanceIndicator.SetActive(false);

                isOnScreen = true;
            }
            else if (!IsVisable && IPanel.OffScreen != null && isOnScreen)
            {
                if (ShowOffScreen)
                {
                    distanceIndicator.SetActive(true);
                    distanceIndicator.transform.SetParent(IPanel.OffScreen.transform);
                    distanceIndicator.transform.localPosition = OffScreenPosOffset;
                }
                else
                    distanceIndicator.SetActive(false);

                isOnScreen = false;
            }

            //  Set the decimal point
            switch (DecimalPoint)
            {
                case Decial.None:
                    distanceText.text = DistanceFromViewer.ToString("F0") + distanceSuffix;
                    break;
                case Decial.Tenths:
                    distanceText.text = DistanceFromViewer.ToString("F1") + distanceSuffix;
                    break;
                case Decial.Hundredths:
                    distanceText.text = DistanceFromViewer.ToString("F2") + distanceSuffix;
                    break;
                case Decial.Thousandths:
                    distanceText.text = DistanceFromViewer.ToString("F3") + distanceSuffix;
                    break;
                case Decial.TenThousandths:
                    distanceText.text = DistanceFromViewer.ToString("F4") + distanceSuffix;
                    break;
            }

            distanceIndicator.transform.localScale = Vector3.one;
            distanceIndicator.transform.rotation = Quaternion.identity;
        }
    }
    #endregion

    #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.
    IEnumerator CoCreateDistanceTracker()
    {
        IPanel = IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = IndicatorPanel;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
       
        //  Now that the indicator panel exist, create the tracker
        CreateDistanceTracker();
    }
    #endregion

    #region Create the distance indicator
    void CreateDistanceTracker()
    {
        distanceIndicator = new GameObject("DistanceTrackerIndicator");
        distanceIndicator.layer = 1 << 4;

        //  Determine initial parent
        if (IsVisable && IPanel.OnScreen != null)
            distanceIndicator.transform.SetParent(IndicatorPanel.OnScreen.transform);
        else if (!IsVisable && IPanel.OffScreen != null)
            distanceIndicator.transform.SetParent(IndicatorPanel.OffScreen.transform);
        else
            distanceIndicator.transform.SetParent(IndicatorPanel.transform);

        //  Reset position & scale
        distanceIndicator.transform.localPosition = OnScreenPosOffset;
        distanceIndicator.transform.localScale = Vector3.one;

        //  text
        distanceText = distanceIndicator.AddComponent<Text>();
        distanceText.alignment = TextAnchor.MiddleCenter;
        distanceText.horizontalOverflow = HorizontalWrapMode.Overflow;
        distanceText.verticalOverflow = VerticalWrapMode.Overflow;
        distanceText.fontSize = FontSize;
        if (CustomFont == null)
        {
            CustomFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            distanceText.font = CustomFont;
        }


        //  Add ui effects
        distanceIndicator.AddComponent<Shadow>();
        //distanceIndicator.AddComponent<Outline>();

        //distanceIndicator.SetActive(false);
    }
    #endregion
}
