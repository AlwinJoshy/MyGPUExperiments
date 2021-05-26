using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGAmePromptButton : MonoBehaviour
{
    public string countID;
    public Image fillImage;
    public Vector3 lockOnPoint;
    public float fillSpeed;
    public MiniGameUI.PromptButtonsContainer promptButtonsContainer;
    public bool active, inFocus;
    float fillAmount;

    [SerializeField] float animationAmount;
    [Header("Circle")]
    [SerializeField] RectTransform circleParent;
    [SerializeField] Image circleImage;
    [SerializeField] float maxSize, circleEnd;
    [Header("Circle Inner")]
    [SerializeField] float circleInnerEnd;
    [Header("Key")]
    [SerializeField] TextMeshProUGUI actionButtonKey;
    [SerializeField] float keyDisplayEnd;
    [Header("Description")]
    [SerializeField] TextMeshProUGUI descriptionText;


    Color tempColor;

    private void Update()
    {
        Run();
    }

    void Run()
    {
        animationAmount += (inFocus ? 1 : -1) * Time.deltaTime * 0.5f;
        animationAmount = Mathf.Clamp(animationAmount, 0, 1);
        // Circle
        circleParent.sizeDelta = Vector2.one * maxSize * MiniGameUtilFunctions.ClampLerp01(animationAmount, 0, circleEnd);
        // Inner-Circle
        tempColor = MiniGameDayTimeController.instance.GetLabelLineColor();
        tempColor.a = Mathf.Min(MiniGameUtilFunctions.ClampLerp01(animationAmount, circleEnd, circleInnerEnd), 0.7f);
        circleImage.color = tempColor;
        // Key Display
        tempColor = MiniGameDayTimeController.instance.GetTextColor();
        tempColor.a = MiniGameUtilFunctions.ClampLerp01(animationAmount, circleInnerEnd, keyDisplayEnd);
        actionButtonKey.color = tempColor;
        // Description Display
        tempColor = MiniGameDayTimeController.instance.GetTextColor();
        tempColor.a = MiniGameUtilFunctions.ClampLerp01(animationAmount, keyDisplayEnd, 1);
        descriptionText.color = tempColor;

        if (active && animationAmount == 0) Close();

    }

    public bool Fill()
    {
        Progress(true);
        if (fillAmount < 1)
        {

        }
        else
        {
            return true;
        }
        return false;
    }

    public void UnFill()
    {
        Progress(false);
        if (active && fillAmount > 0)
        {

        }
    }

    public void Progress(bool isPressed)
    {
        fillAmount += (isPressed ? 1 : -1) * fillSpeed * Time.deltaTime;
        tempColor = MiniGameDayTimeController.instance.GetTextColor();
        tempColor.a = fillAmount;
        fillAmount = Mathf.Clamp(fillAmount, 0, 1);
        fillImage.color = tempColor;
    }

    public void RecordContainer(MiniGameUI.PromptButtonsContainer promptButtonsContainer)
    {
        this.promptButtonsContainer = promptButtonsContainer;
    }

    public void Setup(Vector3 stayAtLocation, string keyText, string descriptionString)
    {
        descriptionText.text = descriptionString;
        actionButtonKey.text = keyText;
        lockOnPoint = stayAtLocation;
        active = true;
        gameObject.SetActive(true);
        fillImage.fillAmount = 0;
        fillAmount = 0;
        animationAmount = 0f;
        inFocus = true;
        tempColor = fillImage.color;
        tempColor.a = 0;
        fillImage.color = tempColor;
        Run();
    }

    public void LooseFocus()
    {
        inFocus = false;
    }

    void Close()
    {
        animationAmount = 0;
        promptButtonsContainer.inStore.Enqueue(this);
        promptButtonsContainer.inUse.Remove(this);
        active = false;
        inFocus = false;
        Run();
        gameObject.SetActive(false);
        // keep disabling at the bottom
        // caused issues earlier due to calling disable before setting
        // while active is true
        // which leading it to additself to waiting list for twice
    }


    void OnDisable()
    {
        if (active) Close();
    }

}
