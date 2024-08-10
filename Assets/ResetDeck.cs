using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResetDeck : MonoBehaviour
{
    private void OnMouseDown()
    {
        Solitare.Instance.ResetDeck();
    }
}
