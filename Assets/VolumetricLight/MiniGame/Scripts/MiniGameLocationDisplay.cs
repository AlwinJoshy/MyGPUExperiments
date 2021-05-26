using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameLocationDisplay : MonoBehaviour
{

    public bool active, initialized, focused;

    [Header("WorldPoint")]
    [SerializeField] Transform worldPoint;

    [SerializeField] [Range(0, 1)] float amount;
    [Header("Circle")]
    [SerializeField] Image circleImage;
    [SerializeField] float circleSize, circleEnd;
    [Header("Pointer")]
    [SerializeField] Image pointerLine;
    [SerializeField] float pointerMaxLength, pointerEnd;
    [Header("Underline")]
    [SerializeField] Image underLine;
    [SerializeField] float underLineWidth, underlineEnd;
    [Header("Label")]
    [SerializeField] TextMeshProUGUI labelText;

    Camera mainCamera;
    Vector3 tempVec3One, tempVec3Two, tempVec3Three;
    Vector2 tempVec2;
    Color tempColor;
    Vector3 sreenVector;
    float angle;
    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (!initialized)
        {
            mainCamera = Camera.main;
            initialized = true;
        }
    }

    public void SetUp(Transform worldPoint, string labelText, float angle)
    {
        this.labelText.text = labelText;
        this.labelText.ForceMeshUpdate();
        this.worldPoint = worldPoint;
        underLineWidth = this.labelText.bounds.size.x;
        this.angle = angle;
        sreenVector = Quaternion.Euler(0, 0, angle) * Vector3.right;
        amount = 0;
        active = true;
        focused = true;
    }

    public void Run()
    {
        Init();
        tempVec3One = mainCamera.WorldToScreenPoint(worldPoint.position);
        tempVec3One.z = 0;
        tempVec3Two = tempVec3One;
        tempVec3Two += sreenVector * pointerMaxLength;
        tempVec3Three = tempVec3Two - tempVec3One;
        // Circle
        circleImage.color = MiniGameDayTimeController.instance.GetLabelLineColor();
        circleImage.rectTransform.position = tempVec3One;
        circleImage.rectTransform.sizeDelta = Vector2.one * circleSize * MiniGameUtilFunctions.ClampLerp01(amount, 0, circleEnd);
        // Pointer Line
        pointerLine.color = MiniGameDayTimeController.instance.GetLabelLineColor();
        pointerLine.rectTransform.position = tempVec3One;
        tempVec2 = pointerLine.rectTransform.sizeDelta;
        tempVec2.x = pointerMaxLength * MiniGameUtilFunctions.ClampLerp01(amount, circleEnd, pointerEnd);
        pointerLine.rectTransform.right = tempVec3Three;
        pointerLine.rectTransform.sizeDelta = tempVec2;
        //Under Line
        underLine.color = MiniGameDayTimeController.instance.GetLabelLineColor();
        underLine.rectTransform.position = tempVec3Two;
        tempVec2 = underLine.rectTransform.sizeDelta;
        tempVec2.x = underLineWidth * MiniGameUtilFunctions.ClampLerp01(amount, pointerEnd, underlineEnd);
        underLine.rectTransform.sizeDelta = tempVec2;
        // LabelText
        labelText.rectTransform.position = tempVec3Two;
        tempColor = MiniGameDayTimeController.instance.GetTextColor();
        tempColor.a = MiniGameUtilFunctions.ClampLerp01(amount, underlineEnd, 1);
        labelText.color = tempColor;
    }

    public void DeltaValue()
    {
        amount += (focused ? 1 : -1) * Time.deltaTime * MiniGameUILabelManager.instance.locationDisplaySpeed;
        amount = Mathf.Clamp(amount, 0, 1);
        if (amount != 0)
        {
            Run();
        }
        else
        {
            MiniGameUILabelManager.instance.RemoveLabel(this);
        }
    }

}
