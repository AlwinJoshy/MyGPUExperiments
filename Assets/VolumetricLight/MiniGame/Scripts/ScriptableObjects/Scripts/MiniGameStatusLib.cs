using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusLib", menuName = "MiniGame/statusLibrary")]
public class MiniGameStatusLib : ScriptableObject
{
    [SerializeField] MiniGameStatus[] allMiniGameItems;



    public int GetIdFromName(string itemName)
    {
        for (int i = 0; i < allMiniGameItems.Length; i++)
        {
            if (allMiniGameItems[i].statusName != itemName) continue;
            else return i;
        }
        return -1;
    }

    public float GetDepleationFromId(int id)
    {
        return allMiniGameItems[id].depleationRate;
    }

    public string GetNameFromId(int id)
    {
        return allMiniGameItems[id].statusName;
    }

    public Sprite GetSpriteFromId(int id)
    {
        return allMiniGameItems[id].statusImage;
    }

    public MaintainRequirements[] GetRequirements(int id)
    {
        return allMiniGameItems[id].maintainableRequirements;
    }

    [System.Serializable]
    public class MiniGameStatus
    {
        public string statusName;
        public Sprite statusImage;
        public float depleationRate;
        public bool isDetramental;
        public MaintainRequirements[] maintainableRequirements;
    }

    [System.Serializable]
    public class MaintainRequirements
    {
        public string itemName;
        public int itemAmount;
    }

}
