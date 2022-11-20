using Mighty;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour

{

    GameObject battlePanel;
    Player player;

    [Header("Cards held by player")]
    public List<string> cardNames = new List<string>();
    public List<int> cardCounts = new List<int>();
    public List<Card> cards = new List<Card>();

    [Header("Cards held by enemy")]
    public EnemyBattle EnemyBattle;
    public Place place;

    public bool enteredBattle = false;
    private List<Card> enemyCards;
    // Start is called before the first frame update
    void Start()
    {
        enemyCards = EnemyBattle.enemyCards;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnterBattle(Player player)
    {
        Debug.Log("Entered battle");
        Debug.Log("Player has " + player.cards.Count + " cards");
        Debug.Log("Resolve: if player has more cards than enemy, player wins");
        if (player.cards.Count > enemyCards.Count)
        {
            Debug.Log("Player wins");
        }
        else
        {
            Debug.Log("Player loses");
        }
        //this.player = player;
        //foreach (Card card in player.cards)
        //{
        //    if(cardNames.Contains(card.name))
        //    {
        //        int index = cardNames.IndexOf(card.name);
        //        cardCounts[index] += 1;
        //        cards[index] = card;
        //    }
        //    else
        //    {
        //        cardNames.Add(card.name);
        //        cardCounts.Add(1);
        //        cards.Add(card);
        //    }
        //}

        //GameObject cardsHolder = MainGameManager.Instance.BattleCardsHolder;
        //for (int i = 0; i < cardsHolder.transform.childCount; i++)
        //{
        //    Destroy(cardsHolder.transform.GetChild(i).gameObject);
        //}

        //for (int i = 0; i < cards.Count; i++)
        //{
        //    Card card = cards[i];
        //    // For each card spawn the number of repeated cards
        //    for(int j = 0; j < cardCounts[i]; j++)
        //    {
        //    GameObject spawnedCard = GameObject.Instantiate(MainGameManager.Instance.battleCardPrefab, Vector3.zero, Quaternion.identity);
        //    spawnedCard.transform.parent = cardsHolder.transform;
        //    spawnedCard.GetComponent<RectTransform>().localPosition = new Vector3(-550 + (550 * i), -350, 0);
        //    spawnedCard.GetComponent<RectTransform>().localScale = new Vector3(0.5f,0.5f,0.5f);
        //    spawnedCard.GetComponent<CardVisualization>().index = i;
        //    //spawnedCard.GetComponent<CardVisualization>().village = this;
        //    spawnedCard.transform.GetChild(0).transform.Find("Illustration").gameObject.GetComponent<Image>().sprite = card.image;

        //    // Pass the unit prefab to visualization
        //    spawnedCard.GetComponent<CardVisualization>().unitPrefab = card.unitPrefab;
        //    // Juice in
        //    }

        //}

        enteredBattle = true;

        // Transit
        //MightyGameBrain.Instance.TransitToNextGameState("Battle");
    }
}
