using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGamePirateTradeRateBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rateTelText;
    [SerializeField] Image sellObjImage;
    [SerializeField] Image buyObjImage;
    MiniGamePirateShop.MiniGameShopGoods recTradeRateData;
    MiniGameItem sellItem;


    public void SetupTradeTab(MiniGamePirateShop.MiniGameShopGoods goods)
    {
        recTradeRateData = goods;
        rateTelText.text = "COST : " + goods.inItemRate;
        sellObjImage.sprite = goods.inItemSprite;
        buyObjImage.sprite = goods.outItemSprite;
    }

    public void buyItem()
    {
        MiniGameItem playerStatusItem = MiniGameGlobalRef.playerStat.GetItemObject(recTradeRateData.inItemID);
        if (playerStatusItem != null && playerStatusItem.quantity >= recTradeRateData.inItemRate)
        {
            MiniGameGlobalRef.playerStat.AddWeapon(recTradeRateData);
        }
    }

    MiniGameItem GetStatItem(int id)
    {

        for (int i = 0; i < MiniGameGlobalRef.playerStat.playerStatItem.Count; i++)
        {
            if (MiniGameGlobalRef.playerStat.playerStatItem[i].itemID == id) return MiniGameGlobalRef.playerStat.playerStatItem[i];
        }
        return null;
    }

}
