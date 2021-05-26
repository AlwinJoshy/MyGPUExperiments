using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemLib", menuName = "MiniGame/ItemLibrary")]
public class MiniGameItemLib : ScriptableObject
{
    public MiniGameItem[] allMiniGameItems;
    public int[] paidItemsIDs;

    public MiniGameShop.MiniGameShopRate[] GenerateRates()
    {
        int cycle = 0;
        int itemCount = 3;
        List<MiniGameShop.MiniGameShopRate> generatedRates = new List<MiniGameShop.MiniGameShopRate>();
        List<int> addedItemIndex = new List<int>();

        while (cycle < 100 && addedItemIndex.Count < itemCount)
        {
            cycle++;
            int i = Random.Range(0, allMiniGameItems.Length);
            if (!addedItemIndex.Contains(i) && allMiniGameItems[i].sellable)
            {
                addedItemIndex.Add(i);
                MiniGameShop.MiniGameShopRate newRate = new MiniGameShop.MiniGameShopRate();
                newRate.inItemID = i;
                newRate.inItemName = allMiniGameItems[i].itemName;
                newRate.inItemRate = Random.Range(allMiniGameItems[i].minMaxRate.x, allMiniGameItems[i].minMaxRate.y);
                newRate.inItemSprite = allMiniGameItems[i].itemImage;

                int n = -1;
                bool found = false;

                while (cycle < 100 && !found)
                {
                    n = Random.Range(0, allMiniGameItems.Length);
                    if (i != n) found = true;
                    cycle++;
                }

                newRate.outItemID = n;
                newRate.outItemName = allMiniGameItems[n].itemName;
                newRate.outItemRate = Random.Range(allMiniGameItems[n].minMaxRate.x, allMiniGameItems[n].minMaxRate.y);
                newRate.outItemSprite = allMiniGameItems[n].itemImage;
                generatedRates.Add(newRate);
            }
        }

        return generatedRates.ToArray();

    }

    public int GetIdFromName(string itemName)
    {
        for (int i = 0; i < allMiniGameItems.Length; i++)
        {
            if (allMiniGameItems[i].itemName != itemName) continue;
            else return i;
        }
        return -1;
    }

    public int GetItemCount()
    {
        return allMiniGameItems.Length;
    }

    public string GetNameFromId(int id)
    {
        return allMiniGameItems[id].itemName;
    }

    public int GetAvrageValueFromId(int id)
    {
        return (int)((allMiniGameItems[id].minMaxRate.x + allMiniGameItems[id].minMaxRate.y) * 0.5f);
    }

    public Sprite GetSpriteFromId(int id)
    {
        return allMiniGameItems[id].itemImage;
    }

    public Vector2Int GetExportRange(int id)
    {
        return allMiniGameItems[id].exportRange;
    }

    public int GetMaxDropAmount(int id){
        return allMiniGameItems[id].dropAmount;
    }

    public int GenerateRandomItemID()
    {
        while (true)
        {
            int selectedItemID = Random.Range(0, allMiniGameItems.Length);
            if (Random.Range(0, 100) <= allMiniGameItems[selectedItemID].chance)
            {
                return selectedItemID;
            }
        }
    }

    [System.Serializable]
    public class MiniGameItem
    {
        public string itemName;
        public Sprite itemImage;
        public Vector2Int exportRange;
        public Vector2Int minMaxRate;
        public bool sellable;
        public int chance = 1; // rarety from one to 100
        public int dropAmount;
    }

}
