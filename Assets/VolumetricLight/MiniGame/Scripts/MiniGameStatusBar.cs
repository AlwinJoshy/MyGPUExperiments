using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameStatusBar : MonoBehaviour
{
    [SerializeField] Image uiImage;
    [SerializeField] Slider sliderMain;
    [SerializeField] Slider sliderQuantity;
    [SerializeField] float quantityChangeSpeed;
    public MiniGameDepleatableItem depleatableItemSource;
    float targetQuantity, currentAmount;
    float nextUpdation, temp;

    public void SetSprite(Sprite sprite)
    {
        uiImage.sprite = sprite;
    }

    public void SetSlider(float quantity)
    {
        targetQuantity = quantity;
        sliderQuantity.value = quantity;
    }

    // set the source of status bar data
    public void SetSatusBarSourceItem(MiniGameDepleatableItem srcItem)
    {
        depleatableItemSource = srcItem;
        sliderQuantity.maxValue = srcItem.capasity;
        sliderMain.maxValue = srcItem.capasity;
    }

    // returns maintainable resources
    public MiniGameDepleatableItem.MiniGameStatusMaintainResource[] GetRequiredItemList()
    {
        return depleatableItemSource.maintainables;
    }

    void Update()
    {
        if (!Mathf.Approximately(targetQuantity, currentAmount) && Time.time > nextUpdation)
        {
            temp = targetQuantity - currentAmount;
            currentAmount += temp * 0.1f;
            sliderMain.value = currentAmount;
            nextUpdation = Time.time + 1 / quantityChangeSpeed;
        }
    }

}
