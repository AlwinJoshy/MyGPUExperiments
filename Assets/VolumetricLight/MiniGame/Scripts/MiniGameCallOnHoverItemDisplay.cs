using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCallOnHoverItemDisplay : MonoBehaviour
{
    [SerializeField] Transform displayPoint;
    [SerializeField] MiniGameStatusBar miniGameStatus;

    public void DisplayItem()
    {
        MiniGameStatusRequirementDisplay.instance.DisplayItemNeeds(displayPoint, miniGameStatus);
    }

    public void HideDisplay()
    {
        MiniGameStatusRequirementDisplay.instance.HideItemNeeds();
    }

    public void Replenish()
    {
        MiniGameGlobalRef.playerStat.ReplenishStatus(miniGameStatus);
    }

}
