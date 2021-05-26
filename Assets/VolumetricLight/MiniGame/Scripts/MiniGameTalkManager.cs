using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class MiniGameTalkManager : MonoBehaviour
{

    public static MiniGameTalkManager instance;
    public static MiniGameConversation currentConversation;
    public Transform mainCamera;
    public MiniGameAcceptPrompt entryPrompt;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

    }

    private void Update()
    {
        currentConversation?.Run();
    }

    public static void SetConversation(MiniGameConversation conversation)
    {
        currentConversation = conversation;
        conversation.SetUp();
    }

}

[System.Serializable]
public class MiniGameConversation
{
    public string name;
    public EditorLocationLite cameraPoint;
    public TalkEntity[] entityList;
    public MiniGameCameraLoc.CamLocInfo camLocInfo;

    public KeyCode keycode;

    public int talkIndex;
    public TalkText[] dialogue;

    public UnityEvent onStartTalk;
    public UnityEvent onEndTalk;

    public void SetUp()
    {
        talkIndex = 0;
        if (camLocInfo == null) camLocInfo = new MiniGameCameraLoc.CamLocInfo(cameraPoint.position, null, null, true);
        DisplayTalk();
        MiniGameCameraLoc.instance.SetNewTarget(camLocInfo);
        MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.talk);
        MiniGameGlobalRef.StopPlayerMovement();
        onStartTalk.Invoke();
    }

    public void Run()
    {
        if (Input.GetKeyDown(keycode))
        {
            talkIndex++;
            if (talkIndex < dialogue.Length) DisplayTalk();
            else Close();
        }
    }

    void DisplayTalk()
    {
        MiniGameConversationManager.instance.AddConversation(entityList[dialogue[talkIndex].entityID].name + " : " + dialogue[talkIndex].talkText);
        camLocInfo.target = entityList[dialogue[talkIndex].entityID].name == "Pessant" ? MiniGameGlobalRef.playerShip.position : entityList[dialogue[talkIndex].entityID].entityPosition.position;
        MiniGameParticleLib.instance.PlayEffect(ParticleType.voiceParticle, entityList[dialogue[talkIndex].entityID].name == "Pessant" ? MiniGameGlobalRef.playerShip.position : entityList[dialogue[talkIndex].entityID].entityPosition.position, Vector3.up, true);
    }

    public void Close()
    {
        MiniGameCameraLoc.instance.RemoveTheLastOne();
        MiniGameTalkManager.currentConversation = null;
        MiniGameGlobalRef.ResetGameState();
        onEndTalk.Invoke();
    }

    [System.Serializable]
    public class TalkEntity
    {
        public string name;
        public EditorLocationLite entityPosition;
    }

    [System.Serializable]
    public class TalkText
    {
        public int entityID;
        public string talkText;
    }
}

[System.Serializable]
public class EditorLocationLite
{
    public Vector3 position;
}

public interface SelectPositionForEdit
{

    void FindElementWithID(string propertyPath);

}
