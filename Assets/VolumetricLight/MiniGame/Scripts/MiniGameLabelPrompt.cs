using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameLabelPrompt : MonoBehaviour
{
    MiniGameLocationDisplay labelObject;
    [SerializeField] float detectionRange, angle;
    [SerializeField] [TextArea] string locationText;
    bool labelDisplayed;
    Vector3 tempVec3;

    private void Update()
    {
        tempVec3 = MiniGameGlobalRef.playerShip.position - transform.position;
        if (tempVec3.sqrMagnitude <= detectionRange * detectionRange)
        {
            if (!labelDisplayed)
            {
                DisplayLabel();
                labelDisplayed = true;
            }
        }
        else
        {
            if (labelDisplayed)
            {
                labelDisplayed = false;
                labelObject.focused = false;
            }
        }
    }

    void DisplayLabel()
    {
        if (!labelObject)
        {
            CreateNewLabel();
        }
        else
        {
            if (!labelObject.focused && labelObject.gameObject.activeSelf)
            {
                labelObject.gameObject.SetActive(true);
                labelObject.focused = true;
            }
            else
            {
                CreateNewLabel();
            }
        }
    }

    void CreateNewLabel()
    {
        labelObject = MiniGameUILabelManager.instance.GetLabel();
        labelObject.gameObject.SetActive(true);
        labelObject.SetUp(transform, locationText, angle);
    }

}
