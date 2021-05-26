using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameSpawnItemManager : MonoBehaviour
{
    static public MiniGameSpawnItemManager instance;
    [SerializeField] Transform spawnedItemContainer;
    [SerializeField] Transform mainCamera;
    public MiniGameItemLib itemLib;
    public GameObject spawnPrefab;
    public List<DropItems> activeItems = new List<DropItems>();
    public Queue<DropItems> dormantItems = new Queue<DropItems>();
    public Gradient colorGradient;
    [SerializeField] float pullInDistance, pullInSpeed, fallLevel;
    [SerializeField] Vector2Int spawnHeight;

    Vector3 tempVec3 = Vector3.zero;
    Vector2 tempVec2 = Vector2.zero;
    float tempFloat;
    GameObject tempGameObject;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateDropedItems();
    }

    void UpdateDropedItems()
    {
        for (int i = 0; i < activeItems.Count; i++)
        {

            tempVec3 = (MiniGameGlobalRef.playerShip.position + Vector3.up * 0.1f) - activeItems[i].obj.transform.position;

            tempFloat = tempVec3.sqrMagnitude;
            if (tempFloat < Mathf.Pow(pullInDistance, 2))
            {
                if (tempFloat < Mathf.Pow(0.2f, 2))
                {
                    TakeItem(activeItems[i]);
                    return;
                }
                else
                {
                    activeItems[i].obj.transform.Translate((tempVec3 / Mathf.Max(tempFloat, 0.5f)) * pullInSpeed * Time.deltaTime, Space.World);
                }
            }
            else if (activeItems[i].obj.transform.position.y - fallLevel > 0)
            {
                activeItems[i].obj.transform.Translate(Vector3.down * Time.deltaTime * 3);
            }
            activeItems[i].obj.LookAt(mainCamera.position);
        }
    }

    void TakeItem(DropItems itemDrop)
    {
        itemDrop.obj.gameObject.SetActive(false);
        MiniGameGlobalRef.playerStat.DeltaItemQuantity(itemDrop.itemID, itemDrop.quantity);
        activeItems.Remove(itemDrop);
        dormantItems.Enqueue(itemDrop);
    }

    public DropItems GetItem(int itemID, int quantity, Vector3 pos)
    {
        DropItems returnItem;

        if (dormantItems.Count > 0)
        {
            returnItem = dormantItems.Dequeue();
            returnItem.obj.gameObject.SetActive(true);
        }
        else
        {
            returnItem = new DropItems();
            tempGameObject = Instantiate(spawnPrefab, pos, Quaternion.identity, spawnedItemContainer);
            returnItem.obj = tempGameObject.transform;
            returnItem.spriteImage = tempGameObject.GetComponentInChildren<Image>();
            returnItem.quantText = tempGameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        activeItems.Add(returnItem);
        returnItem.itemID = itemID;
        returnItem.quantity = quantity;

        returnItem.obj.transform.position = pos;
        returnItem.spriteImage.sprite = itemLib.GetSpriteFromId(itemID);
        returnItem.quantText.text = quantity.ToString();

        return returnItem;
    }

    public void DropItems(Vector2Int maxAndMin, Vector3 position, float spread)
    {
        for (int i = maxAndMin.x; i < maxAndMin.y; i++)
        {
            tempVec2 = Random.insideUnitCircle * spread;
            position.x += tempVec2.x;
            position.z += tempVec2.y;
            position.y = Random.Range(1, 100) * 0.01f + (spawnHeight.y - spawnHeight.x) + spawnHeight.x;
            int spawnID = itemLib.GenerateRandomItemID();
            GetItem(spawnID, Random.Range(1, itemLib.GetMaxDropAmount(spawnID)), position);
        }
    }

}

[System.Serializable]
public class DropItems
{
    public bool active;
    public int itemID;
    public int quantity;
    public Transform obj;
    public Image spriteImage;
    public TextMeshProUGUI quantText;

}
