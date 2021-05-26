
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUIShipWorkNavigation : MiniGameUINavigation
{

    [Header("Spawnable UI Objects")]
    [SerializeField] GameObject factorTab;
    [SerializeField] Transform factorTabHolder;

    #region Trade
    Queue<MiniGameUpgradeTab> factorTabInUse = new Queue<MiniGameUpgradeTab>();
    Queue<MiniGameUpgradeTab> factorTabsAvilable = new Queue<MiniGameUpgradeTab>();
    #endregion


    public override void PassData(object data, string id)
    {
        switch (id)
        {
            case "Upgrade":
                ShipAttribs[] upgradeDetailsList = data as ShipAttribs[];
                SetupUpgradeShop(upgradeDetailsList);
                break;

            default:
                break;
        }
    }

    public override void Close()
    {
        base.Close();
        MiniGameShipUpgradeShop.showEnterButton = true;
    }

    public void SetupUpgradeShop(ShipAttribs[] upgradeDetailsList)
    {
        ResetUpgradeStrips();

        for (int i = 0; i < upgradeDetailsList.Length; i++)
        {
            MiniGameUpgradeTab newTab = GetUpgradeStrip();
            newTab.Init(upgradeDetailsList[i]);
        }

    }

    MiniGameUpgradeTab GetUpgradeStrip()
    {
        MiniGameUpgradeTab upgradeTab;
        if (factorTabsAvilable.Count > 0)
        {
            upgradeTab = factorTabsAvilable.Dequeue();
        }
        else
        {
            upgradeTab = Instantiate(factorTab, factorTabHolder).GetComponent<MiniGameUpgradeTab>();
        }
        factorTabInUse.Enqueue(upgradeTab);
        return upgradeTab;
    }

    void ResetUpgradeStrips()
    {
        MiniGameUpgradeTab upgradeTab;
        while (factorTabInUse.Count > 0)
        {
            upgradeTab = factorTabInUse.Dequeue();
            upgradeTab.gameObject.SetActive(false);
        }
    }

    /*
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
    */

}
