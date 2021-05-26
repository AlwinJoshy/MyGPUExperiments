using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameStatusRequirementDisplay : MonoBehaviour
{
    public static MiniGameStatusRequirementDisplay instance;
    [SerializeField] GameObject uiIconObject;
    public Transform baseParent, iconSpawnParent, hideParent;
    public CanvasGroup canvasGroup;
    public float fadeSpeed;
    Queue<MiniGaitemIcinCountUI> iconAvilable = new Queue<MiniGaitemIcinCountUI>();
    Queue<MiniGaitemIcinCountUI> iconInUse = new Queue<MiniGaitemIcinCountUI>();
    MiniGaitemIcinCountUI tempUIIcon;

    void Start()
    {
        instance = this;
        canvasGroup.alpha = 0;
    }

    public void DisplayItemNeeds(Transform childLocation, MiniGameStatusBar statusBar)
    {
        ResetUI();

        DisplayNeededItems(statusBar);

        transform.SetParent(childLocation);
        transform.localPosition = Vector3.zero;
        LeanTween.alphaCanvas(canvasGroup, 1, fadeSpeed);
    }

    public void HideItemNeeds()
    {
        transform.SetParent(baseParent);
        LeanTween.alphaCanvas(canvasGroup, 0, fadeSpeed);
    }

    public void ResetUI(){
        while (iconInUse.Count > 0)
        {
            tempUIIcon = iconInUse.Dequeue();
            tempUIIcon.gameObject.SetActive(false);
            iconAvilable.Enqueue(tempUIIcon);
            tempUIIcon.transform.SetParent(hideParent);
        }
    }

    // display the required items in a row
    public void DisplayNeededItems(MiniGameStatusBar statusBar){
        MiniGameDepleatableItem.MiniGameStatusMaintainResource[] temResourceList = statusBar.GetRequiredItemList();
        for (int i = 0; i < temResourceList.Length; i++)
        {
            tempUIIcon = GetOrGenerate();
            tempUIIcon.SetIconData(temResourceList[i]);
        }
    }

    // generate or get an already existing UI element
    MiniGaitemIcinCountUI GetOrGenerate(){
        if(iconAvilable.Count > 0){
            tempUIIcon = iconAvilable.Peek();
            iconAvilable.Dequeue();
            tempUIIcon.gameObject.SetActive(true);
        }
        else{
            tempUIIcon = Instantiate(uiIconObject, iconSpawnParent).GetComponent<MiniGaitemIcinCountUI>();
        }
        tempUIIcon.transform.SetParent(iconSpawnParent);
        iconInUse.Enqueue(tempUIIcon);
        return tempUIIcon; 
    }

}
