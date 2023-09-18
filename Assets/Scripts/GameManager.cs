using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField]
	private GameObject[] cardPrefabs, playerCardPosition1, playerCardPosition2, playerCardPosition3, playerCardPosition4, playerCardPosition5, dealerCardPosition;
	[SerializeField]
	private Button primaryBtn, secondaryBtn, resetBalanceBtn;
    [SerializeField]
	private Slider betSlider, numSlider;
    [SerializeField]
    private GameObject cardHolder1, cardHolder2, cardHolder3, cardHolder4, cardHolder5, dealerCardHolder, mainDeck;
    [SerializeField]
    private TMP_Text textPlayerPoints, textDealerPoints, textWinner;
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
    private int turn = 0;
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
        currentBet = 50;
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
            players.Add(new Player(playerCardPosition3, cardHolder3, playerMoney));
        }
        if (numPlayer > 1)
        {
            players.Add(new Player(playerCardPosition2, cardHolder2, playerMoney));
        }        
        if (numPlayer > 2)
        {
            players.Add(new Player(playerCardPosition1, cardHolder1, playerMoney));
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

        Debug.Log("reset ..");

        resetGame();

        
		primaryBtn.onClick.AddListener(delegate {
			if (isPlaying) {
                if (turn % numPlayer == playerPosition)
                {
                    if (players[playerPosition].playerCardPointer < 5)
                    {
                        playerDrawCard(playerPosition);
                    }
                    // turn += 1;
                }
			} else {
				startGame();
			}
		});

		secondaryBtn.onClick.AddListener(delegate {
            playerEndTurn();
			turn += 1;
		});
    }


	private void revealDealersDownFacingCard() {
		// reveal the dealer's down-facing card
		Destroy(dealerCardHolder.transform.GetChild(0).gameObject);
		Instantiate(dealerCards[0].Prefab, dealerCardPosition[0].transform.position, Quaternion.identity, dealerCardHolder.transform);
	}

	private void playerEndTurn() {
		revealDealersDownFacingCard();
        int playerPoint = players[playerPosition].playerPoints;
		// dealer start drawing
		while (actualDealerPoints < 17 && actualDealerPoints < playerPoint) {
			dealerDrawCard();
		}
		updateDealerPoints(false);
		if (actualDealerPoints > 21)
        {
            if (playerPoint > 21)
            {
                gameDraw();
            }
            else
            {
                dealerBusted();
            }
        }
		else if (actualDealerPoints > playerPoint)
		{
            dealerWin(false);
        }
		else if (actualDealerPoints == players[playerPosition].playerPoints)
			gameDraw();
		else
        {
            if (playerPoint > 21)
			    playerBusted();
            else
                playerWin(false);
        }
	}
	public void endGame() {
		primaryBtn.gameObject.SetActive(false);
		secondaryBtn.gameObject.SetActive(false);
		// betSlider.gameObject.SetActive(false);
		// textPlaceYourBet.text = "";
		// textSelectingBet.text = "";

		// resetImgBtn.gameObject.SetActive(true);
		// resetImgBtn.GetComponent<Button>().onClick.AddListener(delegate {
		// 	resetGame();
		// });
	}
	private void dealerWin(bool winByBust) {
		if (winByBust)
			textWinner.text = "Player Busted\nDealer Win !!!";
		else
			textWinner.text = "Dealer Win !!!";
		endGame();
	}
	private void playerWin(bool winByBust) {
		if (winByBust)
			textWinner.text = "Dealer Busted\nPlayer Win !!!";
		else
			textWinner.text = "Player Win !!!";
		playerMoney += currentBet * 2;
		endGame();
	}
	private void gameDraw() {
		textWinner.text = "Draw";
		playerMoney += currentBet;
		endGame();
	}
	private void playerBusted() {
		dealerWin(true);
	}


	private void dealerBusted() {
		playerWin(true);
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
			// primaryBtn.GetComponentInChildren<Text>().text = "HIT";
			secondaryBtn.gameObject.SetActive(true);

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
			updateDealerPoints(true);

			checkIfPlayerBlackjack();
		}
	}

    private void checkIfPlayerBlackjack()
	{
		if (players[playerPosition].playerPoints == 21)
		{
			playerBlackjack();
		}
	}


	private void playerBlackjack() {
		textWinner.text = "Blackjack !!!";
		playerMoney += currentBet * 2;
		endGame();
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
		    textPlayerPoints.text = players[playerPosition].playerPoints.ToString();
        }
        else
        {
			angle = Quaternion.Euler(new Vector3(180, transform.eulerAngles.y,  transform.eulerAngles.z));
        }

		Instantiate(drawnCard.Prefab, mainDeck.transform.position, angle, players[position].playerCardHolder.transform);
        Debug.Log(players[playerPosition].playerPoints);
    }

    public void dealerDrawCard() {
		Card drawnCard = playingDeck.DrawRandomCard();
		Quaternion angle;
		dealerCards.Add(drawnCard);
        Debug.Log(dealerCardPointer);
        Transform transform = dealerCardPosition[dealerCardPointer].transform;

		if (dealerCardPointer > 0) {
			angle = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y,  transform.eulerAngles.z));
		} else {
			angle = Quaternion.Euler(new Vector3(180, transform.eulerAngles.y,  transform.eulerAngles.z));
		}

		Instantiate(drawnCard.Prefab, mainDeck.transform.position, angle, dealerCardHolder.transform);
        dealerCardPointer++;
		updateDealerPoints(false);

	}

    private void updateDealerPoints(bool hideFirstCard) {
    actualDealerPoints = 0;
    foreach(Card c in dealerCards) {
        actualDealerPoints += c.Point;
    }

    // transform ace to 1 if there is any
    if (actualDealerPoints > 21)
    {
        actualDealerPoints = 0;
        foreach(Card c in dealerCards) {
            if (c.Point == 11)
                actualDealerPoints += 1;
            else
                actualDealerPoints += c.Point;
        }
    }

    if (hideFirstCard)
        displayDealerPoints = dealerCards[1].Point;
    else
        displayDealerPoints = actualDealerPoints;
    textDealerPoints.text = displayDealerPoints.ToString();
	}
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numPlayer; i++)
        {
            for (int j = 0; j < players[i].playerCardHolder.transform.childCount; j++)
            {
                Transform cardPosition = players[i].playerCardPosition[j].transform;
                Transform transform = players[i].playerCardHolder.transform.GetChild(j).gameObject.transform;
                if (Vector3.Distance(transform.position, cardPosition.position) > 0.001f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, cardPosition.position, 1 * Time.deltaTime);
                }
            }
        }
        for (int j = 0; j < dealerCardHolder.transform.childCount; j++)
        {
            Transform cardPosition = dealerCardPosition[j].transform;
            Transform transform = dealerCardHolder.transform.GetChild(j).gameObject.transform;
            if (Vector3.Distance(transform.position, cardPosition.position) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, cardPosition.position, 1 * Time.deltaTime);
            }
        }
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

        primaryBtn.gameObject.SetActive(true);
		// primaryBtn.GetComponentInChildren<Text>().text = "DEAL";
		secondaryBtn.gameObject.SetActive(false);
	    textPlayerPoints.text = "";
		textDealerPoints.text = "";
		textWinner.text = "";
       
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
