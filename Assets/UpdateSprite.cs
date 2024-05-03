using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateSprite : MonoBehaviour
{ 
    public Sprite cardFace;
    public Sprite cardBack;

    private SpriteRenderer render;
    private Selectable selectable;
    private AcesUp acesup;

     void Start()
     {   
        acesup = FindAnyObjectByType<AcesUp>();
        List<string> deck = AcesUp.GenerateDeck();

        int i = 0;
        foreach (string card in deck)
        {
            if (this.name == card)
            {
                cardFace = acesup.CardFaces[i];
            }
            i++;
        }
        render = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }

    private void Update()
    {
        if(selectable.FaceUp == true)
        {
            render.sprite = cardFace;
        }
        else
        {
            render.sprite = cardBack;
        }
    }
}
