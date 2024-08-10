using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class Solitare : Singleton<Solitare>
{
    public Sprite[] CardFaces;
    public GameObject cardPrefab;
    public List<GameObject> playPos, scorePos;
    [HideInInspector]
    public List<GameObject> slotPos; //Prev: bottomPos
    public GameObject deckPos;
    public GameObject discardPos;

    public float cardPadding = .006f;

    public static string[] suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
    [HideInInspector]
    public List<string> cardNames = new List<string>();
    public Dictionary<string, GameObject> cards = new Dictionary<string, GameObject>();


    public List<string>[] cardSlots;  //Prev: Bottoms

    private List<string> playSlot0 = new List<string>(); //Prev: bottom0
    private List<string> playSlot1 = new List<string>();
    private List<string> playSlot2 = new List<string>();
    private List<string> playSlot3 = new List<string>();
    private List<string> playSlot4 = new List<string>();
    private List<string> playSlot5 = new List<string>();
    private List<string> playSlot6 = new List<string>();

    private List<string> scoreSlot0 = new List<string>();
    private List<string> scoreSlot1 = new List<string>();
    private List<string> scoreSlot2 = new List<string>();
    private List<string> scoreSlot3 = new List<string>();

    private List<string> discard = new List<string>();
    private List<string> deck = new List<string>();

    public enum SolitareCompare
    {
        PlayViable,
        ScoreViable,
        Null
    }


    private void Awake()
    {
        NewInstance(this);
    }

    void Start()
    {
        cardSlots = new List<string>[]{ playSlot0, playSlot1, playSlot2, playSlot3, playSlot4, playSlot5, playSlot6,
            scoreSlot0, scoreSlot1, scoreSlot2, scoreSlot3, discard, deck};

        foreach(GameObject playSlot in playPos)
        {
            slotPos.Add(playSlot);
        }

        foreach(GameObject scoreSlot in scorePos)
        {
            slotPos.Add(scoreSlot);
        }

        slotPos.Add(discardPos);

        slotPos.Add(deckPos);
       
        StartCoroutine(Setup());
    }
    IEnumerator Setup()
    {
       // Debug.Log("Dealing cards...");
        yield return new WaitForSeconds(.1f); // Wait for 2 seconds
        SetupCards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Deal());
            //CardsUpdate();
        }
    }

    public void SetupCards()
    {
        cardNames = Solitare.GenerateCardNames();
        Shuffle(cardNames);
        
        GenerateDeck();
        StartCoroutine(Deal());
    }
     public static List<string> GenerateCardNames()
    {
        List<string> newCardNames = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newCardNames.Add(v + "_of_"+s);
            }
        }
        return newCardNames;
     }

    public void Shuffle<T>(List<T> list)
    {
        //Debug.Log("Shuffling");
        System.Random random = new System.Random();
        int n = list.Count;
        while(n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    void GenerateDeck()
    {
        float yOff = 0;
        float zOff = 0f;
        int i = 0;
        foreach (string cardName in cardNames)
        { 
            GameObject NewCard = Instantiate(cardPrefab, new Vector3(deckPos.transform.position.x, deckPos.transform.position.y - yOff, deckPos.transform.position.z - 0.01f - zOff), Quaternion.identity, deckPos.transform);
            NewCard.name = cardName;
            var sel = NewCard.GetComponent<Selectable>();
            sel.FaceUp = false;

            cards.Add(cardName, NewCard);

            i++;
            yOff += 0;
            //zOff += 0.05f;
        }
    }
   
    public IEnumerator Deal()
    {
        int cardsToDeal = 1;
        
        for (int i = 0; i < playPos.Count()-1; i++)
        {
            for(int j = 0;  j < cardsToDeal; j++)
            {
                string s = cardNames.Last<string>();
                if (cards.ContainsKey(s))
                {
                    GameObject cardObject = cards[s];
                    float yOff = 0;
                    float zOff = 0;
                    foreach (string card in cardSlots[i])
                    {
                        yOff += .6f;
                        zOff += cardPadding;
                    }

                    Vector3 tar = new Vector3(playPos[i].transform.position.x, playPos[i].transform.position.y - yOff, playPos[i].transform.position.z - cardPadding - zOff);

                    cardObject.transform.SetParent(playPos[i].transform);
                    Draggable drag = cardObject.GetComponent<Draggable>();
                    drag.enumToPosition(tar);

                    if(j == cardsToDeal - 1)
                    {
                        cardObject.GetComponent<Selectable>().FaceUp = true;
                    }
                    
                }
                cardSlots[i].Add(s);
                cardNames.RemoveAt(cardNames.Count - 1);

                yield return new WaitForSeconds(.1f);
            }
            cardsToDeal++;
        }


        foreach(string cardName in cardNames)
        {
            deck.Add(cardName);
        }

        cardPrefab.gameObject.SetActive(false);
        CardsUpdate(true);
    }

    
    //Checks if the Card is in a Stack and returns that stack as the new target
    public string CardIsInStack(string card)
    {
        int bots = -1;
        var tempStack = "CardSlots";
        if (cardNames.Contains(card))
        {
            tempStack = "CardSlots";
        }
        else
        {
            for (int i = 0; i < slotPos.Count(); i++)
            {

                if (cardSlots[i].Contains(card))
                {
                    tempStack = "Top" + (i.ToString());
                    bots = i;
                    return tempStack;
                }
            }
        }
        return tempStack;
    }

    //public void CardToStack(string card, int stack)
    //{
    //    for (int i = 0; i < (slotPos.Count()); i++)
    //    {
    //        if (CardSlots[i].Contains(card))
    //        {
    //            //Debug.Log($"Card {card} found in slot{i}." + $" placing in slot{stack} ");

    //            if (CardSlots[stack].Count() > 0) { CardsCompare(card, CardSlots[stack].Last()); }
    //            else {  }


    //            CardSlots[i].Remove(card);
    //            CardSlots[stack].Add(card);
    //            cards[card].gameObject.transform.SetParent(slotPos[stack].transform);

    //            CardsUpdate();
    //            return; // Exit the loop if the card is found
    //        }
    //    }
    //}

    public void ResetDeck()
    {
        foreach(string cardName in discard)
        {
            deck.Add(cardName);

            cards[cardName].GetComponent<Selectable>().FaceUp = false;
        }

        discard.Clear();

        CardsUpdate();
    }

    public void CardToStack(string card, string stack)
    {
        int stackk = -1;
        stackk = ParseStackOrder(stack);

        print("FOUND STACKK " + stackk);

        for (int i = 0; i < (slotPos.Count()); i++)
        {
            if (cardSlots[i].Contains(card))
            {

                cardSlots[i].Remove(card);
                cardSlots[stackk].Add(card);
                cards[card].gameObject.transform.SetParent(slotPos[stackk].transform);

                CardsUpdate();
                return; // Exit the loop if the card is found
            }
        }
    }

    public void CardToStack(List<string> cardList, string stack)
    {
        int stackk = -1;
        stackk = ParseStackOrder(stack);

        print("FOUND STACKK " + stackk);

        foreach(string card in cardList)
        {
            for (int i = 0; i < (slotPos.Count()); i++)
            {
                if (cardSlots[i].Contains(card))
                {

                    cardSlots[i].Remove(card);
                    cardSlots[stackk].Add(card);
                    cards[card].gameObject.transform.SetParent(slotPos[stackk].transform);

                    
                    break; // Exit the loop if the card is found
                }
            }
        }

        CardsUpdate();


    }

    public void CardsUpdate(bool dealing = false)
    {
        //foreach (string card in cardNames)
        //{
        //    //cards[card].GetComponent<Selectable>().FaceUp = false;   
        //}
        for (int i = 0; i< slotPos.Count(); i++)
        {
            float zOff = cardPadding;
            float yOff = 0f;

            float zOffDeck = cardPadding;
            foreach (string card in cardSlots[i])
            {
                if (cards.ContainsKey(card) && cardSlots[i] != deck && cardSlots[i] != discard &&
                    cardSlots[i] != scoreSlot0 && cardSlots[i] != scoreSlot1 && cardSlots[i] != scoreSlot2 && cardSlots[i] != scoreSlot3)
                {
                    GameObject cardObject = cards[card];
                    // Modify the position of the cardObject here
                    Vector3 tar = new Vector3(slotPos[i].transform.position.x, slotPos[i].transform.position.y - yOff, slotPos[i].transform.position.z - zOff);
                    if(cardObject.GetComponent<Draggable>()!= null)
                    {
                        cardObject.GetComponent<Draggable>().enumToPosition(tar, 0.04f);  
                    }
                    else 
                    { 
                        cardObject.transform.position = tar; 
                    }

                    if (!dealing && card == LastCardInStack(i))
                    {
                        cardObject.GetComponent<Selectable>().FaceUp = true;
                    }

                    yOff += .6f;
                    zOff += cardPadding;
                }
                else if (cards.ContainsKey(card))
                {
                    GameObject cardObject = cards[card];
                    // Modify the position of the cardObject here
                    Vector3 tar = new Vector3(slotPos[i].transform.position.x, slotPos[i].transform.position.y, slotPos[i].transform.position.z - zOffDeck);
                    if (cardObject.GetComponent<Draggable>() != null)
                    {
                        cardObject.GetComponent<Draggable>().enumToPosition(tar, 0.04f);
                    }
                    else
                    {
                        cardObject.transform.position = tar;
                    }

                    zOffDeck += cardPadding;
                }
                else
                {
                    Debug.LogError("Card '" + card + "' not found in the dictionary.");
                }
            }

            //if(dealing && cardSlots[i] != deck && LastCardInStack(i) != null)
            //{
            //    cards[LastCardInStack(i)].GetComponent<Selectable>().FaceUp = true;
            //}
        }
    }

    public List<Transform> GetStackedCardTransforms(string cardName)
    {
        List<Transform> stackedCardTransforms = new List<Transform>();

        for(int i = 0; i < playPos.Count; i++)
        {
            if (cardSlots[i].Contains(cardName))
            {
                int initialCardIndex = cardSlots[i].IndexOf(cardName);
                for (int j = initialCardIndex + 1; j < cardSlots[i].Count; j++)
                {
                    stackedCardTransforms.Add(cards[cardSlots[i][j]].transform);
                }
            }
        }

        return stackedCardTransforms;
    }

    public bool CompareToPlay(string heldCard, string stack)
    {
        string stackCard = LastCardInStack(ParseStackOrder(stack));

        SolitareCompare sc = CardsCompare(heldCard, stackCard);

        if(sc == SolitareCompare.PlayViable)
        {
            return true;
        }

        return false;
    }

    public bool CompareToScore(string heldCard, string stack)
    {
        string stackCard = LastCardInStack(ParseStackOrder(stack));

        SolitareCompare sc = CardsCompare(heldCard, stackCard);

        if (sc == SolitareCompare.ScoreViable)
        {
            return true;
        }

        return false;
    }

    public string LastCardInStack(int stack)
    {
        string card = null;
        if (cardSlots[stack].Any())
        {
         card = cardSlots[stack].Last<string>();
        }
        
        return card;
    }

    public SolitareCompare CardsCompare(string heldCard, string stackCard)
    {
        if(heldCard == null) 
        { 
            return SolitareCompare.Null; 
        }

        print("COMPARING " + heldCard + " AND " + stackCard);

        // Extract value and suit from card1
        string[] heldCardParts = heldCard.Split('_');
        
        string heldValue = heldCardParts[0];
        string heldSuit = heldCardParts[2];

        // Compare values
        int heldIndex = ParseInteger(heldValue);

        if (heldIndex == 13 && stackCard == null)
        {
            print("King on Play");
            return SolitareCompare.PlayViable;
        }
        else if (heldIndex == 1 && stackCard == null)
        {
            print("Ace on Score");
            return SolitareCompare.ScoreViable;
        }
        else if(stackCard == null)
        {
            return SolitareCompare.Null;
        }

        // Extract value and suit from card2
        string[] stackCardParts = stackCard.Split('_');

        string stackValue = stackCardParts[0];
        string stackSuit = stackCardParts[2];

        // Compare suits
        bool sameSuit = heldSuit == stackSuit;

        //Compare color
        bool sameColor = sameSuit ||
            (heldSuit == "Clubs" && stackSuit == "Spades") || (stackSuit == "Clubs" && heldSuit == "Spades") ||
            (heldSuit == "Diamonds" && stackSuit == "Hearts") || (stackSuit == "Diamonds" && heldSuit == "Hearts");

        //Compare Values
        int stackIndex = ParseInteger(stackValue);

        bool higherValue = heldIndex > stackIndex;

        int difference =  Mathf.Abs(heldIndex - stackIndex);

        //Debug.Log($"{card1} and {card2} " + $"is Higher({higherValue}) and Suited({sameSuit})");

        print("COMPARE RESULTS: HIGHER" + higherValue + " SAMESUIT" + sameSuit + " SAMECOLOR" + sameColor + " DIFFERENCE" + difference);

        if(!higherValue && !sameColor && difference == 1) 
        { 
            return SolitareCompare.PlayViable; 
        }
        else if(higherValue && sameSuit && difference == 1)
        {
            return SolitareCompare.ScoreViable;
        }

        return SolitareCompare.Null;
    }

    public int ParseStackOrder(string stack)
    {
        string[] stackOrder = { "PlaySlot0", "PlaySlot1", "PlaySlot2", "PlaySlot3", "PlaySlot4", "PlaySlot5", "PlaySlot6",
            "ScoreSlot0", "ScoreSlot1", "ScoreSlot2", "ScoreSlot3", "Discard", "DeckButton"};
        for (int i = 0; i < stackOrder.Length; i++)
        {
            if (stack == stackOrder[i])
            {
                //Debug.Log($"ParseStackOrder is {stack} and returning {i}");
                return i;
            }
        }
        //Debug.Log($"ParseStackOrder is {stack} and returning {-1}");
        return -1;
    }

    public int ParseInteger(string valueString)
    {
        int value = -1;
        switch (valueString)
        {
            case "2":
                value = 2;
                break;
            case "3":
                value = 3;
                break;
            case "4":
                value = 4;
                break;
            case "5":
                value = 5;
                break;
            case "6":
                value = 6;
                break;
            case "7":
                value = 7;
                break;
            case "8":
                value = 8;
                break;
            case "9":
                value = 9;
                break;
            case "10":
                value = 10;
                break;
            case "J":
                value = 11;
                break;
            case "Q":
                value = 12;
                break;
            case "K":
                value = 13;
                break;
            case "A":
                value = 1; // In this game we are assuming Ace is always the highest value
                break;
            default:
                value = 0;
                break;
        }  
        return value;
    }
}
//*/