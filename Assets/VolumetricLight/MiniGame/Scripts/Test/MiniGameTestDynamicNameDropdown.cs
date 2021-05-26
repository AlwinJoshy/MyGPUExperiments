using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameTestDynamicNameDropdown : MonoBehaviour
{
    public TestObjects[] allTypeSelection;

    public int selectedID;

    public string selectedType;

    [System.Serializable]
    public class TestObjects
    {
        public string name;
        public int type;
    }

    public int SetID(int ID)
    {
        return selectedID = ID;
    }

    public string[] AllObjectNames(TestObjects[] objectList)
    {
        List<string> displayStrings = new List<string>();
        if (objectList != null)
        {
            for (int i = 0; i < objectList.Length; i++)
            {
                displayStrings.Add(objectList[i].name + " | " + objectList[i].type);
            }
        }
        return displayStrings.ToArray();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MiniGameNotorietyManager.instance.AddNotoriety(30);
            Debug.Log("Click...");
        }
    }

}
