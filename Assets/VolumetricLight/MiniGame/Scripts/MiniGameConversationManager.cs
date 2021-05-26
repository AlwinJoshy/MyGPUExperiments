using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameConversationManager : MonoBehaviour
{
    public static MiniGameConversationManager instance;
    [SerializeField] GameObject textObject;
    [SerializeField] int maxMessageLimit;
    public Queue<MiniGameMessageTalkText> allConvTexts = new Queue<MiniGameMessageTalkText>();
    MiniGameMessageTalkText tempTextObj;
    Queue<MiniGameMessageTalkText> currentMessages = new Queue<MiniGameMessageTalkText>();

    void Awake()
    {
        instance = this;
    }


    public void AddConversation(string conversationText)
    {
        if (allConvTexts.Count > 0)
        {
            tempTextObj = allConvTexts.Peek();
            allConvTexts.Dequeue();
            tempTextObj.SetText(conversationText);
            tempTextObj.transform.SetSiblingIndex(transform.childCount - 1);
            tempTextObj.gameObject.SetActive(true);
        }
        else
        {
            tempTextObj = Instantiate(textObject, transform).GetComponent<MiniGameMessageTalkText>();
            tempTextObj.SetText(conversationText);
        }

        currentMessages.Enqueue(tempTextObj);
        if (currentMessages.Count > maxMessageLimit)
        {
            currentMessages.Peek().Remove();
            currentMessages.Dequeue();
        }
    }
}
