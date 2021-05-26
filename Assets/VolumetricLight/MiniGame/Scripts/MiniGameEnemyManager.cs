using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameEnemyManager : MonoBehaviour
{
    [SerializeField] GameObject boatAi;
    [SerializeField] List<AIBrain> bomberBoats = new List<AIBrain>();

    public void SpawnEnemy(Vector3 spawnAt){
        for (int i = 0; i < bomberBoats.Count; i++)
        {
            if(bomberBoats[i].gameObject.activeSelf){
                bomberBoats[i].transform.position = spawnAt;
                bomberBoats[i].Initialize();
                return;
            }
        }
        GameObject newEnemy = Instantiate(boatAi, spawnAt, Quaternion.identity);
        bomberBoats.Add(newEnemy.GetComponent<AIBrain>());
    }

    public void Init(){
    }

    void Update() {
        
    }

}
