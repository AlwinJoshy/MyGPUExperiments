using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameErrandDisplay : MonoBehaviour
{

    [SerializeField] RectTransform progressIndicator;
    [SerializeField] TextMeshProUGUI stateText;
    Image progressIndicatorRing;
    bool isInitialized;
    Color indicatorColor = Color.black;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (!isInitialized)
        {
            progressIndicatorRing = progressIndicator.GetComponent<Image>();
            indicatorColor = progressIndicatorRing.color;
            isInitialized = true;
        }
    }

    public void SetState(string errandState)
    {
        if(!isInitialized)Init();
        indicatorColor.a = 1;
        progressIndicatorRing.color = indicatorColor;
        stateText.text = errandState;
    }

    public void UpdateTime(float time){
        indicatorColor.a = time;
        progressIndicatorRing.color = indicatorColor;
    }

}


