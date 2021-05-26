using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameTransitionGame : MonoBehaviour
{
    public static MiniGameTransitionGame instance;
    public string loadBackSceneName;
    public Material transMaterial;
    public float transitionSpeed;
    [SerializeField] GameObject tarnsitionScreenObject;

    TransitionControlStates states = TransitionControlStates.none;
    float tempFloat;

    public enum TransitionControlStates
    {
        none,
        idle,
        climbUP,
        climbDown,
        checkerIN,
        checkerOUT,
        delayedClose
    }

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        transMaterial.SetFloat("_DiamondCover", 1);
        transMaterial.SetFloat("_CheckerShut", 1);
        tarnsitionScreenObject.SetActive(true);
        Invoke("OpenScreen", 1);
    }

    public void OpenScreen()
    {
        states = TransitionControlStates.checkerOUT;
        tempFloat = 1;
        tarnsitionScreenObject.SetActive(true);
    }

    public void GameOver()
    {
        states = TransitionControlStates.delayedClose;
        tempFloat = 0;
        tarnsitionScreenObject.SetActive(true);
    }

    void Update()
    {
        switch (states)
        {

            case TransitionControlStates.idle:
                if (tarnsitionScreenObject.activeSelf) tarnsitionScreenObject.SetActive(false);
                break;

            case TransitionControlStates.climbUP:
                tempFloat += transitionSpeed * Time.deltaTime;
                transMaterial.SetFloat("_DiamondCover", tempFloat);
                if (tempFloat >= 1)
                {
                    tempFloat = 0;
                    states = TransitionControlStates.checkerIN;
                }
                break;

            case TransitionControlStates.checkerIN:
                tempFloat += transitionSpeed * Time.deltaTime;
                transMaterial.SetFloat("_CheckerShut", tempFloat);
                if (tempFloat >= 1)
                {
                    states = TransitionControlStates.idle;
                }
                break;

            case TransitionControlStates.checkerOUT:
                tempFloat -= transitionSpeed * Time.deltaTime;
                transMaterial.SetFloat("_CheckerShut", tempFloat);
                if (tempFloat <= 0)
                {
                    tempFloat = 1;
                    states = TransitionControlStates.climbDown;
                }
                break;

            case TransitionControlStates.climbDown:
                tempFloat -= transitionSpeed * Time.deltaTime;
                transMaterial.SetFloat("_DiamondCover", tempFloat);
                if (tempFloat <= 0)
                {
                    states = TransitionControlStates.idle;
                }
                break;

            case TransitionControlStates.delayedClose:
                tempFloat += transitionSpeed * Time.deltaTime;
                if (tempFloat > 0.5)
                {
                    states = TransitionControlStates.climbUP;
                    tempFloat = 0;
                }
                break;

        }
    }




}
