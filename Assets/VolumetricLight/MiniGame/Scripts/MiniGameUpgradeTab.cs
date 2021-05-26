using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameUpgradeTab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI paramNameText;
    [SerializeField] Image paramNameImage;
    [SerializeField] TextMeshProUGUI tradeRateText;
    [SerializeField] Image tradeItemImage;
    [SerializeField] MiniGameProgressBar progressBar;
    ShipAttribs shipAtrribute;

    public void Init(ShipAttribs shipAtrribute)
    {
        this.shipAtrribute = shipAtrribute;
        UpdateUI(shipAtrribute);
    }

    public void OnPressUpgrade()
    {
        MiniGameItem gameItem = MiniGameGlobalRef.playerStat.GetItemByID(shipAtrribute.tradeRateData.itemID);

        if (gameItem != null && gameItem.quantity >= GetRequiredAmount())
        {
            shipAtrribute.param.fractionAmount += 1;
            shipAtrribute.param.fractionAmount = shipAtrribute.param.fractionAmount > shipAtrribute.param.maxSegment ? shipAtrribute.param.maxSegment : shipAtrribute.param.fractionAmount;
            MiniGameGlobalRef.playerStat.DeltaItemQuantity(shipAtrribute.tradeRateData.itemID, -GetRequiredAmount());
            UpdateUI(shipAtrribute);
        }
    }

    int GetRequiredAmount() => (shipAtrribute.param.fractionAmount + 1) * shipAtrribute.tradeRateData.itemMultiplier + shipAtrribute.tradeRateData.itemAdditive;

    void UpdateUI(ShipAttribs shipAtrribute)
    {
        paramNameText.text = shipAtrribute.name;
        paramNameImage.sprite = shipAtrribute.attribImage;
        tradeRateText.text = GetRequiredAmount().ToString();
        tradeItemImage.sprite = MiniGameGlobalRef.itemLib.GetSpriteFromId(shipAtrribute.tradeRateData.itemID);
        progressBar.Setup(shipAtrribute.param.maxSegment, shipAtrribute.param.fractionAmount);
    }

}
