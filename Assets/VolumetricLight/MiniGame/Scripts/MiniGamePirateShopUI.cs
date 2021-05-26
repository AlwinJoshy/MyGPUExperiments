using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGamePirateShopUI : MonoBehaviour
{

    public static MiniGamePirateShopUI instance;

    bool isShiftingScreen;
    [SerializeField] float fadeSpeed;
    [SerializeField] CanvasGroup shopGrupe;
    [SerializeField] GameObject selectionSection, tradeSection, errandSection;
    [SerializeField] Transform tradeTabHolder, errandTabHolder;
    [Header("Spawnable UI Objects")]
    [SerializeField] GameObject tradeTab;
    [SerializeField] GameObject errandTab;

    #region Prvate_Invisble
    GameObject fromScreen, toScreen, currentScreen;

    #region Trade
    Queue<MiniGameTradeRateBar> tradeStripsInUse = new Queue<MiniGameTradeRateBar>();
    Queue<MiniGameTradeRateBar> tradeStripsAvilable = new Queue<MiniGameTradeRateBar>();
    #endregion

    #region Errand
    Queue<MiniGameErrandUI> errandStripsInUse = new Queue<MiniGameErrandUI>();
    Queue<MiniGameErrandUI> errandStripsAvilable = new Queue<MiniGameErrandUI>();
    #endregion

    ShiftPhase shiftPhase = ShiftPhase.phase_From;
    #endregion

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (isShiftingScreen) ShiftSection();
    }

    void ShiftSection()
    {
        switch (shiftPhase)
        {
            case ShiftPhase.phase_From:
                shopGrupe.alpha -= Time.deltaTime * fadeSpeed;
                if (shopGrupe.alpha <= 0)
                {
                    shopGrupe.alpha = 0;
                    fromScreen.SetActive(false);
                    if(toScreen != null)toScreen.SetActive(true);
                    shiftPhase = ShiftPhase.phase_To;
                }
                break;

            case ShiftPhase.phase_To:
                if(toScreen == null){
                    isShiftingScreen = false;
                    return;
                }
                shopGrupe.alpha += Time.deltaTime * fadeSpeed;
                if (shopGrupe.alpha >= 1)
                {
                    shopGrupe.alpha = 1;
                    isShiftingScreen = false;
                }
                break;

            default:
                break;
        }
    }

    public void ButtonGoTo(string areaName)
    {
        shiftPhase = ShiftPhase.phase_From;
        switch (areaName)
        {
            case "back":
                fromScreen = toScreen;
                toScreen = selectionSection;
                isShiftingScreen = true;
                break;

            case "trade":
                fromScreen = toScreen;
                toScreen = tradeSection;
                isShiftingScreen = true;
                break;

            case "errand":
                fromScreen = toScreen;
                toScreen = errandSection;
                isShiftingScreen = true;
                break;

            case "exit":
                fromScreen = toScreen;
                toScreen = null;
                isShiftingScreen = true;
                if(!MiniGameErrandManager.instance.ErrandState()) MiniGameShop.showEnterButton = true;
                break;


            default:
                Debug.Log("Button called " + areaName);
                break;
        }
    }

    public void SetupShop()
    {
        selectionSection.SetActive(true);
        shopGrupe.alpha = 0;
        isShiftingScreen = true;
        shiftPhase = ShiftPhase.phase_To;
        toScreen = selectionSection;
    }

    

    #region  Errand

    public void SetupErrands(MiniGameErrand[] errandData)
    {
        ResetErrandStrips();

        for (int i = 0; i < errandData.Length; i++)
        {
            MiniGameErrandUI newTab = GetErrandTab();
            newTab.SetUpErrandUI(errandData[i]);
        }
    }

    public MiniGameErrandUI GetErrandTab()
    {
        MiniGameErrandUI errandStrip;
        if (tradeStripsAvilable.Count > 0)
        {
            errandStrip = errandStripsAvilable.Dequeue();
        }
        else
        {
            errandStrip = Instantiate(errandTab, errandTabHolder).GetComponent<MiniGameErrandUI>();
        }
        errandStripsInUse.Enqueue(errandStrip);
        return errandStrip;
    }

    void ResetErrandStrips()
    {
        MiniGameErrandUI tradeStrip;
        while (errandStripsInUse.Count > 0)
        {
            tradeStrip = errandStripsInUse.Dequeue();
            tradeStrip.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Trade

    public void SetupTradeSection(MiniGameShop.MiniGameShopRate[] tradeRateData)
    {
        ResetTradeStrips();

        for (int i = 0; i < tradeRateData.Length; i++)
        {
            MiniGameTradeRateBar newTab = GetTradeStrip();
            newTab.SetupTradeTab(tradeRateData[i]);
        }
    }

    MiniGameTradeRateBar GetTradeStrip()
    {
        MiniGameTradeRateBar tradeStrip;
        if (tradeStripsAvilable.Count > 0)
        {
            tradeStrip = tradeStripsAvilable.Dequeue();
        }
        else
        {
            tradeStrip = Instantiate(tradeTab, tradeTabHolder).GetComponent<MiniGameTradeRateBar>();
        }
        tradeStripsInUse.Enqueue(tradeStrip);
        return tradeStrip;
    }

    void ResetTradeStrips()
    {
        MiniGameTradeRateBar tradeStrip;
        while (tradeStripsInUse.Count > 0)
        {
            tradeStrip = tradeStripsInUse.Dequeue();
            tradeStrip.gameObject.SetActive(false);
        }
    }

    #endregion

    public enum ShopAreas
    {
        back,
        trade,
        errand
    }

    public enum ShiftPhase
    {
        phase_From,
        phase_To,
    }

}
