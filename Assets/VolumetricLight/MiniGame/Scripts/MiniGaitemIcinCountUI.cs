using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGaitemIcinCountUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;

    public void SetIconData(MiniGameDepleatableItem.MiniGameStatusMaintainResource resourceData){
        image.sprite = MiniGameGlobalRef.itemLib.GetSpriteFromId(resourceData.itemID);
        text.text = resourceData.itemAmount.ToString();
    }

}
