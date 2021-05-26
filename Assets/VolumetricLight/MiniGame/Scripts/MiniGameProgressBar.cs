using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameProgressBar : MonoBehaviour
{
    public Color activeColor;
    public Color passiveColor;

    public void Setup(int maxCount, int currentCount)
    {

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (i < currentCount) transform.GetChild(0).GetChild(i).GetComponent<Image>().color = activeColor;
            else transform.GetChild(0).GetChild(i).GetComponent<Image>().color = passiveColor;
        }

    }


}
