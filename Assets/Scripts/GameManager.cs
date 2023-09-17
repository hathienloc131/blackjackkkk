using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    [SerializeField]
	private GameObject[] cardPrefabs, playerCardPosition1, playerCardPosition2, playerCardPosition3, playerCardPosition4, playerCardPosition5, dealerCardPosition;
	[SerializeField]
	private Button primaryBtn, secondaryBtn, resetBalanceBtn;
    [SerializeField]
	private Slider betSlider, numSlider;
    [SerializeField]
    private GameObject cardHolder1, cardHolder2, cardHolder3, cardHolder4, cardHolder5, dealerCardHolder;
    public int numPlayer = 4;

    private List<Player> players;
	private List<Card> dealerCards = new List<Card>(); 
	private int dealerCardPointer;
    public int playerPosition = 0;

    private GameObject holders;
    private bool isPlaying;
	private int playerPoints;
	private int actualDealerPoints, displayDealerPoints;
    private int playerMoney;
	private int currentBet;
    private int turn = 1;
    // Start is called before the first frame update
	private Deck playingDeck;

    void _addPhysicComponent(GameObject gO)
    {
        // gO.AddComponent<Rigidbody>();
        // gO.AddComponent<BoxCollider>();
    }
    void Start()
    {
        playerMoney = 5000;
        Debug.Log("start");
        players = new List<Player>();
        //Add collision and rigidbody for CardPrefab
        foreach (GameObject g in cardPrefabs)
        {
            _addPhysicComponent(g);
        }


        // Create list of player position 
        if (numPlayer > 0)
        {
            players.Add(new Player(playerCardPosition1, cardHolder1, playerMoney));
        }
        if (numPlayer > 1)
        {
            players.Add(new Player(playerCardPosition2, cardHolder2, playerMoney));
        }        
        if (numPlayer > 2)
        {
            players.Add(new Player(playerCardPosition3, cardHolder3, playerMoney));
        }        
        if (numPlayer > 3)
        {
            players.Add(new Player(playerCardPosition4, cardHolder4, playerMoney));
        }
        if (numPlayer > 4)
        {
            players.Add(new Player(playerCardPosition5, cardHolder5, playerMoney));
        }

        playerMoney = 1000;
		currentBet = 50;
        // Instantiate(holders);
        Debug.Log("reset ..");

        resetGame();
        Debug.Log("start ..");

        startGame();
    }


	public void startGame() {
		if (playerMoney > 0)
		{
			playerMoney -= currentBet;
			if (playerMoney < 0) {
				playerMoney += currentBet;
				betSlider.maxValue = playerMoney;
				return;
			}

			isPlaying = true;

			// Update UI accordingly
	

			// assign the playing deck
			playingDeck = new Deck(cardPrefabs);
			// draw 2 cards for player
            for (int step = 0; step < 2; step++)
            {       
                for (int i = 0; i < numPlayer; i++)
                {
                    playerDrawCard(i);
                }
            }
            Debug.Log("start ok ");
			// updatePlayerPoints();
			// draw 2 cards for dealer
			dealerDrawCard();
			dealerDrawCard();
			// updateDealerPoints(true);

			// checkIfPlayerBlackjack();
		}
	}
	public void playerDrawCard(int position) {
		Card drawnCard = playingDeck.DrawRandomCard();
        bool isPlayer = position == playerPosition;
        Quaternion angle;
        Transform transform = players[position].AddCard(drawnCard, isPlayer);
        // Quaternion
        if (isPlayer)
        {
			angle = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y,  transform.eulerAngles.z));
        }
        else
        {
			angle = Quaternion.Euler(new Vector3(180, transform.eulerAngles.y,  transform.eulerAngles.z));
        }

		Instantiate(drawnCard.Prefab, transform.position, angle, players[position].playerCardHolder.transform);

	}

    public void dealerDrawCard() {
		Card drawnCard = playingDeck.DrawRandomCard();
		Quaternion angle;
		dealerCards.Add(drawnCard);
        Debug.Log(dealerCardPointer);
        Transform transform = dealerCardPosition[dealerCardPointer].transform;

		if (dealerCardPointer <= 0) {
			angle = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y,  transform.eulerAngles.z));
		} else {
			angle = Quaternion.Euler(new Vector3(180, transform.eulerAngles.y,  transform.eulerAngles.z));
		}



		Instantiate(drawnCard.Prefab, transform.position, angle, dealerCardHolder.transform);
        dealerCardPointer++;
	}
    // Update is called once per frame
    void Update()
    {
        
    }


    private void resetGame() {
		isPlaying = false;
		
		// reset points
		for (int i = 0; i < numPlayer; i++)
        {
            players[i].Reset();
        }
		actualDealerPoints = 0;
		dealerCardPointer = 0;

		// reset cards
		playingDeck = new Deck(cardPrefabs);
		dealerCards = new List<Card>();

		// clear cards on table
		clearCards();
        Debug.Log("reset ok");
	}

	private void clearCards() {
		
        for (int i = 0; i < numPlayer; i++)
        {
            for (int j = 0; j < players[i].playerCardHolder.transform.childCount; j++)
            {
                Destroy(players[i].playerCardHolder.transform.GetChild(j).gameObject);
            }
        }
        for (int i = 0; i < dealerCardHolder.transform.childCount; i++)
        {
            Destroy(dealerCardHolder.transform.GetChild(i).gameObject);
        }
		
	}
}
