
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUIShopNavigation : MiniGameUINavigation
{

    [Header("Spawnable UI Objects")]
    [SerializeField] GameObject tradeTab;
    [SerializeField] GameObject errandTab;
    [SerializeField] Transform tradeTabHolder, errandTabHolder;

    #region Trade
    Queue<MiniGameTradeRateBar> tradeStripsInUse = new Queue<MiniGameTradeRateBar>();
    Queue<MiniGameTradeRateBar> tradeStripsAvilable = new Queue<MiniGameTradeRateBar>();
    #endregion

    #region Errand
    Queue<MiniGameErrandUI> errandStripsInUse = new Queue<MiniGameErrandUI>();
    Queue<MiniGameErrandUI> errandStripsAvilable = new Queue<MiniGameErrandUI>();
    #endregion


    public override void PassData(object data, string id)
    {
        switch (id)
        {
            case "Trade":
                MiniGameShop.MiniGameShopRate[] tradeItemArray = data as MiniGameShop.MiniGameShopRate[];
                SetupTradeSection(tradeItemArray);
                break;

            case "Errand":
                MiniGameErrand[] errandListArray = data as MiniGameErrand[];
                SetupErrands(errandListArray);
                break;

            default:
                break;
        }
    }

    public override void Close()
    {
        base.Close();
        if (!MiniGameErrandManager.instance.ErrandState()) MiniGameShop.showEnterButton = true;
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


}
