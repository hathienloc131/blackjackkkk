using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField]
	public GameObject[] cardPrefabs, playerCardPosition1, playerCardPosition2, playerCardPosition3, playerCardPosition4, playerCardPosition5, dealerCardPosition;
	// [SerializeField]
	// public Button primaryBtn, secondaryBtn, resetBalanceBtn;
    [SerializeField]
    public GameObject primaryBtn_, secondaryBtn_;
    [SerializeField]
	public Slider betSlider, numSlider;
    [SerializeField]
    public GameObject cardHolder1, cardHolder2, cardHolder3, cardHolder4, cardHolder5, dealerCardHolder, mainDeck;
    [SerializeField]
    public TMP_Text textPlayerPoints, textDealerPoints, textWinner;
    public int numPlayer = 4;

    public List<Player> players;
	public List<Card> dealerCards = new List<Card>(); 
	public int dealerCardPointer;
    public int playerPosition = 0;

    public GameObject holders;
    public bool isPlaying;
	public int playerPoints;
	public int actualDealerPoints, displayDealerPoints;
    public int playerMoney;
	public int currentBet;
    public int turn = 0;
    public bool clearBoard = false;
    public bool ending = false;
    // Start is called before the first frame update
	public Deck playingDeck;

    void _addPhysicComponent(GameObject gO)
    {
        // gO.AddComponent<Rigidbody>();
        // gO.AddComponent<BoxCollider>();
    }
    void Start()
    {
        playerMoney = 5000;
        currentBet = 50;
        clearBoard = false;
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

    }

    

    IEnumerator SleepCoroutine(int second)
    {
        yield return new WaitForSeconds(second);
    }
	private void revealDealersDownFacingCard() {
		// reveal the dealer's down-facing card
		Destroy(dealerCardHolder.transform.GetChild(0).gameObject);
		Instantiate(dealerCards[0].Prefab, dealerCardPosition[0].transform.position, Quaternion.identity, dealerCardHolder.transform);
        StartCoroutine(SleepCoroutine(5));
	}

	private void playerEndTurn() {
		revealDealersDownFacingCard();
        int playerPoint = players[playerPosition].playerPoints;
		// dealer start drawing
		while (actualDealerPoints < 17 && actualDealerPoints < playerPoint) {
			dealerDrawCard();
            StartCoroutine(SleepCoroutine(5));
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
		primaryBtn_.gameObject.SetActive(true);
		primaryBtn_.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.63f, 0.42f, 0.1f, 1.0f));

		secondaryBtn_.gameObject.SetActive(false);
		// betSlider.gameObject.SetActive(false);
		// textPlaceYourBet.text = "";
		// textSelectingBet.text = "";

		// resetImgBtn.gameObject.SetActive(true);
		// resetImgBtn.GetComponent<Button>().onClick.AddListener(delegate {
		// 	resetGame();
		// });
        isPlaying = false;
        ending = true;
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
            clearBoard = false;
            clearCards();
			playerMoney -= currentBet;
			if (playerMoney < 0) {
				playerMoney += currentBet;
				betSlider.maxValue = playerMoney;
				return;
			}

			isPlaying = true;

			// Update UI accordingly
            primaryBtn_.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.14f, 0.11f, 0.11f, 1.0f));
			// primaryBtn.GetComponentInChildren<Text>().text = "HIT";
			secondaryBtn_.gameObject.SetActive(true);

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
			angle = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y ,  transform.eulerAngles.z));
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
                Transform transform = players[i].playerCardHolder.transform.GetChild(j).gameObject.transform;
                Transform cardPosition;
                if (clearBoard)
                {
                    cardPosition = mainDeck.transform;
                }
                else
                {
                    cardPosition = players[i].playerCardPosition[j].transform;
                }
                if (Vector3.Distance(transform.position, cardPosition.position) > 0.001f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, cardPosition.position, 1 * Time.deltaTime);
                }
            }
        }
        for (int j = 0; j < dealerCardHolder.transform.childCount; j++)
        {
            Transform transform = dealerCardHolder.transform.GetChild(j).gameObject.transform;
            Transform cardPosition;
            if (clearBoard)
            {
                cardPosition = mainDeck.transform;
            }
            else
            {
                cardPosition = dealerCardPosition[j].transform;
            }
            if (Vector3.Distance(transform.position, cardPosition.position) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, cardPosition.position, 1 * Time.deltaTime);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;
            
            if( Physics.Raycast( ray, out hit, 100 ) )
            {
                if ( hit.transform.gameObject.name == "HitButton")
                {
                    hit.transform.gameObject.transform.localScale = new Vector3(0.1f,0.03f,0.05f);
                    if (isPlaying) {
                        if (turn % numPlayer == playerPosition)
                        {
                            if (players[playerPosition].playerCardPointer < 5)
                            {
                                playerDrawCard(playerPosition);
                            }
                            // turn += 1;
                        }
                    } 
                    else if (ending)
                    {
                        clearBoard = true;
                        resetGame();
                    } else {
                        startGame();
                    }
                }
                if ( hit.transform.gameObject.name == "StandButton")
                {
                    hit.transform.gameObject.transform.localScale = new Vector3(0.1f,0.03f,0.05f);
                    playerEndTurn();
                    turn += 1;
                }
            }
            // Debug.Log("clickedd object");
        } 
        else{
            primaryBtn_.transform.localScale = new  Vector3(0.1f,0.05f,0.05f);
            secondaryBtn_.transform.localScale = new  Vector3(0.1f,0.05f,0.05f);
        }


    }
    


    private void resetGame() {
		isPlaying = false;
		ending = false;
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

        primaryBtn_.gameObject.SetActive(true);
		primaryBtn_.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.56f, 0.11f, 0.11f, 1.0f));
		secondaryBtn_.gameObject.SetActive(false);
	    textPlayerPoints.text = "";
		textDealerPoints.text = "";
		textWinner.text = "";
       
		// clear cards on table
		// clearCards();
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
