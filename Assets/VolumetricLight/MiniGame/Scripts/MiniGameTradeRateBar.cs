using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameTradeRateBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rateTelText, inputQuantity, outputQuantity;
    [SerializeField] Image sellObjImage;
    [SerializeField] Image buyObjImage;
    MiniGameShop.MiniGameShopRate recTradeRateData;
    float inRelativeValue, outRelativeValue;
    MiniGameItem sellItem;
    public int inAmount;

    public void SetupTradeTab(MiniGameShop.MiniGameShopRate tradeRateData){
        inAmount = 0;
        recTradeRateData = tradeRateData;
        sellObjImage.sprite = tradeRateData.inItemSprite;
        buyObjImage.sprite = tradeRateData.outItemSprite;
        float conversionRate = (float)tradeRateData.outItemRate/tradeRateData.inItemRate;
        if(conversionRate < 1){
            inRelativeValue = 1;
            outRelativeValue = Mathf.Ceil(1/ conversionRate);
        }
        else{
            inRelativeValue = Mathf.Ceil(conversionRate);
            outRelativeValue = 1;
        }
        rateTelText.text = "Rate:" + "\n" + inRelativeValue + " to " + outRelativeValue;
        Reset();
    }

    public void inputDelta(int amount){
        sellItem = GetStatItem(recTradeRateData.inItemID);
        
        if(sellItem == null){
        }
        else{
            inAmount += amount;
            if(inAmount > sellItem.quantity)inAmount = sellItem.quantity;
            else if(inAmount < 0) inAmount = 0;
            inputQuantity.text = inAmount.ToString();

            int inputValue = inAmount/(int)inRelativeValue;
            inputValue = inputValue * (int)outRelativeValue;
            outputQuantity.text = inputValue.ToString();
        }
    }

    public void buyItem(){
        int outValue = inAmount/(int)inRelativeValue;
        if(outValue > 0){
            MiniGameGlobalRef.playerStat.DeltaItemQuantity(recTradeRateData.inItemID, -outValue * (int)inRelativeValue);
            outValue = outValue * (int)outRelativeValue;
            MiniGameGlobalRef.playerStat.DeltaItemQuantity(recTradeRateData.outItemID, outValue);
            Reset();
       }
    }

    void Reset(){
        inAmount = 0;
        inputQuantity.text = 0.ToString();
        outputQuantity.text = 0.ToString();
    }

    MiniGameItem GetStatItem(int id){

        for (int i = 0; i < MiniGameGlobalRef.playerStat.playerStatItem.Count; i++)
        {
            if(MiniGameGlobalRef.playerStat.playerStatItem[i].itemID == id) return MiniGameGlobalRef.playerStat.playerStatItem[i];
        }
        return null;
    }

}
