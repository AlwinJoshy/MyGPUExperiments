using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGamePlayerStat : MonoBehaviour
{
    [SerializeField] int initialHearts = 3;
    public UnityEvent onAddHeart;
    public UnityEvent onRemoveHeart;
    public int notoriety = 0;
    public List<MiniGameErrandGoods> allGoodStash = new List<MiniGameErrandGoods>();
    public List<MiniGameItem> playerStatItem = new List<MiniGameItem>();
    public List<MiniGameDepleatableItem> playerStatus = new List<MiniGameDepleatableItem>();
    public List<ActiveItemRef> activeItem = new List<ActiveItemRef>();
    public float statusDepleationRate = 1;
    public Transform purchaseHolder;

    public MiniGameShipParams shipParams;

    GameObject tempGameObject;
    float nextDepleationTime;

    private void Awake()
    {
        MiniGameGlobalRef.playerStat = this;
    }

    void Start()
    {
        for (int i = 0; i < initialHearts; i++) AddHeart();
        AddItem(new MiniGameItem(0, MiniGameGlobalRef.itemLib.GetNameFromId(0), 20));
        AddItem(new MiniGameItem(1, MiniGameGlobalRef.itemLib.GetNameFromId(0), 300));
        AddItem(new MiniGameItem(2, MiniGameGlobalRef.itemLib.GetNameFromId(0), 100));
        DeltaItemQuantity(3, 10);
        AddStatus(new MiniGameDepleatableItem(0, MiniGameGlobalRef.statusLib.GetNameFromId(0), 3, MiniGameGlobalRef.statusLib.GetDepleationFromId(0), MiniGameGlobalRef.statusLib.GetRequirements(0)));
        AddStatus(new MiniGameDepleatableItem(1, MiniGameGlobalRef.statusLib.GetNameFromId(1), 5, MiniGameGlobalRef.statusLib.GetDepleationFromId(1), MiniGameGlobalRef.statusLib.GetRequirements(1)));
    }

    void Update()
    {
        DepleatStatus();
    }

    ///<summery>
    /// Add heart to the player status
    ///<summery>
    public void AddHeart()
    {
        MiniGameGlobalRef.heartCount++;
        onAddHeart.Invoke();
    }

    public void RemoveHeart()
    {
        MiniGameGlobalRef.heartCount--;
        onRemoveHeart.Invoke();
    }

    public void AddWeapon(MiniGamePirateShop.MiniGameShopGoods recTradeRateData)
    {
        if (!purchaseHolder.Find(recTradeRateData.outItemPrefab.name))
        {
            tempGameObject = Instantiate(recTradeRateData.outItemPrefab, purchaseHolder);
            MiniGameGlobalRef.playerStat.DeltaItemQuantity(recTradeRateData.inItemID, -recTradeRateData.inItemRate);
            activeItem.Add(new ActiveItemRef()
            {
                itemObjectRef = tempGameObject,
                itemUIDisplay = MiniGameGlobalRef.gameUI.AddWeapon(recTradeRateData),
                pirateGoodID = recTradeRateData.outItemID
            });
        }
    }

    public MiniGameItem GetItemByID(int itemID)
    {
        for (int i = 0; i < playerStatItem.Count; i++)
        {
            if (playerStatItem[i].itemID == itemID) return playerStatItem[i];
        }
        return null;
    }

    public void AddItem(MiniGameItem item)
    {
        playerStatItem.Add(item);
        MiniGameGlobalRef.itemChanged = true;
    }


    public void AddStatus(MiniGameDepleatableItem item)
    {
        playerStatus.Add(item);
        MiniGameGlobalRef.statusChanged = true;
    }

    public void ReplenishStatus(MiniGameStatusBar statusBar)
    {
        // check the required quantity to replenish full
        int fullReplenishQuantity = (int)Mathf.Ceil(statusBar.depleatableItemSource.capasity - statusBar.depleatableItemSource.quantity);

        MiniGameItem maintainableItem = GetItemObject(statusBar.depleatableItemSource.maintainables[0].itemID);

        if (maintainableItem != null)
        {
            if (maintainableItem.quantity >= fullReplenishQuantity)
            {
                DeltaItemQuantity(statusBar.depleatableItemSource.maintainables[0].itemID, -fullReplenishQuantity);
                statusBar.depleatableItemSource.deltaQuantity(fullReplenishQuantity);
            }
            else
            {
                DeltaItemQuantity(statusBar.depleatableItemSource.maintainables[0].itemID, -maintainableItem.quantity);
                statusBar.depleatableItemSource.deltaQuantity(maintainableItem.quantity);
            }
        }

    }

    public void AddGoodsStash(MiniGameErrandGoods goodStash)
    {
        allGoodStash.Add(goodStash);
        MiniGameGlobalRef.gameUI.AddGoodsStash(goodStash);
    }

    public void DropGoodsStash(MiniGameErrandGoods goodStash)
    {
        if (goodStash.goodsUI) Destroy(goodStash.goodsUI.gameObject);
        allGoodStash.Remove(goodStash);
    }

    public void TakeGoodsStash(MiniGameErrandGoods goodStash)
    {
        Destroy(goodStash.goodsUI.gameObject);
        DeltaItemQuantity(goodStash.allGoods[0].goods, goodStash.allGoods[0].amount);
    }

    public void DepleatStatus()
    {
        if (Time.time > nextDepleationTime)
        {
            for (int i = 0; i < playerStatus.Count; i++)
            {
                playerStatus[i].deltaQuantity(-playerStatus[i].depleationRate * Time.deltaTime);
                playerStatus[i].updated = true;
                MiniGameGlobalRef.statusChanged = true;
                if (playerStatus[i].quantity <= 0) KillsPlayer();
            }
            nextDepleationTime = Time.time + 1 / statusDepleationRate;
        }
    }

    void KillsPlayer()
    {
        Debug.Log("Kill Player...");
    }

    public void DeltaItemQuantity(string itemName, int changeAmount)
    {
        DeltaItemQuantity(MiniGameGlobalRef.itemLib.GetIdFromName(itemName), changeAmount);
    }

    public void DeltaItemQuantity(int itemID, int changeAmount)
    {
        MiniGameItem foundItem = GetItemObject(itemID);
        if (foundItem == null)
        {
            foundItem = new MiniGameItem(itemID, MiniGameGlobalRef.itemLib.GetNameFromId(itemID), 0);
            AddItem(foundItem);
            playerStatItem.Add(foundItem);
        }
        foundItem.deltaQuantity(changeAmount);
        foundItem.updated = true;
        MiniGameGlobalRef.itemChanged = true;
    }

    public void DeltaStatusQuantity(string name, int changeAmount, float depleation, bool isDepleation, bool reduceByTime, bool haveUI)
    {
        DeltaStatusQuantity(MiniGameGlobalRef.statusLib.GetIdFromName(name), changeAmount, depleation, isDepleation, reduceByTime, haveUI);
    }

    public void DeltaStatusQuantity(int id, int changeAmount, float depleation, bool isDetremental, bool reduceByTime, bool haveUI)
    {
        MiniGameDepleatableItem foundStatus = GetStatusObject(id);
        if (foundStatus == null)
        {
            foundStatus = new MiniGameDepleatableItem(id, MiniGameGlobalRef.itemLib.GetNameFromId(id), changeAmount, changeAmount, depleation, isDetremental, reduceByTime, haveUI, MiniGameGlobalRef.statusLib.GetRequirements(0));
            AddStatus(foundStatus);
            playerStatus.Add(foundStatus);
        }
        foundStatus.deltaQuantity(changeAmount);
        foundStatus.updated = true;
        MiniGameGlobalRef.statusChanged = true;
    }

    public MiniGameItem GetItemObject(int itemID)
    {
        for (int i = 0; i < playerStatItem.Count; i++)
        {
            if (itemID != playerStatItem[i].itemID) continue;
            else return playerStatItem[i];
        }
        return null;
    }

    public MiniGameDepleatableItem GetStatusObject(int id)
    {
        for (int i = 0; i < playerStatus.Count; i++)
        {
            if (id != playerStatus[i].id) continue;
            else return playerStatus[i];
        }
        return null;
    }

}
