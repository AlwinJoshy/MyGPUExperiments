using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameGoods : MonoBehaviour
{
    [SerializeField] GameObject uiObjectPrefab;
    [SerializeField] Transform uiItemContainer;

    public void SetUp(MiniGameErrandGoods goods)
    {
        goods.goodsUI = this;
        for (int i = 0; i < goods.allGoods.Count; i++)
        {
            MiniGameItemUI tepUIItem = Instantiate(uiObjectPrefab, transform).GetComponent<MiniGameItemUI>();
            tepUIItem.SetSprite(MiniGameGlobalRef.itemLib.GetSpriteFromId(goods.allGoods[i].goods));
            tepUIItem.SetText(goods.allGoods[i].amount);
        }
    }

    public void TakeErrandGoods()
    {
        MiniGameErrandManager.instance.TakeTheGood();
    }

}
