using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameItem
{
    public bool updated = false;
    public int itemID;
    public string itemName;
    public int quantity;
    public MiniGameItemUI uiObject;

    public MiniGameItem(int id, string name, int amount)
    {
        itemID = id;
        itemName = name;
        quantity = amount;
        updated = true;
        uiObject = null;
    }

    public void deltaQuantity(int amount)
    {
        quantity += amount;
    }

}

public class MiniGameDepleatableItem
{
    public string name;
    public int id;
    public float depleationRate = 1;
    public float nextDepleationTime;
    public float quantity;
    public float capasity;
    public bool isDetramental;
    public bool reduceByTime;
    public bool shouldHaveUI;
    public bool updated = false;
    public MiniGameStatusMaintainResource[] maintainables;
    public MiniGameStatusBar uiObject;


    // DATA TYPES        
    [System.Serializable]
    public class MiniGameStatusMaintainResource
    {
        public int itemID;
        public int itemAmount;

        public MiniGameStatusMaintainResource(int ID, int count)
        {
            itemID = ID;
            itemAmount = count;
        }
    }

    public MiniGameDepleatableItem(int id, string name, float quantity, float capasity, float depleation, bool isDetramental, bool reduceByTime, bool shouldHaveUI, MiniGameStatusLib.MaintainRequirements[] maintainanceRequirements)
    {
        this.name = name;
        this.id = id;
        this.quantity = quantity;
        this.depleationRate = depleation;
        this.capasity = capasity;
        this.quantity = quantity;
        this.isDetramental = isDetramental;
        this.reduceByTime = reduceByTime;
        this.shouldHaveUI = shouldHaveUI;
        updated = false;
        this.uiObject = null;

        RecordeMaintainables(maintainanceRequirements);

    }

    ///<summary>
    /// Makes a object which will display through the UI element
    ///</summary>

    public MiniGameDepleatableItem(int id, string name, float quantity, float capasity, float depleation, MiniGameStatusLib.MaintainRequirements[] maintainanceRequirements)
    {
        this.name = name;
        this.id = id;
        this.quantity = quantity;
        this.depleationRate = depleation;
        this.capasity = capasity;
        this.quantity = quantity;
        this.isDetramental = true;
        this.reduceByTime = true;
        this.shouldHaveUI = true;
        updated = false;
        this.uiObject = null;

        RecordeMaintainables(maintainanceRequirements);
    }

    ///<summary>
    /// Makes a object which will display through the UI element
    ///</summary>

    public MiniGameDepleatableItem(int id, string name, float quantity, float depleation, MiniGameStatusLib.MaintainRequirements[] maintainanceRequirements)
    {
        this.name = name;
        this.id = id;
        this.quantity = quantity;
        this.depleationRate = depleation;
        this.capasity = quantity;
        this.quantity = quantity;
        this.isDetramental = true;
        this.reduceByTime = true;
        this.shouldHaveUI = true;
        updated = false;
        this.uiObject = null;

        RecordeMaintainables(maintainanceRequirements);
    }



    public void RecordeMaintainables(MiniGameStatusLib.MaintainRequirements[] reqs)
    {
        maintainables = new MiniGameStatusMaintainResource[reqs.Length];
        for (int i = 0; i < reqs.Length; i++)
        {
            maintainables[i] = new MiniGameStatusMaintainResource(MiniGameGlobalRef.itemLib.GetIdFromName(reqs[i].itemName), reqs[i].itemAmount);
        }
    }

    public void deltaQuantity(float amount)
    {
        quantity += amount;
    }

    public void depleatQuantity()
    {
        quantity -= depleationRate;
    }

    public void changeCapaity(float capasity)
    {
        this.capasity = capasity;
    }

    public float GetPerUno()
    {
        return quantity / capasity;
    }

    public float GetPercent()
    {
        return GetPerUno() * 100;
    }

}

public class MiniGameErrandGoods
{
    public List<ItemsAndAmount> allGoods = new List<ItemsAndAmount>();
    public MiniGameGoods goodsUI;
    public ItemsAndAmount rewardItem;
}

public struct ItemsAndAmount
{
    public int goods;
    public int amount;
}

public struct ActiveItemRef
{
    public int pirateGoodID;
    public GameObject itemObjectRef;
    public GameObject itemUIDisplay;
}