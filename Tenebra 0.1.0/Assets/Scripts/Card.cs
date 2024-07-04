using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardSO cardSO;

    public bool isPlayer;

    public int currentHealth, attackPower, essenceCost;

    public TMP_Text healthText, attackText, costText, nameText, descriptionText;

    public Image characterArt, bgArt;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public float selectedRotateSpeed = 720f; // Se�ildi�inde kullan�lacak olan rotation h�z�
    public float scaleSpeed = 5f; // �l�ek de�i�tirme h�z�

    public bool inHand;
    public int handPosition;

    private HandController theHC;

    private bool isSelected;
    private bool returningToHand; // Ele d�nerken farkl� h�z kullan�m� i�in
    private Collider2D theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;

    public Animator anim;

    private Vector3 originalScale;
    private Vector3 targetScale;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f); // Kart�n �zerine gelindi�inde b�y�me oran�
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f); // Kart se�ildi�inde b�y�me oran�

    void Start()
    {
        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }

        SetupCard();

        theHC = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider2D>();

        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        essenceCost = cardSO.essenceCost;

        UpdateCardDisplay();

        nameText.text = cardSO.cardName;
        descriptionText.text = cardSO.cardDescription;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
    }

    void Update()
    {
        if (isSelected && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;
            MoveToPoint(worldPosition + new Vector3(0f, 2f, -4f), Quaternion.identity);

            if (Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
            {
                ReturnToHand();
            }

            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.battleEnded == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, whatIsPlacement);

                if (hit.collider != null && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        if (BattleController.instance.playerEssence >= essenceCost)
                        {
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            MoveToPoint(selectedPoint.transform.position + new Vector3(0f, 0.75f, 0f), Quaternion.identity);

                            inHand = false;

                            isSelected = false;
                            returningToHand = false;

                            targetScale = originalScale;

                            theHC.RemoveCardFromHand(this);

                            BattleController.instance.SpendPlayerEssence(essenceCost);

                            ActivateAbility();
                        }
                        else
                        {
                            ReturnToHand();

                            UIController.instance.ShowEssenceWarning();
                        }
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        float currentRotateSpeed = isSelected || returningToHand ? selectedRotateSpeed : rotateSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, currentRotateSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);

        justPressed = false;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    private void OnMouseOver()
    {
        if (inHand && !isSelected && isPlayer && BattleController.instance.battleEnded == false)
        {
            targetScale = hoverScale;
            Vector3 hoverPosition = theHC.cardPositions[handPosition] + new Vector3(0f, 1f, -2f);
            MoveToPoint(hoverPosition, targetRot);
        }
    }

    private void OnMouseExit()
    {
        if (inHand && !isSelected && isPlayer && BattleController.instance.battleEnded == false)
        {
            targetScale = originalScale;
            MoveToPoint(theHC.cardPositions[handPosition], targetRot);
        }
    }

    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            isSelected = true;
            theCol.enabled = false;
            targetRot = Quaternion.identity;
            targetScale = selectedScale;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        returningToHand = true;
        theCol.enabled = true;
        targetRot = theHC.cardRotations[handPosition];
        MoveToPoint(theHC.cardPositions[handPosition], targetRot);
        targetScale = originalScale;
    }

    public void DamageCard(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            assignedPlace.activeCard = null;

            StartCoroutine(WaitJumpAfterDeadCo());
        }

        anim.SetTrigger("Hurt");

        UpdateCardDisplay();
    }

    IEnumerator WaitJumpAfterDeadCo()
    {
        yield return new WaitForSeconds(.2f);

        anim.SetTrigger("Jump");

        yield return new WaitForSeconds(.5f);

        MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);

        Destroy(gameObject, 5f);
    }

    public void ActivateAbility()
    {
        if (cardSO.ability != null)
        {
            cardSO.ability.ActivateAbility(this);
        }
    }

    public void UpdateCardDisplay()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = essenceCost.ToString();
    }
}
