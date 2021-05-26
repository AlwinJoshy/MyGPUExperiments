using UnityEngine;

public class MiniGameShipUpgradeShop : MonoBehaviour
{
    [SerializeField] bool promptButtonShow;
    bool isSetup = false;

    static public MiniGameAcceptPrompt entryPrompt;
    static public bool showEnterButton = true;
    MiniGamePortProperty shopProperty;
    public MiniGameAcceptPrompt prompter;
    public int[] tradeItemIDList;

    void Start()
    {
        if (!entryPrompt && prompter) entryPrompt = prompter;
        shopProperty = GetComponent<MiniGamePortProperty>();

        GenerateOrderedTradeItems();
        ResetlayerStatus();
        promptButtonShow = true;
        showEnterButton = true;
        isSetup = false;
    }

    void Update()
    {
        if (showEnterButton)
        {
            if ((transform.position - MiniGameGlobalRef.playerShip.position).sqrMagnitude < Mathf.Pow(entryPrompt.detectionDistance, 2))
            {
                if (isSetup)// if Upgrade Screen is In display
                {
                    Time.timeScale = 1f;
                    MiniGameGlobalRef.ResetGameState();
                    isSetup = false;
                }
                else if (promptButtonShow) // Setup the prompter inplace
                {
                    entryPrompt.SetUp(transform.position, "Enter");
                    promptButtonShow = false;
                }
                else if (entryPrompt.done) // when button is fully pressed
                {
                    entryPrompt.Visible(false);
                    SendShipParams();
                    MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.shop);
                    showEnterButton = false;
                    promptButtonShow = true;
                    isSetup = true;
                }
            }
            else
            {
                if (!promptButtonShow)
                {
                    promptButtonShow = true;
                }
            }
        }
    }

    void StopPromptDisplay()
    {
        entryPrompt.Close();
        promptButtonShow = false;
        isSetup = false;
    }

    void GenerateOrderedTradeItems()
    {
        MiniGameShipParams shipParamData = MiniGameGlobalRef.playerStat.shipParams;
        for (int i = 0; i < shipParamData.shipAttribs.Length; i++)
        {
            shipParamData.shipAttribs[i].tradeRateData.itemID = tradeItemIDList[Random.Range(0, tradeItemIDList.Length)];
            shipParamData.shipAttribs[i].tradeRateData.itemMultiplier = Random.Range(2, 4);
            shipParamData.shipAttribs[i].tradeRateData.itemAdditive = Random.Range(0, 2);
        }
    }

    void ResetlayerStatus()
    {
        for (int i = 0; i < MiniGameGlobalRef.playerStat.shipParams.shipAttribs.Length; i++)
        {
            MiniGameGlobalRef.playerStat.shipParams.shipAttribs[i].param.fractionAmount = 1;
        }
    }

    void SendShipParams()
    {
        showEnterButton = false;
        MiniGameShopUI.instance.SetupShop<MiniGameUIShipWorkNavigation>();

        MiniGameShopUI.instance.SetupSectionWithData(MiniGameGlobalRef.playerStat.shipParams.shipAttribs, "Upgrade");
        // send the data 
        // in a for loop
        // spawn the tabs
        // pass the data into the tabs
        // setup the bar and tabs
    }

    /*
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
    */
}


