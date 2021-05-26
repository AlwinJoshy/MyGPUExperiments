using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameConversator : MonoBehaviour, SelectPositionForEdit
{
    [SerializeField] bool isSetup, active, promptButtonShow;
    [SerializeField] RegType regType;
    [SerializeField] float detectionDistance;
    [SerializeField] public int conversationID = 0;
    [SerializeField] public MiniGameConversation[] conversations;

    [HideInInspector] public EditorLocationLite selectedConversationLocation;
    const float ticRate = 10;
    float nextTick = 0;

    string[] tempStrings;

    void Update()
    {
        if (active && MiniGameTalkManager.currentConversation == null)
        {
            if ((transform.position - MiniGameGlobalRef.playerShip.position).sqrMagnitude < Mathf.Pow(MiniGameTalkManager.instance.entryPrompt.detectionDistance, 2))
            {
                if (regType == RegType.areaTrigger)
                {
                    EnableConversation();
                }

                else
                {

                    if (isSetup)// if Upgrade Screen is In display
                    {
                        Time.timeScale = 1f;
                        MiniGameGlobalRef.ResetGameState();
                        isSetup = false;
                    }
                    else if (promptButtonShow) // Setup the prompter inplace
                    {
                        MiniGameTalkManager.instance.entryPrompt.SetUp(transform.position, "Talk");
                        promptButtonShow = false;
                    }
                    else if (MiniGameTalkManager.instance.entryPrompt.done) // when button is fully pressed
                    {
                        EnableConversation();
                        MiniGameTalkManager.instance.entryPrompt.Visible(false);
                        MiniGameTalkManager.instance.entryPrompt.enabled = false;
                        MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.talk);
                        promptButtonShow = true;
                        isSetup = true;
                    }
                }
            }
            else
            {
                if (!promptButtonShow)
                {
                    promptButtonShow = true;
                }
            }
        }
    }


    void EnableConversation()
    {
        MiniGameTalkManager.SetConversation(conversations[conversationID]);
        active = false;
    }

    public void SetUpTalker(int conversationSetID)
    {
        active = true;
        conversationID = conversationSetID;
        if (regType == RegType.prompt) MiniGameTalkManager.instance.entryPrompt.enabled = true;
    }

    int GetDataINdex(string dataString) => int.Parse(dataString.Split('[')[1].Replace("]", ""));

    public void FindElementWithID(string propertyPath)
    {
        string[] pathFrag = propertyPath.Split('.');
        switch (pathFrag[pathFrag.Length - 1])
        {
            case "cameraPoint":
                selectedConversationLocation = conversations[GetDataINdex(pathFrag[2])].cameraPoint;
                break;

            case "entityPosition":
                Debug.Log(propertyPath);
                selectedConversationLocation = conversations[GetDataINdex(pathFrag[2])].entityList[GetDataINdex(pathFrag[5])].entityPosition;
                break;

            default:
                break;
        }
    }

    public string[] GetEntityNames(string dataPath)
    {
        string[] pathFrag = dataPath.Split('.');
        int entityListLength = conversations[GetDataINdex(pathFrag[2])].entityList.Length;
        tempStrings = new string[conversations[GetDataINdex(pathFrag[2])].entityList.Length];
        for (int i = 0; i < entityListLength; i++)
        {
            tempStrings[i] = conversations[GetDataINdex(pathFrag[2])].entityList[i].name;
        }
        return tempStrings;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }

    public enum RegType
    {
        prompt,
        areaTrigger
    }

}
