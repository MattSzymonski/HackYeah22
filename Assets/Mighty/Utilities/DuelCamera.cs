using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

//Lerps camera to the target position in the middle of distance between two players, also lerps rotation to always look at point between them
namespace Mighty
{
    public class DuelCamera : MonoBehaviour
    {
        public GameObject player1;
        public GameObject player2;

        public bool moveToAction;
        public bool lookAtAction;

        [Header("Move To Action")] [ShowIf("moveToAction")] public float movementSmoothness;
        [ShowIf("moveToAction")] public float length;
        [ShowIf("moveToAction")] public float height;


        [Header("Look At Action")] [ShowIf("lookAtAction")] public float lookAtSmoothness;
        [ShowIf("lookAtAction")] public Vector3 lookAtTargetOffset;

        [Space(20)]
        public bool drawGizmos;

        Vector3 middleLinePoint;
        Vector3 direction;
        Vector3 targetPoint;

        void Update()
        {
            Move();
        }

        void Move()
        {
            //if (player1 && player2 && gameManager.gameState == Mighty.GameState.Playing)
            {
                middleLinePoint = player1.transform.position - (player1.transform.position - player2.transform.position) * 0.5f;
                targetPoint = middleLinePoint + MightyUtilites.PerpendicularToLine(player1.transform.position, player2.transform.position) * length + new Vector3(0, height, 0);

                if (moveToAction)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPoint, movementSmoothness * Time.deltaTime);
                }

                if (lookAtAction)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(middleLinePoint + lookAtTargetOffset - transform.position, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookAtSmoothness * Time.deltaTime);
                }      
            }
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos && player1 && player2)
            {
                Vector3 middleLinePointGizmo = player1.transform.position - (player1.transform.position - player2.transform.position) * 0.5f;
                Vector3 lengthPoint = middleLinePointGizmo + MightyUtilites.PerpendicularToLine(player1.transform.position, player2.transform.position) * length;

                if (moveToAction)
                {                  
                    Gizmos.DrawLine(player1.transform.position, player2.transform.position);                  
                    Gizmos.DrawLine(middleLinePointGizmo, lengthPoint);
                    Gizmos.DrawLine(lengthPoint, lengthPoint + new Vector3(0, height, 0));
                    Gizmos.DrawSphere(lengthPoint + new Vector3(0, height, 0), 0.5f);
                }
               
                if (lookAtAction)
                {
                    Gizmos.DrawSphere(middleLinePointGizmo + lookAtTargetOffset, 0.5f);
                    Gizmos.DrawLine(transform.position, middleLinePointGizmo + lookAtTargetOffset);
                }      
            }     
        }
    }
}