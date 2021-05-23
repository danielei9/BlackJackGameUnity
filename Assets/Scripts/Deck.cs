using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text textBet;
    public Text textBankAcc;
    public Text probMessage;
    public float probDealer = 0f, prob17_21 = 0f, probOvertake = 0f;
    public int[] values = new int[52];
    int cardIndex = 0;
    float bankAcc = 1000f;
    float betTotal = 0f;

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
        textBet = GameObject.Find("TextBet").GetComponent<Text>();
        textBankAcc = GameObject.Find("TextBankAcc").GetComponent<Text>();
        textBankAcc.text = "1000€";
        textBet.text = "0€";
    }
    public void Bet()
    {
        betTotal += 10;
        textBet.text = betTotal.ToString();
    }
    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int count = 0;
        int letterCardCount = 0;

        for (int i = 0; i < values.Length; i++)
        { // Si vale mas que nueve asignar su valor a la carta de letras 
            if (count >= 10)
            {
                letterCardCount++;
            }
            if (count < 10) // si vale menos que nueve la cuenta es equivalente al valor de la carta
            {
                count++;
            }
            values[i] = count;
            //Debug.Log(values[i]);
            if (letterCardCount == 3) // en caso de llegar a la k reset son 3 por que el 10 tmb lo cuenta
            {
                letterCardCount = 0;
                count = 0;
            }
        }

    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        /*
         
      faces.Length: Un entero que señala la cantidad total de ítems que tiene el array.
      i: Un entero, que rige la cuenta del bucle.
      randomNumber: Un entero, elegido por una función Random en el rango 0-k (nótese que k se va reduciendo).
      tmpFace: Un entero, que ha de contener un valor para intercambiar valores entre 2 posiciones.

         */
        int tmpValues;
        int randomNumber;
        Sprite tmpFace;
        //Sprite[] outFace;
        for (int i = 0; i < faces.Length; i++)
        {
            randomNumber = Random.Range(0, 52);

            tmpValues = values[randomNumber];
            values[randomNumber] = values[i];
            values[i] = tmpValues;

            tmpFace = faces[randomNumber];
            faces[randomNumber] = faces[i];
            faces[i] = tmpFace;
            // Debug.Log(faces[i]);
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            chekBlackjack();
        }
        CalculateProbabilities();
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        // valor de nuestra mano + posibilidades // as ->1 // 10, k, j, q -> 10
        // int[] posibleCardValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };
        CardModel[] playerHandCards = new CardModel[2];
        CardModel[] dealerHandCards = new CardModel[2];
        float countPosiblesOvertake = 0;
        float countPosiblesPlayer = 0;
        float countPosiblesDealer = 0;
        float ifTakeNewCardValue = 0;

        playerHandCards[0] = player.GetComponent<CardHand>().cards[0].GetComponent<CardModel>();
        dealerHandCards[0] = player.GetComponent<CardHand>().cards[0].GetComponent<CardModel>();
        playerHandCards[1] = player.GetComponent<CardHand>().cards[1].GetComponent<CardModel>();
        dealerHandCards[1] = player.GetComponent<CardHand>().cards[1].GetComponent<CardModel>();

        float playerHandValue = player.GetComponent<CardHand>().points;
        //int dealerHandValue = dealerHandCards[0].value + dealerHandCards[1].value;

        float totalCount = 0;

        bool deletePlayerCard0 = true;
        bool deletePlayerCard1 = true;
        float posibleDealerValue = 0;

        for (int i = 0; i < values.Length; i++)
        {
            // si el valor es el mismo que el de la carta 0 del jugador no lo cuenta una vez
            if (values[i] == playerHandCards[0].value && deletePlayerCard0)
            {
                deletePlayerCard0 = false;
            }
            // si el valor es el mismo que el de la carta 1 del jugador no lo cuenta una vez
            if (values[i] == playerHandCards[1].value && deletePlayerCard1)
            {
                deletePlayerCard1 = false;
            }
            else
            {
                // una vez quitadas las cartas del jugador
                // contamos cuantas hay en total sin contar las dos citadas anteriores
                totalCount++;

                ifTakeNewCardValue = playerHandValue + values[i];
                //  - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
                if (ifTakeNewCardValue >= 17 && ifTakeNewCardValue <= 21)
                {
                    countPosiblesPlayer++;
                }
                //* - Probabilidad de que el jugador obtenga más de 21 si pide una carta        
                if (ifTakeNewCardValue > 21)
                {
                    countPosiblesOvertake++;
                }

            }
            //-Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
            posibleDealerValue = player.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value + values[i];
            if (posibleDealerValue > playerHandValue)
            {
                countPosiblesDealer++;
            }
        }

        probDealer = (countPosiblesDealer * 100f) / totalCount;
        prob17_21 = (countPosiblesPlayer * 100f) / totalCount;
        probOvertake = (countPosiblesOvertake * 100f) / totalCount;

        /*Debug.Log("probDealer "+probDealer);
        Debug.Log("prob17_21" + prob17_21);
        Debug.Log("probOvertake" + probOvertake);
        Debug.Log("countPosiblesOvertake" + countPosiblesOvertake);
        Debug.Log("countPosiblesPlayer" + countPosiblesPlayer);
        Debug.Log("countPosiblesDealer" + countPosiblesDealer);*/

        probMessage.text = "P(17-21): " + prob17_21.ToString() + "%  " + "p(D>P): " + probDealer.ToString() + "%  " + "  p(Overtake): " + probOvertake.ToString();

    }


    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        chekBlackjack();
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;

    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.color = Color.blue;
            finalMessage.text = "U LoSe";
            // Invoke("PlayAgain", 5f);
            bankAcc -= betTotal;
            betTotal = 0;
        }
        else {
            finalMessage.text = "";
        }
        chekBlackjack();
        CalculateProbabilities();
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        if (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();
            if (dealer.GetComponent<CardHand>().points > 21)
            {
                finalMessage.color = Color.green;
                finalMessage.text = "U WiN";
            }

        }
        else
        {
            if (player.GetComponent<CardHand>().points > 21)
            {
                finalMessage.color = Color.red;
                finalMessage.text = "U LoSe";
                //Invoke("PlayAgain", 5f);
                bankAcc -= betTotal;
                betTotal = 0;
            }
            if (dealer.GetComponent<CardHand>().points > 21)
            {
                finalMessage.color = Color.green;
                finalMessage.text = "U WiN";
                // Invoke("PlayAgain", 5f);
                bankAcc += betTotal;
                betTotal = 0;
            }
            if (dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points )
            {
                finalMessage.color = Color.green;
                finalMessage.text = "U WiN";
                // Invoke("PlayAgain", 5f);ç
                bankAcc += betTotal;
                betTotal = 0;
            }
            if (dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points )
            {
                finalMessage.color = Color.red;
                finalMessage.text = "U LoSe";
                //Invoke("PlayAgain", 5f);
                bankAcc -= betTotal;
                betTotal = 0;
            }
            if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
            {
                finalMessage.color = Color.yellow;
                finalMessage.text = "Same";
                //Invoke("PlayAgain", 5f);
                betTotal = 0;
            }
        }
        chekBlackjack();
        CalculateProbabilities();
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
        textBankAcc.text = bankAcc.ToString();
        textBet.text = "0€";
    }
    private void chekBlackjack()
    {
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "U WiN";
            finalMessage.color = Color.green;
            bankAcc += betTotal;
            betTotal = 0;
        }
        if (dealer.GetComponent<CardHand>().points == 21)
        {
            finalMessage.color = Color.red;
            finalMessage.text = "U LOsE";
            bankAcc -= betTotal;
            betTotal = 0;
        }
        if (dealer.GetComponent<CardHand>().points == 21 && player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.color = Color.yellow;
            finalMessage.text = "2 BlackJack WOW";
            betTotal = 0;
        }

    }
}
