using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "shipParam", menuName = "MiniGame/ShipParam")]
public class MiniGameShipParams : ScriptableObject
{
    public ShipAttribs[] shipAttribs;

    public ShipDataParam GetAttributeWithName(string name)
    {
        for (int i = 0; i < shipAttribs.Length; i++)
        {
            if (shipAttribs[i].name == name) return shipAttribs[i].param;
        }
        return null;
    }
}

[System.Serializable]
public class ShipAttribs
{
    public string name;
    public Sprite attribImage;
    public ShipDataParam param;
    public TradeItem tradeRateData;
}


[System.Serializable]
public class ShipDataParam
{
    public float value;
    public int maxSegment;
    public int fractionAmount;

    public float GetValue()
    {
        return value * (float)fractionAmount / (float)maxSegment;
    }

    public void Delta(int delta)
    {
        fractionAmount += delta;
        if (fractionAmount < 0) fractionAmount = 0;
        else if (fractionAmount > maxSegment) fractionAmount = maxSegment;
    }
}

[System.Serializable]
public struct TradeItem
{
    public int itemID;
    public int itemMultiplier;
    public int itemAdditive;
}