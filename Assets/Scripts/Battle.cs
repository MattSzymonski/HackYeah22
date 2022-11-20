using Mighty;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<Card> enemyCards = new List<Card>();


    public List<GameObject> playerUnits = new List<GameObject>();
    public List<GameObject> enemyUnits = new List<GameObject>();

    public bool battleWon;

    // Start is called before the first frame update
    void Start()
    {
        player = MainGameManager.Instance.player.GetComponent<Player>();
        enemyCards = EnemyBattle.enemyCards;
    }

    List<int> playerIndicesToRemove = new List<int>();
    int enemyUnitGroupsDead = 0;

    // Update is called once per frame
    void Update()
    {
        if (!battleWon && MainGameManager.Instance.currentBattle == this)
        {
            // Player
            foreach (var item in playerUnits)
            {
                if (item.GetComponent<UnitGroup>().squadMembers.All(x => !x.active))
                {
                    if (!item.GetComponent<UnitGroup>().dead)
                    {
                        playerIndicesToRemove.Add(item.GetComponent<UnitGroup>().index);
                        item.GetComponent<UnitGroup>().dead = true;
                    }
                }
            }

            if (playerIndicesToRemove.Count == player.cards.Count)
            {
                MightyGameBrain.Instance.TransitToNextGameState("GameOver");
            }

            // Enemy
            foreach (var item in enemyUnits)
            {
                if (item.GetComponent<UnitGroup>().squadMembers.All(x => !x.active))
                {
                    if (!item.GetComponent<UnitGroup>().dead)
                    {
                        enemyUnitGroupsDead++;
                        item.GetComponent<UnitGroup>().dead = true;
                    }
                }
            }

            if (enemyUnitGroupsDead == enemyCards.Count)
            {
                BattleWon();
            }
        }
    }
      

    public void BattleWon()
    {
        Debug.Log("xxxxxxxxxxxxxxxxxxx won");

        MightyGameBrain.Instance.TransitToNextGameState("Map");
        player.gold += Random.Range(2, 5);
        battleWon = true;

        for (int i = player.cards.Count - 1; i >= 0; i--)
        {
            if (playerIndicesToRemove.Contains(i))
            {
                player.cards.RemoveAt(i);
            }

            //if (i==)
            //Card objToDestroy = player.cards[i];
            //Destroy(objToDestroy);
        }

        for (int i = enemyUnits.Count - 1; i >= 0; i--)
        {
            Destroy(enemyUnits[i].gameObject);
        }

        for (int i = playerUnits.Count - 1; i >= 0; i--)
        {
            Destroy(playerUnits[i].gameObject);
        }

    }

    public void EnterBattle(Player player)
    {

        GameObject pare = new GameObject();
        //Debug.Log("Entered battle");
        //Debug.Log("Player has " + player.cards.Count + " cards");
        //Debug.Log("Resolve: if player has more cards than enemy, player wins");
        //if (player.cards.Count > enemyCards.Count)
        //{
        //    Debug.Log("Player wins");
        //}
        //else
        //{
        //    Debug.Log("Player loses");
        //}
        this.player = player;
        foreach (Card card in player.cards)
        {
            if(cardNames.Contains(card.name))
            {
                int index = cardNames.IndexOf(card.name);
                cardCounts[index] += 1;
                cards[index] = card;
            }
            else
            {
                cardNames.Add(card.name);
                cardCounts.Add(1);
                cards.Add(card);
            }
        }

        GameObject cardsHolder = MainGameManager.Instance.BattleCardsHolder;
        for (int i = 0; i < cardsHolder.transform.childCount; i++)
        {
            Destroy(cardsHolder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            // For each card spawn the number of repeated cards
            for(int j = 0; j < cardCounts[i]; j++)
            {
            GameObject spawnedCard = GameObject.Instantiate(MainGameManager.Instance.battleCardPrefab, Vector3.zero, Quaternion.identity);
            spawnedCard.transform.parent = cardsHolder.transform;
            spawnedCard.GetComponent<RectTransform>().localPosition = new Vector3(-550 + (550 * i), -350, 0);
            spawnedCard.GetComponent<RectTransform>().localScale = new Vector3(0.5f,0.5f,0.5f);
            spawnedCard.GetComponent<CardVisualization>().index = i;
             //   spawnedCard.GetComponent<CardVisualization>().index = i;

                spawnedCard.GetComponent<CardVisualization>().battle =this;
                //spawnedCard.GetComponent<CardVisualization>().village = this;
                spawnedCard.transform.GetChild(0).transform.Find("Illustration").gameObject.GetComponent<Image>().sprite = card.image;

            // Pass the unit prefab to visualization
            spawnedCard.GetComponent<CardVisualization>().unitPrefab = card.unitPrefab;
            // Juice in
            }

        }

        enteredBattle = true;

        // Transit
        MainGameManager.Instance.currentBattle = this;
        MightyGameBrain.Instance.TransitToNextGameState("Battle");
    }

    public void SpawnEnemies()
    {
        Debug.Log("Spawning enemies");
        for (int i = 0; i < EnemyBattle.enemyCards.Count; ++i)
        {
            Vector3 spawnLocation = EnemyBattle.enemyPositions[i];
            GameObject unit = Instantiate(enemyCards[i].unitPrefab, spawnLocation, Quaternion.identity) as GameObject;
            unit.transform.position = new Vector3(spawnLocation.x, spawnLocation.y, 5);
            unit.GetComponent<UnitGroup>().Spawn();
            enemyUnits.Add(unit);
        }
        
    }
}
