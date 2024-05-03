using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame : MonoBehaviour
{

    /// <summary>
    /// This Script is incomplete. It will be the new base for multiple cardgames to derive from. All the functionality is currenlty in AcesUp.cs
    /// 
    /// </summary>

    [Header("Deck and Prefab")]
    public GameObject DeckPos;
    public GameObject cardPrefab;
    public GameObject DiscardPos;
    [Space]

    [Header("Card Slots")]
    public GameObject CardSlotsParent;
    public List<GameObject> CardSlotPositions; //Prev: bottomPos
    public List<string>[] CardSlots; //Prev: bottoms\\
    [HideInInspector]
    public List<string>slot0, slot1, slot2, slot3, slot4, slot5, slot6, slot7, slot8, slot9, slot10, slot11, slot12 = new List<string>();
    [Space]
    public static string[] suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
    public static string[] values = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
    
    [Header("Generated Cards and Faces")]
    public List<string> deck = new List<string>();
    public Dictionary<string, GameObject> cards = new Dictionary<string, GameObject>();
    public Sprite[] CardFaces;
    void Start()
    {

        foreach(Transform child in CardSlotsParent.transform)
        {
            CardSlotPositions.Add(child.gameObject);
            Debug.Log(child.gameObject.name);
        }

        CardSlots = new List<string>[] { slot0, slot1, slot2, slot3, slot4, slot5, slot6, slot7, slot8, slot9, slot10, slot11, slot12 };
        SlotsToUse();
    }

    protected virtual void SlotsToUse()
    {
        
    }


}
