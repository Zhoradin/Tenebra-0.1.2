using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaController : MonoBehaviour
{
    public List<TypeInfo> TypeList = new List<TypeInfo>();
    public List<GodInfo> GodsList = new List<GodInfo>();
    public List<CardInfo> CardsList = new List<CardInfo>();
    public List<MoonPhaseInfo> PhasesList = new List<MoonPhaseInfo>();

    public TMP_Text typeTitle, typeInfoDescription, godTitle, godInfoDescription, cardTitle, cardInfoDescription, phaseTitle, phaseInfoDescription;
    public Image typeImage, phaseImage;

    public GameObject typesPanel, cardInfoPanel, godsPanel, moonPhasePanel;

    // TMP Sprite Asset for replacing type names with sprites in text
    public TMP_SpriteAsset typeSpriteAsset;
    public CardSlot cardSlotPrefab;
    public Transform cardSlotContainer;

    private DataCarrier dataCarrier;

    void Start()
    {
        dataCarrier = FindObjectOfType<DataCarrier>();

        CheckCardButtons();
        ActivateTypeInfo(TypeList[0]);
        ActivateGodInfo(GodsList[0]);
        ActivatePhaseInfo(PhasesList[0]);

        foreach (var cardInfo in CardsList)
        {
            cardInfo.button.onClick.AddListener(() => OnCardButtonClicked(cardInfo));
        }

        if (CardsList.Count > 0)
        {
            OnCardButtonClicked(CardsList[0]);
        }
    }


    public void OnTypesClicked()
    {
        typesPanel.SetActive(true);
        cardInfoPanel.SetActive(false);
        godsPanel.SetActive(false);
        moonPhasePanel.SetActive(false);
    }

    public void OnCardInfoClicked()
    {
        typesPanel.SetActive(false);
        cardInfoPanel.SetActive(true);
        godsPanel.SetActive(false);
        moonPhasePanel.SetActive(false);
    }

    public void OnGodsClicked()
    {
        typesPanel.SetActive(false);
        cardInfoPanel.SetActive(false);
        moonPhasePanel.SetActive(false);
        godsPanel.SetActive(true);
    }

    public void OnPhaseClicked()
    {
        typesPanel.SetActive(false);
        cardInfoPanel.SetActive(false);
        godsPanel.SetActive(false);
        moonPhasePanel.SetActive(true);
    }

    public void OnMoonPhaseClicked()
    {
        string formattedPhaseName = FormatMoonPhaseName(MoonPhaseController.instance.currentMoonPhase.ToString());

        MoonPhaseInfo phaseInfo = PhasesList.Find(moonPhase => moonPhase.phaseName == formattedPhaseName);
        if (phaseInfo != null)
        {
            ActivatePhaseInfo(phaseInfo);
        }
        else
        {
            Debug.LogError($"Phase info for '{formattedPhaseName}' not found in PhasesList.");
        }

        // Moon Phase panelini aktif yap
        UIController.instance.OpenEncyclopediaPanel();
        typesPanel.SetActive(false);
        cardInfoPanel.SetActive(false);
        godsPanel.SetActive(false);
        moonPhasePanel.SetActive(true);
    }

    private string FormatMoonPhaseName(string moonPhase)
    {
        return System.Text.RegularExpressions.Regex.Replace(moonPhase, "([a-z])([A-Z])", "$1 $2");
    }

    public void ActivateTypeInfo(TypeInfo typeInfo)
    {
        string description = ReplaceTypeNamesWithSprites(typeInfo.description);

        typeTitle.text = typeInfo.typeName;
        typeImage.sprite = typeInfo.typeSprite;
        typeInfoDescription.text = description;

        typeInfoDescription.spriteAsset = typeSpriteAsset;
    }

    private string ReplaceTypeNamesWithSprites(string text)
    {
        text = text.Replace("Aqua", "<sprite name=\"aqua\"> Aqua");
        text = text.Replace("Holy", "<sprite name=\"holy\"> Holy");
        text = text.Replace("Dust", "<sprite name=\"dust\"> Dust");
        text = text.Replace("Gale", "<sprite name=\"gale\"> Gale");
        text = text.Replace("Gloom", "<sprite name=\"gloom\"> Gloom");
        text = text.Replace("Ember", "<sprite name=\"ember\"> Ember");
        text = text.Replace("Abyss", "<sprite name=\"abyss\"> Abyss");

        return text;
    }

    private void CheckCardButtons()
    {
        foreach (var cardInfo in CardsList)
        {
            if (dataCarrier.deckToUse.Contains(cardInfo.cardSO))
            {
                cardInfo.button.interactable = true;
                cardInfo.buttonImage.color = Color.white; // Butonun rengi normal beyaz olarak ayarlan�r
                cardInfo.buttonText.text = cardInfo.cardName; // Orijinal kart ismi g�sterilir
                cardInfo.buttonText.color = Color.black; // Orijinal text rengi siyah olarak ayarlan�r
            }
            else
            {
                cardInfo.button.interactable = false;
                cardInfo.buttonImage.color = Color.black; // Butonun rengi siyah olarak ayarlan�r
                cardInfo.buttonText.text = "???"; // ??? metni g�sterilir
                cardInfo.buttonText.color = Color.white; // Metin rengi beyaz olarak ayarlan�r
            }
        }
    }

    public void OnCardButtonClicked(CardInfo cardInfo)
    {
        cardTitle.text = cardInfo.cardName;
        cardInfoDescription.text = cardInfo.description;

        // Instantiate a new CardSlot for the selected card
        InstantiateCardSlot(cardInfo.cardSO);
    }

    private void InstantiateCardSlot(CardSO cardSO)
    {
        // Clear previous slots
        foreach (Transform child in cardSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new CardSlot
        CardSlot newCardSlot = Instantiate(cardSlotPrefab, cardSlotContainer);
        newCardSlot.SetupCardSlot(cardSO);
    }

    public void ActivateGodInfo(GodInfo godInfo)
    {
        godTitle.text = godInfo.godName;
        godInfoDescription.text = godInfo.godDescription;
    }

    public void OnAquaClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Aqua"));
    }

    public void OnHolyClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Holy"));
    }

    public void OnDustClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Dust"));
    }

    public void OnGaleClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Gale"));
    }

    public void OnGloomClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Gloom"));
    }

    public void OnEmberClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Ember"));
    }

    public void OnAbyssClicked()
    {
        ActivateTypeInfo(TypeList.Find(type => type.typeName == "Abyss"));
    }

    public void OnCaerulisnClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Caerulisn"));
    }

    public void OnAmarunisClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Amarunis"));
    }

    public void OnPoulviClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Poulvi"));
    }

    public void OnArstelloClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Arstello"));
    }

    public void OnLogiumClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Logium"));
    }

    public void OnRohvClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Rohv"));
    }

    public void OnSoliriaClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Soliria"));
    }

    public void OnTenebraClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Tenebra"));
    }

    public void OnAbororClicked()
    {
        ActivateGodInfo(GodsList.Find(god => god.godName == "Abor'or"));
    }

    public void ActivatePhaseInfo(MoonPhaseInfo phaseInfo)
    {
        phaseTitle.text = phaseInfo.phaseName;
        phaseInfoDescription.text = phaseInfo.phaseDescription;
        phaseImage.sprite = phaseInfo.phaseSprite;
    }

    public void OnNewMoonClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "New Moon"));
    }

    public void OnWaxingCrescentClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "Waxing Crescent"));
    }

    public void OnFirstQuarterClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "First Quarter"));
    }

    public void OnWaxingGibbousClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "Waxing Gibbous"));
    }

    public void OnFullMoonClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "Full Moon"));
    }

    public void OnWaningGibbousClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "Waning Gibbous"));
    }

    public void OnLastQuarterClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "Last Quarter"));
    }

    public void OnWaningCrescentClicked()
    {
        ActivatePhaseInfo(PhasesList.Find(moonPhase => moonPhase.phaseName == "Waning Crescent"));
    }
}

