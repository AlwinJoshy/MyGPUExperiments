using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameItemUI : MonoBehaviour
{
    [SerializeField] Image uiImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float quantityChangeSpeed;
    int targetQuantity, currentAmount;
    float nextUpdation;

    public void SetSprite(Sprite sprite){
        uiImage.sprite = sprite;
    }

    public void SetText(int quantity){
        targetQuantity = quantity;
    }

    void Update() {
        if(targetQuantity != currentAmount && Time.time > nextUpdation){
            text.text = (targetQuantity > currentAmount ? ++currentAmount : --currentAmount).ToString();
            nextUpdation = Time.time + 1/quantityChangeSpeed;
        }
    }

}
