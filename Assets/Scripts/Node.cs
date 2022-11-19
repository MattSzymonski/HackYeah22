using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Neighbouring nodes
    public List<Node> neighbours = new List<Node>();
    public CircleCollider2D circleCollider;
    public GameObject nodeLevel;
    public bool invaded = false;
    // Start is called before the first frame update
    void Start()
    {
       circleCollider = GetComponent<CircleCollider2D>(); 
       nodeLevel = transform.parent.gameObject;
       foreach(Node neighbour in neighbours)
       {
            Debug.DrawLine(transform.position, neighbour.transform.position, Color.red, 1000f);
            //var lr = gameObject.AddComponent<LineRenderer>();
            
            //lr.SetPosition(0, transform.position);
            //lr.SetPosition(1, neighbour.transform.position);

       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {
        Debug.Log("Clicked on node" + transform.parent.gameObject.name + gameObject.name);
    }
}
