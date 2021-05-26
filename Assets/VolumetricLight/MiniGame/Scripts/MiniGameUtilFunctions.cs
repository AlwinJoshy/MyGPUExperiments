using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MiniGameUtilFunctions
{
    static Dictionary<string, SpawanableObject> enemyDictionary = new Dictionary<string, SpawanableObject>();
    public class SpawanableObject
    {
        public Queue<GameObject> objectInUse = new Queue<GameObject>();
        public Queue<GameObject> objectInReserve = new Queue<GameObject>();

        public SpawanableObject(GameObject newInstance)
        {
            AddNewObject(newInstance);
        }

        public void AddNewObject(GameObject newInstance)
        {
            objectInUse.Enqueue(newInstance);
        }
    }

    public static GameObject GetObjFromDictonary(GameObject spawnObject)
    {
        bool keyExists = enemyDictionary.ContainsKey(spawnObject.name);
        GameObject returnObject = null;

        if (keyExists)
        {
            SpawanableObject objectRack = enemyDictionary[spawnObject.name];

            if (enemyDictionary[spawnObject.name].objectInReserve.Count > 0)
            {
                returnObject = enemyDictionary[spawnObject.name].objectInReserve.Dequeue();
                enemyDictionary[spawnObject.name].objectInUse.Enqueue(returnObject);
            }
            else
            {
                returnObject = MiniGameUtilFunctionHolder.InstanciateGameObject(spawnObject);
                objectRack.AddNewObject(returnObject);
            }
        }
        else
        {
            returnObject = MiniGameUtilFunctionHolder.InstanciateGameObject(spawnObject);
            enemyDictionary.Add(spawnObject.name, new SpawanableObject(returnObject));
        }
        return returnObject;
    }

    public static void ReturnObjToDictonary(GameObject spawnObject)
    {
        spawnObject.SetActive(false);
        string trueFormat = spawnObject.name.Split('(')[0];
        bool keyExists = enemyDictionary.ContainsKey(trueFormat);

        if (keyExists)
        {
            SpawanableObject objectRack = enemyDictionary[trueFormat];
            objectRack.objectInReserve.Enqueue(spawnObject);
        }
    }


    public static float ClampLerp01(float x, float start, float end)
    {
        return Mathf.Min(Mathf.Max(x - start, 0) / (end - start), 1);
    }

}
