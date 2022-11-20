using Mighty;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Village : MonoBehaviour
{
    public List<Card> cardsToOffer;
    [ReadOnly] public List<Card> cardsOffering;

    public Place place;
    public Sprite background;
    GameObject villagePanel;

    Player player;

    void Start()
    {
        villagePanel = GameObject.Find("VillagePanel");

        if (cardsToOffer.Count < 3)
        {
            MightyGameBrain.Abort("Village has less than 3 cards to offer");
        }

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, cardsToOffer.Count);
            Card card = cardsToOffer[index];
            cardsToOffer.RemoveAt(index);
            cardsOffering.Add(card);
        }
    }

    void Update()
    {
        
    }

    public bool Buy(int index)
    {
        Card card = cardsOffering[index];

        // Sell
        if (player.gold >= card.cost)
        {
            player.gold -= card.cost;
            cardsOffering[index] = null;
            player.cards.Add(card);
            MightyAudioManager.Instance.PlaySound("Coin");

            return true;
        }

        return false;
    }

    public void EnterVillage(Player player)
    {
        this.player = player;
        // Background
        MainGameManager.Instance.villageBackground.sprite = background;

        // Destroy cards and add cards
        GameObject cardsHolder = MainGameManager.Instance.villageCardsHolder;
        for (int i = 0; i < cardsHolder.transform.childCount; i++)
        {
            Destroy(cardsHolder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < 3; i++)
        {
            Card card = cardsOffering[i];
            if (card == null)
            {
                continue;
            }

            GameObject spawnedCard = GameObject.Instantiate(MainGameManager.Instance.cardPrefab, Vector3.zero, Quaternion.identity);
            spawnedCard.transform.parent = cardsHolder.transform;
            spawnedCard.GetComponent<RectTransform>().localPosition = new Vector3(-550 + (550 * i), -40, 0);
            spawnedCard.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            spawnedCard.GetComponent<CardVisualization>().index = i;
            spawnedCard.GetComponent<CardVisualization>().village = this;
            spawnedCard.transform.GetChild(0).transform.Find("Illustration").gameObject.GetComponent<Image>().sprite = card.image;
            // Juice in
        }

        // Transite
        MightyGameBrain.Instance.TransitToNextGameState("Village");

        //villagePanel.transform.Find("Cards");

      


        //yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("VillagePanel", false, true));
    }
}
