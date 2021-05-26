using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameErrandUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemCount, pickupPortName, dropPortName;
    [SerializeField] Image pickItemIcon, payItemIcon;
    [SerializeField] Transform tabHolder;
    [SerializeField] CanvasGroup canvasGroup;
    MiniGameErrand errandSrc;
    float tabShiftVal;

    public void SetUpErrandUI(MiniGameErrand errandObj)
    {
        errandSrc = errandObj;
        canvasGroup.alpha = 1;
        itemCount.text = "Collect " + errandObj.transferItemCount.ToString();
        pickupPortName.text = "from " + errandObj.collectFromPoint.portName;
        dropPortName.text = "drop at " + errandObj.dropAtPoint.portName + " for " + errandObj.rewardItemCount.ToString();

        pickItemIcon.sprite = MiniGameGlobalRef.itemLib.GetSpriteFromId(errandObj.transferItemID);
        payItemIcon.sprite = MiniGameGlobalRef.itemLib.GetSpriteFromId(errandObj.rewardItemID);
        tabShiftVal = 0;
    }

    public void OnAcceptErrand()
    {
        MiniGameErrandManager.instance.SetErrand(errandSrc);
        errandSrc.display = false;
        gameObject.SetActive(false); // need to do tween fade for this component
        LeanTween.moveX(tabHolder.gameObject, Screen.width / 2, 0.5f);
        LeanTween.alphaCanvas(canvasGroup, 0, 0.5f);
    }
}
