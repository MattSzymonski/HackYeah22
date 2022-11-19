using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giraffe : MonoBehaviour
{
    public int index;

    [ReadOnly] public float angle = 90;
    public float targetAngle = 120;
    public Vector2 maxAngles = new Vector2(120, 60);
    public float sweetAngle = 10;
    [ReadOnly] public bool left = false;
    public float baseSpeed;
    [ReadOnly] public float boost;
    public float boostIncrease;
    public float slowMultiplier;
    public int maxBoostIncreases = 3;
    [ReadOnly] public int boostIncreases =0;


    [ReadOnly] public bool slow;

    public float slowThreshold;

    public GameObject neck;
    //public Mighty.MightyTimer keyHoldManager;

    [ReadOnly] public bool timerRunning;
    [ReadOnly] public float time;
    public float timerSpeed;


    [ReadOnly] public float timeX;
    public float timerSpeedX;

    public float movementSpeed;


    public List<Rigidbody2D> rbs;
    public float drag;
    public float activedrag;


    void Start()
    {
        //keyHoldManager = Mighty.MightyTimersManager.Instance.CreateTimer("KeyHoldManager", 1f, 1f, false, true); //Not looping, stopped on start
    }



    void SwitchSide(bool increased)
    {
        if (left)
        {
            left = false;
            targetAngle = maxAngles.y;
        }
        else
        {
            left = true;
            targetAngle = maxAngles.x;
        }

        if (boostIncreases != 0 && !increased)
        {
            boost -= boostIncrease;
            boostIncreases -= 1;
        }
    }

    void Update()
    {
        if (index == 0)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.position = transform.position + new Vector3(-1,0,0) * movementSpeed * Time.deltaTime; 
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position = transform.position + new Vector3(1, 0, 0) * movementSpeed * Time.deltaTime;
            }
        }

        if (index == 1)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = transform.position + new Vector3(-1, 0, 0) * movementSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = transform.position + new Vector3(1, 0, 0) * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            foreach (var item in rbs)
            {
                item.drag = activedrag;
            }
        }
        else
        {
            foreach (var item in rbs)
            {
                item.drag = drag;
            }
        }

        foreach (var item in rbs)
        {
            //item.transform.GetComponent<SpringJoint2D>().attachedRigidbody.transform.position
        }


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    time = 0;
        //    timerRunning = true;
        //}

        //timeX += timerSpeedX * Time.deltaTime;

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    if (timeX > 0.1f)
        //    {
        //        timeX = 0;



        //        bool increased = false;
        //        if (time < slowThreshold && !slow)
        //        {
        //            if (IsSweetSpot())
        //            {
        //                if (boostIncreases != maxBoostIncreases)
        //                {
        //                    boost += boostIncrease;
        //                    boostIncreases += 1;
        //                    increased = true;
        //                }
        //            }
        //            else
        //            {

        //            }
        //        }

        //        SwitchSide(increased);

        //        time = 0;
        //        timerRunning = false;
        //        slow = false;
        //    }
        //}

        //// Overtime boost 

        //if (timerRunning && time > slowThreshold)
        //{
        //    slow = true;
        //}



        //if (timerRunning)
        //{
        //    time += timerSpeed * Time.deltaTime;
        //}

        //angle = Mathf.Lerp(angle, targetAngle, Time.deltaTime * (baseSpeed + boost) * (slow ? slowMultiplier : 1) );


        //if (Mathf.Abs(angle - targetAngle) < 10)
        //{
        //   // Debug.Log("Swwww " + angle + " " + targetAngle);
        //    SwitchSide(false);
        //}



        //neck.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    float CurrentTargetAngle()
    {
        return left ? maxAngles.x : maxAngles.y;
    }

    bool IsSweetSpot()
    {
        return Mathf.Abs(angle - CurrentTargetAngle()) < sweetAngle;
    }

    
}
