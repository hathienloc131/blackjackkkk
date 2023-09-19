using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    public List<Card> playerCards;
    public GameObject[] playerCardPosition;
    public GameObject playerCardHolder;

    public int playerMoney;
    public int playerCardPointer;
    public int playerPoints;
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
        playerPoints = 0;
    }
    public Transform AddCard(Card card, bool isPlayer)
    {
        playerCards.Add(card);
        updatePlayerPoints();
        return playerCardPosition[playerCardPointer++].transform;
    }
    private void updatePlayerPoints() {
        playerPoints = 0;
        foreach(Card c in playerCards) {
            playerPoints += c.Point;
        }

        // transform ace to 1 if there is any
        if (playerPoints > 21)
        {
            playerPoints = 0;
            foreach(Card c in playerCards) {
                if (c.Point == 11)
                    playerPoints += 1;
                else
                    playerPoints += c.Point;
            }
        }
		// textPlayerPoints.text = playerPoints.ToString();
	}
}