[System.Serializable]
public class TypeInfo
{
    public string typeName;
    public Sprite typeSprite;
    [TextArea(5,10)]
    public string description;

    public TypeInfo(string typeName, Sprite typeSprite, string description)
    {
        this.typeName = typeName;
        this.typeSprite = typeSprite;
        this.description = description;
    }
}

[System.Serializable]
public class GodInfo
{
    public string godName;
    [TextArea(5,10)]
    public string godDescription;

    public GodInfo(string godName, string godDescription)
    {
        this.godName = godName;
        this.godDescription = godDescription;
    }
}

[System.Serializable]
public class CardInfo
{
    public string cardName;
    public CardSO cardSO;
    public Button button;
    public Image buttonImage; // Butonun Image bile�eni
    public TMP_Text buttonText; // Butonun Text bile�eni
    [TextArea(5,10)]
    public string description;

    public CardInfo(string cardName, CardSO cardSO, Button button, Image buttonImage, TMP_Text buttonText, string description)
    {
        this.cardName = cardName;
        this.cardSO = cardSO;
        this.button = button;
        this.buttonImage = buttonImage;
        this.buttonText = buttonText;
        this.description = description;
    }
}

[System.Serializable]
public class MoonPhaseInfo
{
    public string phaseName;
    [TextArea(5, 10)]
    public string phaseDescription;
    public Sprite phaseSprite;

    public MoonPhaseInfo(string phaseName, string phaseDescription, Sprite phaseSprite)
    {
        this.phaseName = phaseName;
        this.phaseDescription = phaseDescription;
        this.phaseSprite = phaseSprite;
    }
}
