using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class MiniGameDayTimeController : MonoBehaviour
{   
    public static MiniGameDayTimeController instance;


    [SerializeField] bool manual;
    [SerializeField] [Range(0, 1)] float dayTime;
    [SerializeField] float tickRate, maxScale = 1;
    [SerializeField] AnimationCurve timeChangeGraph;
    [SerializeField] Gradient atmosphereFogColor;
    [SerializeField] Gradient shadowColor;
    [SerializeField] Gradient waterHighColor;
    [SerializeField] Gradient waterLowColor;
    [SerializeField] Gradient waterFoamColor;
    [SerializeField] Gradient textColor;
    [SerializeField] Gradient labelLineColor;

    [SerializeField] Material displayMat;
    [SerializeField] Material seaMat;

    [Header("Day")]
    [SerializeField] [Range(0, 1)] float dayPoint;
    [SerializeField] UnityEvent OnDayArrived;

    [Header("Night")]
    [SerializeField] [Range(0, 1)] float nightPoint;
    [SerializeField] UnityEvent OnNightArrived;

    float lastRecTime, nextTick, lastupDateTime;
    bool isNight;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (manual || Time.time > nextTick)
        {
            if (!manual) dayTime = timeChangeGraph.Evaluate(Mathf.PingPong(Time.time, maxScale) / maxScale);
            Shader.SetGlobalFloat("_DayTime", dayTime);
            shadowColor.Equals(dayTime);
            displayMat.SetVector("_ShadowColor", shadowColor.Evaluate(dayTime));
            displayMat.SetVector("_FogColor", atmosphereFogColor.Evaluate(dayTime));
            seaMat.SetVector("_HighColor", waterHighColor.Evaluate(dayTime));
            seaMat.SetVector("_LowColor", waterLowColor.Evaluate(dayTime));
            seaMat.SetVector("_FoamColor", waterFoamColor.Evaluate(dayTime));

            // check if night arrived
            if (!isNight && dayTime > nightPoint && lastupDateTime - dayTime < 0)
            {
                OnNightArrived.Invoke();
                isNight = true;
            }

            else if (isNight && dayTime < dayPoint && lastupDateTime - dayTime > 0)
            {
                OnDayArrived.Invoke();
                isNight = false;
            }

            lastupDateTime = dayTime;

            nextTick = Time.time + 1 / tickRate;
        }
    }

    public Color GetTextColor()
    {
        return textColor.Evaluate(dayTime);
    }

    public Color GetLabelLineColor()
    {
        return labelLineColor.Evaluate(dayTime);
    }

}
