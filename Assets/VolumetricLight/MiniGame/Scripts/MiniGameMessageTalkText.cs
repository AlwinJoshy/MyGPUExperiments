using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameMessageTalkText : MonoBehaviour
{
    [SerializeField] float expandSpeed, textFadeSpeed, endSpacing = 20, heightOffset, lifeTime = 5;
    [SerializeField] TextMeshProUGUI textMeshProGUI;
    [SerializeField] Image backgroundImage;
    Vector3 tempVec3 = Vector3.zero;
    Vector2 tempVec2 = Vector2.zero;
    Color textColor;
    public State textState;

    public float maxWidth, maxHeight, generalOpacity, expandValue, recLifeTime;

    public string outDataDisplay;


    public void SetText(string text){
        textMeshProGUI.text = text;
        textMeshProGUI.transform.position = textMeshProGUI.transform.position + Vector3.up * heightOffset;
        textColor = textMeshProGUI.color;
        textColor.a = 0;
        textMeshProGUI.color = textColor;
        recLifeTime = lifeTime;
        Invoke("UpdateSize", 0.1f);
       
    }

    public void Remove(){
       textState = State.die;
    }

    void UpdateSize() {
        UnityEngine.Bounds textSize = textMeshProGUI.textBounds;
        maxWidth = textSize.size.x + endSpacing;
        maxHeight = textSize.size.y;
        textState = State.expand;
    }

    void Update() {

        if(recLifeTime > 0){
            recLifeTime -= Time.deltaTime;
        }
        else if(textState == State.idle){
            recLifeTime = 0;
            textState = State.die;
        }

        switch (textState)
        {
            case State.expand :
            if(expandValue < 1){
                expandValue += Time.deltaTime * expandSpeed;
                tempVec2.x = maxWidth;
                tempVec2.y = maxHeight * expandValue;
                backgroundImage.rectTransform.sizeDelta = tempVec2;
            }
            else if(generalOpacity < 1){
                generalOpacity += Time.deltaTime * textFadeSpeed;
                if(generalOpacity >=1){
                    generalOpacity = 1;
                    textState = State.idle;
                }
                textColor.a = generalOpacity;
                textMeshProGUI.color = textColor;
            }
            break;
            
             case State.die :

            if(generalOpacity > 0){
                generalOpacity -= Time.deltaTime * textFadeSpeed;
                textColor.a = generalOpacity;
                textMeshProGUI.color = textColor;
            }
            else{
                if(expandValue > 0){
                    expandValue -= Time.deltaTime * expandSpeed;
                    if(expandValue <= 0){
                        textState = State.idle;
                        expandValue = 0;
                        MiniGameConversationManager.instance.allConvTexts.Enqueue(this);
                        gameObject.SetActive(false);
                    }
                    tempVec2.x = maxWidth;
                    tempVec2.y = maxHeight * expandValue;
                    backgroundImage.rectTransform.sizeDelta = tempVec2;
                }
            }
            break;

            default:
            break;
        }
    }


    public enum State
    {
        idle,
        expand,
        die
    }

}
