//*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using Unity.VisualScripting;
//using System.Diagnostics;

public class Draggable : MonoBehaviour
{

    /// <summary>
    /// The code here will be refactored as "CardGame.cs" and "GameAcesUp.cs" functionality is distributed properly.
    /// Example: CheckDropViability() that "Determining if a Drop is Viable" is not up to the card itself, but the "GameAcesUp.cs"'s criteria
    /// </summary>


    public CardGame game;
    public AcesUp aces;
    private Collider col;

    public bool isDragging = false;
    private Vector3 offset;
    private Vector3 targetLoc;
    private Vector3 originalPosition;
    public float DragSpeed = 0.01f;
    public float LerpSpeed = 0.08f;

    //public LayerMask raycastIgnoreThis;

    public float checkRadius = .1f;
    public float rotationLerpFactor = 0.1f;
    //public float maxRotationAngle = 30f;

    private void Start(){col = this.GetComponent<Collider>();}
    private void Update()
    {
        if (isDragging && col.enabled == false) {   col.enabled = false; }
        else if(col.enabled == false) {             col.enabled = true; }
    }

    private void OnMouseDown()
    {
        if (IsMouseOverObject() != null)
        {
            isDragging = true;
            offset = transform.position - GetMouseWorldPosition();
            originalPosition = transform.position;

        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            targetLoc = Vector3.Lerp(this.transform.position, GetMouseWorldPosition() + offset, DragSpeed);

            //Card rotation is not currently working due to changes in the "Draggable" system. But here is where it would go
            /*if (IsOutsideRadius())
            {
                RotateToTarget();
            }*/

            transform.position = targetLoc;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        CheckDropViability();
    }

    

    private void CheckDropViability()
    {
        string drop = IsDropLocationViable(transform.position);
        string top = CheckViableDropLocation(transform.position);
       
        if (drop == null)
        {
            Debug.Log(drop);
        }
        else
        {

            Debug.Log(drop + " " + drop);

            bool yup = aces.AcesCompareToLast(this.name, drop);
            if (yup)
            {
                Debug.Log("Gets here");
                aces.CardToStack(this.name, drop);
                return;
            }else if (aces.LastCardInStack(aces.ParseStackOrder(drop)) == null)
            {
                aces.CardToStack(this.name, drop);
            }
            else { StartCoroutine(LerpToPosition(originalPosition)); }
        }

    }

    private string CheckViableDropLocation(Vector3 droploc)
    {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
        
        if(hit){ 
            if (hit.collider.CompareTag("Tops"))
            {
                return hit.collider.gameObject.name;
            }
            else if(hit.collider.CompareTag("Card"))
            {
                return null;
            }
        }
        else
        {
            GameObject temp = IsMouseOverObject();
            if(temp != null)
            {
                if (temp.CompareTag("Card"))
                {
                    Debug.Log("This was found: " + temp.gameObject.name);
                    return temp.gameObject.name;
                }
            }
        }
        
        return null;
    }

    private string IsDropLocationViable(Vector3 dropPosition)
    {
        // Viablility check for Drop location.
        // If the Drop is attempted overtop a "Tops" position,
        // or a Card that is currenlty under a Tops poisiton,
        
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
        if (hit)
        {
            
            if (hit.collider.CompareTag("Tops"))
            {
                return hit.rigidbody.name;
            }
            else if (hit.collider.CompareTag("Card"))
            {

            }
        }
        else
        {
            string tempstring = IsMouseOverObject().name;
            if(this.name != tempstring && tempstring != null)
            {
                string strinn = aces.CardIsInStack(tempstring);
                return strinn;
            }
            
            return null;
        }
        return null;
    }

   public void enumToPosition(Vector3 tar)
    {
        StartCoroutine(LerpToPosition(tar));
    }
     
    public void enumToPosition(Vector3 tar, float lerpspd)
    {
        StartCoroutine(LerpToPosition(tar, lerpspd));
    }

    public IEnumerator LerpToPosition(Vector3 tar)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        while (Time.time < startTime + LerpSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, tar, (Time.time - startTime) / LerpSpeed);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (Time.time - startTime) / LerpSpeed);

            yield return null;
        }
        transform.position = tar; // Ensure we reach the exact original position
        transform.rotation = targetRotation;
    }
    public IEnumerator LerpToPosition(Vector3 tar, float LerpSpeedAlt)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        while (Time.time < startTime + LerpSpeedAlt)
        {
            transform.position = Vector3.Lerp(startPosition, tar, (Time.time - startTime) / LerpSpeedAlt);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (Time.time - startTime) / LerpSpeedAlt);

            yield return null;
        }
        transform.position = tar; // Ensure we reach the exact original position
        transform.rotation = targetRotation;
    }

    private GameObject IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }


    private bool IsOutsideRadius()
    {
        float distance = Vector3.Distance(transform.position, targetLoc);
        return distance > checkRadius;
    }

    private void RotateToTarget()
    {
        Vector3 direction = (targetLoc - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float lerpValue = Mathf.Clamp01((Vector3.Distance(transform.position, targetLoc) - checkRadius) * rotationLerpFactor);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpValue);
    }

}