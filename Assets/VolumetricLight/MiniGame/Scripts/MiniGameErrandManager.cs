using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MiniGameErrandManager : MonoBehaviour
{

    public static MiniGameErrandManager instance;

    [SerializeField] Transform portHolder;
    [SerializeField] MiniGameItemLib itemLib;
    [SerializeField] MiniGamePortProperty[] portLocations;
    [SerializeField] float timePerUnitDistance;


    [SerializeField] MiniGameErrand currentErrand;
    [SerializeField] TransportState currentState;
    [SerializeField] GameObject handPointer;
    [SerializeField] MiniGameGuideLine locationGuide;
    [SerializeField] MiniGameAcceptPrompt buttonPrompt;
    [SerializeField] MiniGameErrandDisplay miniGameErrandDisplay;

    MiniGameErrandGoods goodsStash;

    float totalTime, remainingTime;

    bool setupDone, wayFound, goodesStolen;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (portLocations.Length == 0) portLocations = portHolder.GetComponentsInChildren<MiniGamePortProperty>();
        buttonPrompt.gameObject.SetActive(false);
    }

    void StateAction(Vector3 destinationPoint,
                     string uiButtonText,
                     TransportState nextState,
                     Action initializationAction,
                     Action compleatedAction,
                     Action timeFailAction)
    {
        if (!setupDone)
        {
            if (!goodesStolen)
            {
                locationGuide.Setup(MiniGameGlobalRef.playerShip.transform.position, destinationPoint);
                buttonPrompt.SetUp(destinationPoint, uiButtonText);
            }

            miniGameErrandDisplay.SetState(uiButtonText);
            setupDone = true;
            wayFound = false;
            if (initializationAction != null) initializationAction.Invoke();
            // setingupTime
            totalTime = ((MiniGameGlobalRef.playerShip.transform.position - destinationPoint).magnitude * timePerUnitDistance) + 5;
            remainingTime = totalTime;
        }

        else if (!wayFound && buttonPrompt.reachedLocation)
        {
            locationGuide.Stop();
            wayFound = true;
        }

        else if (buttonPrompt.done)
        {
            if (compleatedAction != null) compleatedAction.Invoke();
            setupDone = false;
            currentState = nextState;
        }


        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            miniGameErrandDisplay.UpdateTime(remainingTime / totalTime);
        }
        else
        {
            if (timeFailAction != null)
            {
                timeFailAction.Invoke();
            }
        }

    }

    void Update()
    {
        // if errand is selected
        if (currentErrand != null)
        {
            switch (currentState)
            {
                case TransportState.pickUp:
                    StateAction(currentErrand.collectFromPoint.transform.position, "Pickup", TransportState.transport, null, GiveErrandGoodsToPlayer, ErradFail);
                    break;

                case TransportState.transport:
                    StateAction(currentErrand.dropAtPoint.transform.position, "Drop", TransportState.payment, null, DropErrandItem, ReturnGoods);
                    break;

                case TransportState.returnGoods:
                    StateAction(currentErrand.collectFromPoint.transform.position, "Return", TransportState.none, null, DropErrandItem, ViolateErrandDrop);
                    break;

                case TransportState.payment:
                    StateAction(currentErrand.paymentPoint.transform.position, "Payment", TransportState.compleated, null, PayPlayer, ErradFail);
                    break;

                case TransportState.compleated:
                    currentErrand = null;
                    locationGuide.Stop();
                    break;

                default:
                    break;
            }

        }
        // if errand is not selected
        else
        {

        }
    }

    public void GiveErrandGoodsToPlayer()
    {
        goodsStash = new MiniGameErrandGoods();
        goodsStash.allGoods.Add(new ItemsAndAmount()
        { goods = currentErrand.transferItemID, amount = currentErrand.transferItemCount });
        goodsStash.rewardItem = new ItemsAndAmount()
        { goods = currentErrand.rewardItemID, amount = currentErrand.rewardItemCount };

        MiniGameGlobalRef.playerStat.AddGoodsStash(goodsStash);
    }

    void DropErrandItem()
    {
        MiniGameGlobalRef.playerStat.DropGoodsStash(goodsStash);
    }

    public void ViolateErrandDrop()
    {
        currentErrand = null;
        miniGameErrandDisplay.gameObject.SetActive(false);
        locationGuide.Stop();
        buttonPrompt.gameObject.SetActive(false);
        MiniGameShop.showEnterButton = true;
        if (!goodsStash.goodsUI.Equals(null))
        {
            MiniGameGlobalRef.playerStat.TakeGoodsStash(goodsStash);
            MiniGameGlobalRef.playerStat.DropGoodsStash(goodsStash);
        }

        int stolenValue = MiniGameGlobalRef.itemLib.GetAvrageValueFromId(goodsStash.allGoods[0].goods) * goodsStash.allGoods[0].amount;

        MiniGameNotorietyManager.instance.Stolen(stolenValue);
    }

    public void TakeTheGood()
    {
        if (goodsStash.goodsUI) MiniGameGlobalRef.playerStat.TakeGoodsStash(goodsStash);
        goodesStolen = true;
        locationGuide.Stop();
        buttonPrompt.gameObject.SetActive(false);
        MiniGameShop.showEnterButton = true;
    }

    void PayPlayer()
    {
        MiniGameGlobalRef.playerStat.DeltaItemQuantity(goodsStash.rewardItem.goods, goodsStash.rewardItem.amount);
        miniGameErrandDisplay.gameObject.SetActive(false);
        MiniGameShop.showEnterButton = true;
    }

    public void SetErrand(MiniGameErrand pickedErrand)
    {

        if (currentErrand != null)
        {
            ViolateErrandDrop();
        }

        currentErrand = pickedErrand;
        buttonPrompt.Visible(false);

        currentState = TransportState.pickUp;
        miniGameErrandDisplay.gameObject.SetActive(true);
        locationGuide.gameObject.SetActive(true);
        setupDone = false;
        goodesStolen = false;
        MiniGameShop.showEnterButton = false;
        MiniGameShopUI.instance.ButtonGoTo("exit");
    }

    public void ErradFail()
    {
        currentErrand = null;
        miniGameErrandDisplay.gameObject.SetActive(false);
        locationGuide.Stop();
        buttonPrompt.gameObject.SetActive(false);
        MiniGameShop.showEnterButton = true;
    }

    void ReturnGoods()
    {
        currentState = TransportState.returnGoods;
        setupDone = false;
    }

    public MiniGameErrand[] GenerateErrands()
    {
        Queue<MiniGameErrand> generatedErrand = new Queue<MiniGameErrand>();
        int randomErrandCount = UnityEngine.Random.Range(2, 5);
        for (int i = 0; i < randomErrandCount; i++)
        {
            generatedErrand.Enqueue(GetMeErrands());
        }
        return generatedErrand.ToArray();
    }

    MiniGameErrand GetMeErrands()
    {
        MiniGameErrand newErrand = new MiniGameErrand();

        int totalItemCount = itemLib.GetItemCount();
        int transnsferItemID = UnityEngine.Random.Range(0, totalItemCount);
        Vector2Int exportRange = itemLib.GetExportRange(transnsferItemID);
        int transferAmount = UnityEngine.Random.Range(50, 100);
        int rewardItemID = itemLib.paidItemsIDs[UnityEngine.Random.Range(0, itemLib.paidItemsIDs.Length)];
        int rewardItemAmount = UnityEngine.Random.Range(1, 5); // is subjected to change

        newErrand.transferItemID = transnsferItemID;
        newErrand.transferItemCount = transferAmount;
        newErrand.rewardItemID = rewardItemID;
        newErrand.rewardItemCount = rewardItemAmount;

        List<MiniGamePortProperty> usedLocation = new List<MiniGamePortProperty>();

        for (int i = 0; i < 3; i++)
        {
            GetALocation(usedLocation);
        }

        newErrand.SetLocations(usedLocation);

        return newErrand;
    }

    bool CheckIfSpotPicked(MiniGamePortProperty newSpot, List<MiniGamePortProperty> usedLocation)
    {
        for (int i = 0; i < usedLocation.Count; i++)
        {
            if (usedLocation[i] == newSpot) return true;
        }
        return false;
    }

    MiniGamePortProperty GetALocation(List<MiniGamePortProperty> usedLocation)
    {

        if (portLocations.Length == 0) portLocations = portHolder.GetComponentsInChildren<MiniGamePortProperty>();

        MiniGamePortProperty pickedLocation = null;
        do
        {
            pickedLocation = portLocations[UnityEngine.Random.Range(0, portLocations.Length)];
        }
        while (CheckIfSpotPicked(pickedLocation, usedLocation));

        usedLocation.Add(pickedLocation);

        return pickedLocation;
    }

    public bool ErrandState()
    {
        return currentErrand != null && !goodesStolen ? true : false;
    }

}

public class MiniGameErrand
{
    public bool display = true;
    public MiniGamePortProperty collectFromPoint;
    public MiniGamePortProperty dropAtPoint;
    public MiniGamePortProperty paymentPoint;

    public float remainingTime;
    public int transferItemID;
    public int transferItemCount;
    public int rewardItemID;
    public int rewardItemCount;


    public void SetLocations(List<MiniGamePortProperty> usedLocation)
    {
        collectFromPoint = usedLocation[0];
        dropAtPoint = usedLocation[1];
        paymentPoint = usedLocation[2];
    }
}

enum TransportState
{
    none,
    pickUp,
    transport,
    payment,
    compleated,
    returnGoods,
}


