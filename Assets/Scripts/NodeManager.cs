using System.Collections;
using Mighty;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodeManager : MonoBehaviour
{
    public List<GameObject> NodeLevels = new List<GameObject>();
    public List<Node> AllNodes = new List<Node>();
    public Node currentSelectedNode;
    [Header("Swede Wave tracking")]
    public int movesMade = 0;
    public int totalLevels = 3;
    public int movesForWaveProgression = 2;
    public int movesLeft = 0;
    public int waveProgression = -1;
    [Header("Player Marker")]
    public GameObject PlayerMarker;
    public float speed = 0.2f;
    // Start is called before the first frame update

    public Player player;

    public GameObject infoPanel;

    private PotopManager potopManager;
    public MainGameManager mainGameManager;

    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject level = child.gameObject;
            NodeLevels.Add(level);
            foreach (Transform nodeTransform in level.transform)
            {
                Node newNode = nodeTransform.GetComponent<Node>();
                if (newNode != null)
                {
                    AllNodes.Add(newNode);
                }
            }
        }
        currentSelectedNode = NodeLevels[0].transform.GetChild(0).gameObject.GetComponent<Node>();
        PlayerMarker.transform.position = currentSelectedNode.transform.position;

        potopManager = GameObject.Find("PotopManager").GetComponent<PotopManager>();
        mainGameManager = GameObject.Find("GameManager").GetComponent<MainGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        movesLeft = movesMade % movesForWaveProgression;

        if (PlayerMarker.transform.position != currentSelectedNode.transform.position)
        {
            PlayerMarker.transform.position = Vector3.Lerp(PlayerMarker.transform.position, currentSelectedNode.transform.position, speed*Time.deltaTime);
        }
        if (MightyGameBrain.Instance.currentGameStateName != "Village")
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //if(circleCollider.OverlapPoint(mousePos))
                //{
                //    Debug.Log("Clicked on node");
                //}
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
                    
                if(hit.collider != null)
                {
                    Node clickedNode = hit.collider.transform.gameObject.GetComponent<Node>();
                    if (currentSelectedNode.neighbours.Contains(clickedNode))
                    {
                        // Check if current node is invaded or not
                        if (clickedNode.invaded == false)
                        {
                            currentSelectedNode = clickedNode;
                            // move player marker
                            movesMade++;
                            PlayerMarkerMoved();
                        }
                    }

                }
            }
        }
    }

    void PlayerMarkerMoved()
    {
        if (movesMade % movesForWaveProgression == 0)
        {
            waveProgression++;
            potopManager.AdvanceWave(currentSelectedNode);
            // TODO CHECK IF WAVE PROGRESSION IS EQUAL TO TOTAL LEVELS

            foreach(Transform node in NodeLevels[waveProgression].transform)
            {
                node.gameObject.GetComponent<Node>().invaded = true;
                node.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        // Display the UI info about this node
        // If village exists, enter village
        Village village = currentSelectedNode.gameObject.GetComponent<Village>();
        // If Battle exists, enter Battle
        Battle battle = currentSelectedNode.gameObject.GetComponent<Battle>();
        var infoPanelManager = infoPanel.GetComponent<InfoPanelManager>();
        if (village)
            infoPanelManager.SetInfo(village.name);
        else if (battle)
            infoPanelManager.SetInfo(battle.name);

        mainGameManager.OpenInfoPanel();
    }

    public void EnterNode()
    {
        if (!currentSelectedNode)
            return;

        Village village = currentSelectedNode.gameObject.GetComponent<Village>();
        // If Battle exists, enter Battle
        Battle battle = currentSelectedNode.gameObject.GetComponent<Battle>();
        
        village?.EnterVillage(player);
        if (battle && !battle.enteredBattle)
        {
            battle.EnterBattle(player);
        }

        mainGameManager.CloseInfoPanel();
    }
}
