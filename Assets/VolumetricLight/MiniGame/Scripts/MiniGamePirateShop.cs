using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePirateShop : MonoBehaviour
{
    [SerializeField] float startDistance;
    [SerializeField] bool isSetup, promptButtonShown;
    [SerializeField] MiniGamePirateGoodsLib pirateGoodsLib;
    MiniGameShopGoods[] shopeRates;
    MiniGameErrand[] shopErrands;
    MiniGamePortProperty shopProperty;

    static public MiniGameAcceptPrompt entryPrompt;
    static public bool showEnterButton = true;

    void Start()
    {
        if (!entryPrompt) entryPrompt = MIniGameShopLocationManager.instance.prompter;

        shopProperty = GetComponent<MiniGamePortProperty>();
        CreateTradeRates();
        //   CreateErrands();
    }


    void Update()
    {
        if (showEnterButton)
        {
            if (!promptButtonShown && (MiniGameGlobalRef.playerShip.position - transform.position).sqrMagnitude < Mathf.Pow(startDistance, 2))
            {
                entryPrompt.SetUp(transform.position, "Enter Shop" + "\n" + shopProperty.portName);
                entryPrompt.gameObject.transform.position = transform.position;
                Time.timeScale = 1f;
                promptButtonShown = true;
            }
            else if (promptButtonShown)
            {
                if ((MiniGameGlobalRef.playerShip.position - transform.position).sqrMagnitude > Mathf.Pow(startDistance, 2))
                {
                    StopPromptDisplay();
                }
                else if (!isSetup && entryPrompt.done)
                {
                    entryPrompt.Visible(false);
                    SetupShop();
                    MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.shop);
                    isSetup = true;
                }
            }
        }
        else if (promptButtonShown)
        {
            StopPromptDisplay();
            MiniGameGlobalRef.ResetGameState();
            Time.timeScale = 0.25f;
        }
    }

    void StopPromptDisplay()
    {
        entryPrompt.Visible(false);
        promptButtonShown = false;
        isSetup = false;
    }

    #region Trade

    void CreateTradeRates()
    {
        shopeRates = pirateGoodsLib.GenerateGoods();
    }

    void UploadRates()
    {
        MiniGameShopUI.instance.SetupTradeSection(shopeRates);
    }


    #endregion

    #region Errand

    void CreateErrands()
    {
        shopErrands = MiniGameErrandManager.instance.GenerateErrands();
    }
    void UploadErrands()
    {
        //  MiniGameShopUI.instance.SetupErrands(shopErrands);
    }

    #endregion

    #region General

    void SetupShop()
    {
        showEnterButton = false;
        MiniGameShopUI.instance.SetupShop<MiniGameUIPirateShopNavigation>();

        UploadRates();
        UploadErrands();

    }

    #endregion

    public class MiniGameShopGoods
    {
        public int inItemID;
        public string inItemName;
        public int inItemRate;
        public Sprite inItemSprite;

        public int outItemID;
        public string outItemName;
        public Sprite outItemSprite;
        public GameObject outItemPrefab;

    }

}
