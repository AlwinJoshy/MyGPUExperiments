using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MiniGameDayCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dayDisplayText;
    [SerializeField] TextMeshProUGUI dayStringText;
    [SerializeField] UnityEvent OnDaysOver;
    bool active;
    int daysLeft;
    public void StartDayCount(int daysLeft)
    {
        active = true;
        gameObject.SetActive(true);
        this.daysLeft = daysLeft;
        DisplayDate();
    }

    public void DayOver()
    {
        if (active)
        {
            daysLeft--;
            if (daysLeft < 1) OnCountOver();
            else DisplayDate();
        }
    }

    public void OnCountOver()
    {
        OnDaysOver.Invoke();
        gameObject.SetActive(false);
        active = false;
    }

    public void DisplayDate()
    {
        dayDisplayText.text = daysLeft.ToString();
        if (daysLeft > 1) dayStringText.text = "Days";
        else dayStringText.text = "Day";
    }

}
