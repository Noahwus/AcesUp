using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class AcesUp : MonoBehaviour
{
    public Sprite[] CardFaces;
    public GameObject cardPrefab;
    public List<GameObject> slotPos; //Prev: bottomPos
    public GameObject DeckPos;
    public GameObject DiscardPos;

    public static string[] suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
    public static string[] values = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"};
    public List<string> deck = new List<string>();
    public Dictionary<string, GameObject> cards = new Dictionary<string, GameObject>();


    public List<string>[] CardSlots;  //Prev: Bottoms

    private List<string> slot0 = new List<string>(); //Prev: bottom0
    private List<string> slot1 = new List<string>();
    private List<string> slot2 = new List<string>();
    private List<string> slot3 = new List<string>();



    
    void Start()
    {
        CardSlots = new List<string>[]{ slot0, slot1, slot2, slot3 };
       
        StartCoroutine(Deal());
    }
    IEnumerator Deal()
    {
       // Debug.Log("Dealing cards...");
        yield return new WaitForSeconds(.1f); // Wait for 2 seconds
        PlayCards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AcesDeal());
            //CardsUpdate();
        }
    }

    public void PlayCards()
    {
        deck = GenerateDeck();
        Shuffle(deck);
        
        AcesGenerateDeck();
        StartCoroutine(AcesDeal());
    }
     public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(v + "_of_"+s);
            }
        }
        return newDeck;
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

    void AcesGenerateDeck()
    {
        float yOff = 0;
        float zOff = 0f;
        int i = 0;
        foreach (string card in deck)
        { 
            GameObject NewCard = Instantiate(cardPrefab, new Vector3(DeckPos.transform.position.x, DeckPos.transform.position.y - yOff, DeckPos.transform.position.z - 0.01f - zOff), Quaternion.identity, DeckPos.transform);
            NewCard.name = card;
            var sel = NewCard.GetComponent<Selectable>();
            sel.FaceUp = false;
            sel.aces = this.GetComponent<AcesUp>();

            cards.Add(card, NewCard);

            i++;
            yOff += 0;
            zOff += 0.05f;
        }
    }
   
    public IEnumerator AcesDeal()
    {   for (int i = 0; i < slotPos.Count(); i++)
        {
           string s = deck.Last<string>();
            if (cards.ContainsKey(s))
            {
                GameObject cardObject = cards[s];
                float yOff = 0;
                float zOff = 0;
                foreach (string card in CardSlots[i])
                {
                    yOff += .6f;
                    zOff += .1f;
                }

                Vector3 tar = new Vector3(slotPos[i].transform.position.x, slotPos[i].transform.position.y - yOff, slotPos[i].transform.position.z - 0.1f - zOff) ;

                cardObject.transform.SetParent(slotPos[i].transform);
                Draggable drag = cardObject.GetComponent<Draggable>();
                drag.enumToPosition(tar);
                drag.aces = this.GetComponent<AcesUp>();
                
                cardObject.GetComponent<Selectable>().FaceUp = true;
            }
           CardSlots[i].Add(s);
           deck.RemoveAt(deck.Count - 1);
           
            yield return new WaitForSeconds(.1f);
        }
        cardPrefab.gameObject.SetActive(false);
        CardsUpdate();
    }

    
    //Checks if the Card is in a Stack and returns that stack as the new target
    public string CardIsInStack(string card)
    {
        int bots = -1;
        var tempStack = "CardSlots";
        if (deck.Contains(card))
        {
            tempStack = "CardSlots";
        }
        else
        {
            for (int i = 0; i < slotPos.Count(); i++)
            {

                if (CardSlots[i].Contains(card))
                {
                    tempStack = "Top" + (i.ToString());
                    bots = i;
                    return tempStack;
                }
            }
        }
        return tempStack;
    }

    public void CardToStack(string card, int stack)
    {
        for (int i = 0; i < (slotPos.Count()); i++)
        {
            if (CardSlots[i].Contains(card))
            {
                //Debug.Log($"Card {card} found in slot{i}." + $" placing in slot{stack} ");

                if (CardSlots[stack].Count() > 0) { CardsCompare(card, CardSlots[stack].Last()); }
                else {  }


                CardSlots[i].Remove(card);
                CardSlots[stack].Add(card);
                cards[card].gameObject.transform.SetParent(slotPos[stack].transform);

                CardsUpdate();
                return; // Exit the loop if the card is found
            }
        }
    }
    public void CardToStack(string card, string stack)
    {
        int stackk = -1;
        stackk = ParseStackOrder(stack);
        for (int i = 0; i < (slotPos.Count()); i++)
        {
            if (CardSlots[i].Contains(card))
            {
                //Debug.Log($"Card {card} found in slot{i}." + $" placing in slot{stack} ");

                if (CardSlots[stackk].Count() > 0) { CardsCompare(card, CardSlots[stackk].Last()); }
                else { }


                CardSlots[i].Remove(card);
                CardSlots[stackk].Add(card);
                cards[card].gameObject.transform.SetParent(slotPos[stackk].transform);

                CardsUpdate();
                return; // Exit the loop if the card is found
            }
        }
    }

    public void CardsUpdate()
    {
        foreach (string card in deck)
        {
            //cards[card].GetComponent<Selectable>().FaceUp = false;   
        }
        for (int i = 0; i< slotPos.Count(); i++)
        {
            float zOff = 0.1f;
            float yOff = 0f;
            foreach (string card in CardSlots[i])
            {
                if (cards.ContainsKey(card))
                {
                    GameObject cardObject = cards[card];
                    // Modify the position of the cardObject here
                    Vector3 tar = new Vector3(slotPos[i].transform.position.x, slotPos[i].transform.position.y - yOff, slotPos[i].transform.position.z - zOff);
                    if(cardObject.GetComponent<Draggable>()!= null)
                    {
                        cardObject.GetComponent<Draggable>().enumToPosition(tar, 0.04f);  
                    }
                    else { cardObject.transform.position = tar; }
                    
                    cardObject.GetComponent<Selectable>().FaceUp = true;


                    yOff += .6f;
                    zOff += .1f;
                }
                else
                {
                    Debug.LogError("Card '" + card + "' not found in the dictionary.");
                }
            }
        }
    }


    public bool AcesCompareToLast(string card1, string stack)
    {
        bool CanDo = false;
        string card2 = LastCardInStack(ParseStackOrder(stack));
        //Debug.Log(card2);
        CanDo = CardsCompare(card1, card2);

        return CanDo;
    }

    public string LastCardInStack(int stack)
    {
        string card = null;
        if (CardSlots[stack].Any())
        {
         card = CardSlots[stack].Last<string>();
        }
        
        return card;
    }

    public bool CardsCompare(string card1, string card2)
    {
        if(card1 == null || card2 == null) { return false; }
        // Extract value and suit from card1
        string[] card1Parts = card1.Split('_');
        
        string value1 = card1Parts[0];
        string suit1 = card1Parts[2];

        // Extract value and suit from card2
        string[] card2Parts = card2.Split('_');

        string value2 = card2Parts[0];
        string suit2 = card2Parts[2];

        // Compare suits
        bool sameSuit = suit1 == suit2;

        // Compare values
        int index1 = ParseInteger(value1);
        int index2 = ParseInteger(value2);

        bool higherValue = index1 > index2;

        //Debug.Log($"{card1} and {card2} " + $"is Higher({higherValue}) and Suited({sameSuit})");

        if(!higherValue && sameSuit) { return true; }

        return false;
    }

    public int ParseStackOrder(string stack)
    {
        string[] stackOrder = { "Top0", "Top1", "Top2", "Top3" };
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
                value = 14; // In this game we are assuming Ace is always the highest value
                break;
            default:
                value = 0;
                break;
        }  
        return value;
    }
}
//*/