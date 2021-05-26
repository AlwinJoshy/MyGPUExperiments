using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameShopUI : MonoBehaviour
{

    public static MiniGameShopUI instance;
    MiniGameUINavigation shopNavigation;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        shopNavigation?.Run();
        //   if (isShiftingScreen) ShiftSection();
    }

    public void SetupShop<T>()
    {
        shopNavigation = GetComponentInChildren<T>() as MiniGameUINavigation;
        shopNavigation.Init();
    }
    /*
        public void SetupShop(System.Type typeT)
        {
            System.Type t = typeT.GetType();
           // shopNavigation = GetComponentInChildren<typeT>();
            shopNavigation.Init();
        }
    */
    public void SetupErrands(object errandData)
    {
        //shopNavigation.SetupErrands(errandData);
        shopNavigation.PassData(errandData, "Errand");
    }

    public void SetupTradeSection(object tradeRateData)
    {
        shopNavigation.PassData(tradeRateData, "Trade");
        //shopNavigation.SetupTradeSection(tradeRateData);
    }

    public void SetupSectionWithData(object tradeRateData, string sectionName)
    {
        shopNavigation.PassData(tradeRateData, sectionName);
        //shopNavigation.SetupTradeSection(tradeRateData);
    }

    public void ButtonGoTo(string areaName)
    {
        shopNavigation.ButtonGoTo(areaName);
    }

}
