using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIniGameShopLocationManager : MonoBehaviour
{
    public static MIniGameShopLocationManager instance;
    public MiniGameAcceptPrompt prompter;

    void Awake()
    {
        instance = this;
    }

    public Transform GetMeAShop()
    {
        int randomID = Random.Range(0, transform.childCount);
        return transform.GetChild(randomID);
    }

}
