using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameShop : MonoBehaviour
{
    [SerializeField] float startDistance;
    [SerializeField] bool isSetup, promptButtonShown;
    [SerializeField] MiniGameItemLib itemLib;
    MiniGameShopRate[] shopeRates;
    MiniGameErrand[] shopErrands;
    MiniGamePortProperty shopProperty;

    static public MiniGameAcceptPrompt entryPrompt;
    static public bool showEnterButton = true;

    void Start()
    {
        if (!entryPrompt) entryPrompt = MIniGameShopLocationManager.instance.prompter;
        shopProperty = GetComponent<MiniGamePortProperty>();
        CreateTradeRates();
        CreateErrands();
    }


    void Update()
    {
        if (showEnterButton)
        {
            if (!promptButtonShown && (MiniGameGlobalRef.playerShip.position - transform.position).sqrMagnitude < Mathf.Pow(startDistance, 2))
            {
                entryPrompt.SetUp(transform.position, "Enter Shop" + "\n" + shopProperty.portName);
                entryPrompt.gameObject.transform.position = transform.position;
                promptButtonShown = true;
                Time.timeScale = 1f;
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
            //     MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.shop);
        }
    }

    void StopPromptDisplay()
    {
        entryPrompt.Close();
        promptButtonShown = false;
        isSetup = false;
    }

    #region Trade

    void CreateTradeRates()
    {
        shopeRates = itemLib.GenerateRates();
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
        MiniGameShopUI.instance.SetupErrands(shopErrands);
    }

    #endregion

    #region General

    void SetupShop()
    {
        showEnterButton = false;
        MiniGameShopUI.instance.SetupShop<MiniGameUIShopNavigation>();

        UploadRates();
        UploadErrands();

    }

    #endregion

    public class MiniGameShopRate
    {
        public int inItemID;
        public string inItemName;
        public int inItemRate;
        public Sprite inItemSprite;

        public int outItemID;
        public string outItemName;
        public int outItemRate;
        public Sprite outItemSprite;

    }

}
