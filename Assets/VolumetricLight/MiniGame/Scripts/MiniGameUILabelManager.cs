using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUILabelManager : MonoBehaviour
{
    public static MiniGameUILabelManager instance;
    public float locationDisplaySpeed;
    [SerializeField] GameObject labelPrefab;
    List<MiniGameLocationDisplay> labels = new List<MiniGameLocationDisplay>();
    GameObject tempGameObject;
    MiniGameLocationDisplay tempLocationDisplay;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        for (int i = 0; i < labels.Count; i++)
        {
            if (labels[i].active)
            {
                labels[i].DeltaValue();
            }
        }
    }

    public MiniGameLocationDisplay GetLabel()
    {
        tempGameObject = null;
        tempGameObject = MiniGameUtilFunctions.GetObjFromDictonary(labelPrefab);
        tempGameObject.transform.parent = transform;
        tempLocationDisplay = tempGameObject.GetComponent<MiniGameLocationDisplay>();
        labels.Add(tempLocationDisplay);
        return tempLocationDisplay;
    }

    public void RemoveLabel(MiniGameLocationDisplay label)
    {
        label.active = false;
        MiniGameUtilFunctions.ReturnObjToDictonary(label.gameObject);
        labels.Remove(label);
    }

}
