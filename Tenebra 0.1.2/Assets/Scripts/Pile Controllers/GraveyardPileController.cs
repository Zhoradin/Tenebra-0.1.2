using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardPileController : MonoBehaviour
{
    public static GraveyardPileController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> graveyardPile = new List<CardSO>();

    public GameObject cardSlotPrefab;
    public Transform cardSlotParent;

    // Start is called before the first frame update
    void Start()
    {
        CreateGraveyardPileCardSlots();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateGraveyardPileCardSlots()
    {
        ClearCardSlots();
        UIController.instance.ShowDiscardPileCount();
        foreach (CardSO card in graveyardPile)
        {
            GameObject newCardSlot = Instantiate(cardSlotPrefab, cardSlotParent);
            CardSlot cardSlot = newCardSlot.GetComponent<CardSlot>();
            cardSlot.SetupCardSlot(card);
        }
    }

    private void ClearCardSlots()
    {
        foreach (Transform child in cardSlotParent)
        {
            Destroy(child.gameObject);
            if(graveyardPile.Count == 0)
            {
                UIController.instance.graveyardPileButton.SetActive(false);
            }
        }
    }

    public void AddToGraveyardPile(CardSO card)
    {
        if (UIController.instance.graveyardPileButton.activeSelf == false)
        {
            UIController.instance.graveyardPileButton.SetActive(true);
        }
        graveyardPile.Add(card);
        CreateGraveyardPileCardSlots();
    }
}
