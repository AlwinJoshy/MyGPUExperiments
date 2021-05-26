using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MiniGameUtilFunctionHolder : MonoBehaviour
{

    public static MiniGameUtilFunctionHolder instance;

    private void Awake()
    {
        instance = this;
    }

    public static GameObject InstanciateGameObject(GameObject obj)
    {
        return instance.Active_InstanciateGameObject(obj);
    }

    public GameObject Active_InstanciateGameObject(GameObject obj)
    {
        return Instantiate(obj);
    }

    public static void SetParentToDorm(Transform obj)
    {
        instance.Active_SetParentToDorm(obj);
    }

    public void Active_SetParentToDorm(Transform obj)
    {
        obj.parent = MiniGameGlobalRef.inActiveObjectParent;
    }

}
