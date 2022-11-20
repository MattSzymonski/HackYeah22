using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
    private NodeManager nodeManager;

    public Place[] places;

    public Text title;
    public Image image;
    public Text description;
    public Button enterButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        nodeManager = GameObject.Find("MapNodes").GetComponent<NodeManager>();
        title = GameObject.Find("Title").GetComponent<Text>();
        image = GameObject.Find("Image").GetComponent<Image>();
        description = GameObject.Find("Description").GetComponent<Text>();
        enterButton = GameObject.Find("EnterButton").GetComponent<Button>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetInfo(string name)
    {
        for(int i = 0; i < places.Length; ++i)
        {
            if (places[i].name.Contains(name))
            {
                title.text = places[i].name;
                image.sprite = places[i].image;
                description.text = places[i].description;
            }
        }
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
