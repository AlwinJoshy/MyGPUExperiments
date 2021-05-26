using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "goodsLib", menuName = "MiniGame/GoodsLibrary")]
public class MiniGamePirateGoodsLib : ScriptableObject
{
    public MiniGameItemLib itemLib;
    public TradeableItem[] tradableItems;
    public MiniGameBlackGoods[] blackGoods;

    public int itemListTest;

    public string itemName;

    string[] selectableItemNames;

    public string[] GetItemNames()
    {
        selectableItemNames = new string[itemLib.allMiniGameItems.Length];
        for (int i = 0; i < itemLib.allMiniGameItems.Length; i++)
        {
            selectableItemNames[i] = itemLib.allMiniGameItems[i].itemName;
        }
        return selectableItemNames;
    }

    [System.Serializable]
    public struct NameTransfer
    {
        public string objName;
    }

    [System.Serializable]
    public class TradeableItem
    {
        public int itemID;
        public Vector2Int reciveQuantity;
    }

    public MiniGamePirateShop.MiniGameShopGoods[] GenerateGoods()
    {
        int generateGoodsCount = Random.Range(1, blackGoods.Length);
        MiniGamePirateShop.MiniGameShopGoods[] goodsObject = new MiniGamePirateShop.MiniGameShopGoods[generateGoodsCount];
        List<int> saleItem = new List<int>();
        for (int i = 0; i < generateGoodsCount; i++)
        {
            MiniGamePirateShop.MiniGameShopGoods newGood = new MiniGamePirateShop.MiniGameShopGoods();

            int saleGoodPick = -1;
            while (true)
            {
                saleGoodPick = Random.Range(0, blackGoods.Length);
                if (!saleItem.Contains(saleGoodPick))
                {
                    saleItem.Add(saleGoodPick);
                    break;
                }
            }

            newGood.outItemID = saleGoodPick;
            newGood.outItemName = blackGoods[saleGoodPick].goodName;
            newGood.outItemSprite = blackGoods[saleGoodPick].goodImage;
            newGood.outItemPrefab = blackGoods[saleGoodPick].goodPrefab;

            int valuableITemID = Random.Range(0, tradableItems.Length);
            int tradeGoodPick = tradableItems[valuableITemID].itemID;

            newGood.inItemID = tradeGoodPick;
            newGood.inItemName = MiniGameGlobalRef.itemLib.GetNameFromId(tradeGoodPick);
            newGood.inItemRate = blackGoods[saleGoodPick].goodValue / itemLib.GetAvrageValueFromId(tradableItems[valuableITemID].itemID);
            // newGood.inItemRate = Random.Range(tradableItems[valuableITemID].reciveQuantity.x, tradableItems[valuableITemID].reciveQuantity.y);
            newGood.inItemSprite = MiniGameGlobalRef.itemLib.GetSpriteFromId(tradeGoodPick);

            goodsObject[i] = newGood;
        }
        return goodsObject;
    }

}

[System.Serializable]
public class MiniGameBlackGoods
{
    public string goodName;
    public Sprite goodImage;
    public GameObject goodPrefab;
    public int goodValue;
}