//*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using Unity.VisualScripting;
using static UnityEngine.RuleTile.TilingRuleOutput;
//using System.Diagnostics;

[RequireComponent(typeof(Collider))]
public class Draggable : MonoBehaviour
{

    /// <summary>
    /// The code here will be refactored as "CardGame.cs" and "GameAcesUp.cs" functionality is distributed properly.
    /// Example: CheckDropViability() that "Determining if a Drop is Viable" is not up to the card itself, but the "GameAcesUp.cs"'s criteria
    /// </summary>


    public CardGame game;
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

    private List<UnityEngine.Transform> stackedCardTrans = new List<UnityEngine.Transform>();
    private List<Vector3> stackedCardPoss = new List<Vector3>();
    private List<string> stackNames = new List<string>();

    private Selectable sel;

    private void Start()
    {
        col = GetComponent<Collider>();
        sel = GetComponent<Selectable>();
    }

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

            stackedCardTrans.Clear();
            stackedCardTrans = Solitare.Instance.GetStackedCardTransforms(name);

            stackedCardPoss.Clear();
            foreach (UnityEngine.Transform t in stackedCardTrans)
            {
                stackedCardPoss.Add(t.position - transform.position);
            }

            stackNames.Clear();
            stackNames.Add(name);
            foreach (UnityEngine.Transform t in stackedCardTrans)
            {
                stackNames.Add(t.name);
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, Solitare.Instance.cardPadding * -53.0f);
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

            UpdateStackedTransforms();
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        CheckDropViability();

        UpdateStackedTransforms();
    }

    public void UpdateStackedTransforms()
    {
        for (int i = 0; i < stackedCardTrans.Count; i++)
        {
            stackedCardTrans[i].position = transform.position + stackedCardPoss[i];
        }
    }

    private void CheckDropViability()
    {
        col.enabled = false;

        string dropTag = GetDropTag(transform.position);

        string stackName = GetStackName(transform.position);

        col.enabled = true;

        print("Drop string:" + dropTag + " StackName:" + stackName);


        if (dropTag == null)
        {
            StartCoroutine(LerpToPosition(originalPosition));
        }
        else if(!sel.FaceUp && dropTag == "Discard")
        {
            sel.FaceUp = true;
            Debug.Log("Gets here");
            Solitare.Instance.CardToStack(this.name, stackName);
            return;
        }
        else if(dropTag == "Play")
        {

            Debug.Log(dropTag + " " + dropTag);

            bool canPlay = Solitare.Instance.CompareToPlay(this.name, stackName);
            if (canPlay)
            {
                Debug.Log("Gets here");
                Solitare.Instance.CardToStack(stackNames, stackName);
                return;
            }
        }
        else if( dropTag == "Score")
        {
            Debug.Log(dropTag + " " + dropTag);

            bool canScore = Solitare.Instance.CompareToScore(this.name, stackName);
            if (canScore)
            {
                Debug.Log("Gets here");
                Solitare.Instance.CardToStack(stackNames, stackName);
                return;
            }
        }

        StartCoroutine(LerpToPosition(originalPosition));

    }

    private string GetStackName(Vector3 droploc)
    {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
        if (hit)
        {
            if (hit.collider.CompareTag("Play") || hit.collider.CompareTag("Score") || hit.collider.CompareTag("Discard"))
            {
                print("HIT " + hit.collider.tag);
                return hit.collider.gameObject.name;
            }
        }
        return null;
    }

    private string GetDropTag(Vector3 dropPosition)
    {
        
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
        if (hit)
        {
            if (hit.collider.CompareTag("Play") || hit.collider.CompareTag("Score") || hit.collider.CompareTag("Discard"))
            {
                print("HIT " + hit.collider.tag);
                return hit.collider.tag;
            }
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

        UpdateStackedTransforms();
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

        UpdateStackedTransforms();
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