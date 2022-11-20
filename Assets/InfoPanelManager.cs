using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
    private NodeManager nodeManager;

    public Text title;
    public Image image;
    public Text description;
    public Button enterButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        nodeManager = GameObject.Find("Map").GetComponentInChildren<NodeManager>();
        title = GameObject.Find("Title").GetComponentInChildren<Text>();
        image = GameObject.Find("Image").GetComponent<Image>();
        description = GameObject.Find("Description").GetComponentInChildren<Text>();
        enterButton = GameObject.Find("EnterButton").GetComponent<Button>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetInfo(Place place)
    {
        title.text = place.name;
        image.sprite = place.image;
        description.text = place.description;
    }

    public void EnterButtonPressed()
    {
        nodeManager.EnterNode();
    }
    
    public void ExitButtonPressed()
    {
        nodeManager.mainGameManager.CloseInfoPanel();
    }
}
