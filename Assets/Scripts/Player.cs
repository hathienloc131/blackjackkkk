using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    public List<Card> playerCards;
    public GameObject[] playerCardPosition;
    public GameObject playerCardHolder;

    private int playerMoney;
    private int playerCardPointer;
    private int playerPoint;
    // Start is called before the first frame update

    public Player(GameObject[] playerPosition, GameObject playerHolder, int money)
    {
        playerCardPosition = playerPosition;
        playerCardHolder = playerHolder;
        playerMoney = money;
        Reset();
       
    }

    public void Reset()
    {
        playerCards = new List<Card>();
        playerCardPointer = 0;
        playerPoint = 0;
    }
    public Transform AddCard(Card card, bool isPlayer)
    {
        playerCards.Add(card);
        return playerCardPosition[playerCardPointer++].transform;
    }
}
