using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MiniGameUI : MonoBehaviour
{
    [Header("UI Prebfabs")]
    public GameObject
    boatHearts,
    itemUIObject,
    statusUIObject,
    promptButton,
    goodsStashUI,
    armouryUI;

    [SerializeField] RectTransform heartRack;
    [Header("UI Holder Transforms")]
    public Transform
    itemUIHolder,
    stashHolder,
    sliderUIHolder,
    uiPromptButtonContainer,
    armouryContainer;

    public CanvasGroup mainScreen;

    Transform tempObj;
    MiniGameItem tempItem;
    MiniGameDepleatableItem tempStatus;
    PromptButtonsContainer promptButtonsContainer;
    Camera mainCamera;


    enum UIStage
    {
        startUP,
        gamePlay
    }

    UIStage uIStage = UIStage.startUP;

    void Awake()
    {
        MiniGameGlobalRef.gameUI = this;
        promptButtonsContainer = new PromptButtonsContainer();
        mainCamera = Camera.main;
    }

    void Start()
    {

    }


    public void AddHeart()
    {
        Instantiate(boatHearts, heartRack);
    }

    public void RemoveHeart()
    {
        if (heartRack.childCount > 1)
        {
            tempObj = heartRack.GetChild(heartRack.childCount - 1) as RectTransform;
            Destroy(tempObj.gameObject);
        }
        else
        {
            // player dead
        }
    }

    public void AddGoodsStash(MiniGameErrandGoods goodsStash)
    {
        MiniGameGoods newGoodStash = Instantiate(goodsStashUI, stashHolder).GetComponent<MiniGameGoods>();
        newGoodStash.SetUp(goodsStash);
    }

    public GameObject AddWeapon(MiniGamePirateShop.MiniGameShopGoods pirateGood)
    {
        GameObject tempGameObject = Instantiate(armouryUI, armouryContainer);
        tempGameObject.transform.GetChild(0).GetComponent<Image>().sprite = pirateGood.outItemSprite;
        return tempGameObject;
    }

    void Update()
    {
        CheckForInputs();
        CheckPlayerItemUpdation();
        if (MiniGameGlobalRef.BeInControl()) CheckPlayerStatusUpdation();
    }

    void CheckForInputs()
    {
        switch (uIStage)
        {
            case UIStage.startUP:

                if (Input.anyKey)
                {
                    var outTask = FadeAndCloseCanvas(2, mainScreen, false);
                    MiniGameCameraLoc.instance.SetNewTarget(new MiniGameCameraLoc.CamLocInfo(Vector3.zero, CameraFollow.instance.cameraPoint, CameraFollow.instance.cameraPoint, false));
                    uIStage = UIStage.gamePlay;
                }

                break;

            default:
                break;
        }
    }

    public class MiniGameUIClass
    {

    }

    async Task<MiniGameUIClass> FadeAndCloseCanvas(float time, CanvasGroup canvasGroup, bool fadeIn)
    {
        if (fadeIn) canvasGroup.gameObject.SetActive(true);
        time *= 100;
        float totalTime = time;
        float actionInterval = 100 / 24;
        while (time > 0)
        {
            time -= actionInterval;
            canvasGroup.alpha = fadeIn ? 1 - (time / totalTime) : time / totalTime;
            await Task.Delay((int)(10 * actionInterval));
        }
        if (!fadeIn) canvasGroup.gameObject.SetActive(false);
        return null;
    }

    void LateUpdate()
    {
        PromptButtonsManagement();
    }

    public MiniGAmePromptButton GetPromptButton()
    {
        return promptButtonsContainer.GetAButton();
    }

    void PromptButtonsManagement()
    {
        MiniGAmePromptButton tempButton;
        for (int i = 0; i < promptButtonsContainer.inUse.Count; i++)
        {
            tempButton = promptButtonsContainer.inUse[i];
            tempButton.transform.position =
            mainCamera.WorldToScreenPoint(tempButton.lockOnPoint);
        }
    }

    void CheckPlayerItemUpdation()
    {

        if (MiniGameGlobalRef.itemChanged)
        {
            for (int i = 0; i < MiniGameGlobalRef.playerStat.playerStatItem.Count; i++)
            {
                tempItem = MiniGameGlobalRef.playerStat.playerStatItem[i];
                if (tempItem.updated)
                {
                    // giving ref to ui
                    if (tempItem.uiObject == null)
                    {
                        tempItem.uiObject = GetUIItemObjectRef(tempItem);
                    }
                    if (tempItem.quantity < 1)
                    {
                        MiniGameGlobalRef.playerStat.playerStatItem.Remove(tempItem);
                        Destroy(tempItem.uiObject.gameObject);
                    }
                    tempItem.uiObject.SetText(tempItem.quantity);
                    tempItem.updated = false;
                }
            }

            MiniGameGlobalRef.itemChanged = false;
        }
    }

    void CheckPlayerStatusUpdation()
    {
        if (MiniGameGlobalRef.statusChanged)
        {
            for (int i = 0; i < MiniGameGlobalRef.playerStat.playerStatus.Count; i++)
            {
                tempStatus = MiniGameGlobalRef.playerStat.playerStatus[i];
                if (tempStatus.shouldHaveUI)
                {
                    if (tempStatus.updated)
                    {
                        if (tempStatus.uiObject == null)
                        {
                            tempStatus.uiObject = GetUIStatusObjectRef(tempStatus);
                        }
                        tempStatus.uiObject.SetSlider(tempStatus.quantity);
                        tempStatus.updated = false;
                    }
                }
            }
            MiniGameGlobalRef.statusChanged = false;
        }
    }

    MiniGameItemUI GetUIItemObjectRef(MiniGameItem item)
    {
        MiniGameItemUI itemUIDisplay = Instantiate(itemUIObject, itemUIHolder).GetComponent<MiniGameItemUI>();
        itemUIDisplay.SetSprite(MiniGameGlobalRef.itemLib.GetSpriteFromId(item.itemID));
        return itemUIDisplay;
    }

    MiniGameStatusBar GetUIStatusObjectRef(MiniGameDepleatableItem item)
    {
        MiniGameStatusBar statusUIDisplay = Instantiate(statusUIObject, sliderUIHolder).GetComponent<MiniGameStatusBar>();
        statusUIDisplay.SetSprite(MiniGameGlobalRef.statusLib.GetSpriteFromId(item.id));
        statusUIDisplay.SetSatusBarSourceItem(item);
        return statusUIDisplay;
    }

    #region Premitive_UI_Systems

    // propmt button
    [System.Serializable]
    public class PromptButtonsContainer
    {
        public static int countID;
        MiniGAmePromptButton tempButton;
        public List<MiniGAmePromptButton> inUse = new List<MiniGAmePromptButton>();
        public Queue<MiniGAmePromptButton> inStore = new Queue<MiniGAmePromptButton>();

        public PromptButtonsContainer()
        {
            countID++;
        }

        public MiniGAmePromptButton GetAButton()
        {
            tempButton = null;
            if (inStore.Count > 0)
            {
                Debug.Log("Get Prompt button");
                tempButton = inStore.Dequeue();
            }
            else
            {
                tempButton = Instantiate(MiniGameGlobalRef.gameUI.promptButton, MiniGameGlobalRef.gameUI.uiPromptButtonContainer).GetComponent<MiniGAmePromptButton>();
                tempButton.promptButtonsContainer = this;
                tempButton.countID = countID.ToString();
            }
            inUse.Add(tempButton);
            return tempButton;
        }
    }

    #endregion
}

