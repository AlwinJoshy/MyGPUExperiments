using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameAcceptPrompt : MonoBehaviour
{
    public bool done, reachedLocation;
    public bool active, uiPromptShown;
    public float detectionDistance;
    [SerializeField] Transform keyHolderPos;
    [SerializeField] KeyCode key;
    [SerializeField] string descriptionText;
    float nextCheck;
    [SerializeField] MiniGAmePromptButton promptButton;


    void Update()
    {
        if (active && Time.time > nextCheck)
        {
            if (Vector3.SqrMagnitude(MiniGameGlobalRef.playerShip.transform.position - transform.position) < Mathf.Pow(detectionDistance, 2))
            {
                if (!uiPromptShown)
                {
                    reachedLocation = true;
                    promptButton = MiniGameGlobalRef.gameUI.GetPromptButton();
                    promptButton.Setup(keyHolderPos.position, key.ToString(), descriptionText);
                    uiPromptShown = true;
                }

                else
                {
                    if (Input.GetKey(key))
                    {
                        if (promptButton.Fill())
                        {
                            promptButton.LooseFocus();
                            active = false;
                            uiPromptShown = false;
                            done = true;
                        }
                    }
                    else promptButton.UnFill();
                }

            }
            else
            {
                if (uiPromptShown)
                {
                    reachedLocation = false;
                    promptButton.LooseFocus();
                    uiPromptShown = false;
                }
            }

            nextCheck = Time.time + 1 / 3;
        }
    }

    public void SetUp(Vector3 pos, string decripText)
    {
        gameObject.SetActive(true);
        reachedLocation = false;
        this.descriptionText = decripText;
        transform.position = pos;
        active = true;
        done = false;
        Visible(true);
        Debug.Log("Button Setup...");
    }

    public void Visible(bool state)
    {
        if (promptButton) promptButton.gameObject.SetActive(state);
    }

    public void Close()
    {
        if (promptButton) promptButton.LooseFocus();
    }

    void OnEnable()
    {
        Visible(true);
    }

    void OnDisable()
    {
        Visible(false);
    }

}
