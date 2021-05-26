using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameNotorietyManager : MonoBehaviour
{
    public static MiniGameNotorietyManager instance;
    public static Transform allEnemyParent, dormentEnemyParent;


    public Transform enemyContainer, dormentEnemyContainer;
    public NotorietyLevelControl[] allNotorietyLevel;
    public Vector3[] spawnLocations;
    public MalitiaTire[] attackLevelTires;

    public TextMeshProUGUI wantedLevelNum, wantedLevelName;
    [SerializeField] GameObject notorietyPanel;

    public float attackDelay = 20;


    [System.Serializable]
    public class NotorietyLevelControl
    {
        public string stageName;
        public int maxValue;
        public int offecncePower;
        public int offeceCatagory;
    }

    [System.Serializable]
    public class MalitiaTire
    {
        public string tireName;
        public AIBrain[] enemyList;
    }


    [SerializeField] float notorietyPerValue, notorietyPerImportance;
    bool notorietyUpdated = false;
    int currentNotorietyLevel = 0;
    float nextAttackTime = 10;
    RaycastHit hit = new RaycastHit();
    List<AIBrain> enemyList = new List<AIBrain>();

    private void Start()
    {
        allEnemyParent = enemyContainer;
        dormentEnemyParent = dormentEnemyContainer;
        instance = this;
    }




    #region Inspector_GUI_Functions
    public void AddNewLocationPoint()
    {
        System.Array.Resize(ref spawnLocations, spawnLocations.Length + 1);
        spawnLocations[spawnLocations.Length - 1] = spawnLocations.Length > 1 ? spawnLocations[spawnLocations.Length - 2] + transform.forward : transform.position + transform.forward;
    }
    public void RemoveSelectedLocationPoint(int id)
    {
        if (spawnLocations.Length > 0)
        {
            spawnLocations[id] = spawnLocations[spawnLocations.Length - 1];
            if (spawnLocations.Length > 1)
            {
                System.Array.Resize(ref spawnLocations, spawnLocations.Length - 1);
            }
            else spawnLocations = new Vector3[] { };
        }
    }
    #endregion


    void Update()
    {
        if (notorietyUpdated)
        {
            for (int i = currentNotorietyLevel; i < allNotorietyLevel.Length; i++)
            {
                if (MiniGameGlobalRef.playerStat.notoriety > allNotorietyLevel[i].maxValue)
                {
                    continue;
                }
                else
                {
                    currentNotorietyLevel = i;
                    break;
                }
            }
            notorietyUpdated = false;
        }
        else if (MiniGameGlobalRef.playerStat.notoriety > 0 && Time.time > nextAttackTime && enemyContainer.childCount < 1)
        {
            IssueHit();
            nextAttackTime = Time.time + attackDelay;
        }
    }

    public void IssueHit()
    {

        notorietyPanel.SetActive(true);
        wantedLevelNum.text = (currentNotorietyLevel + 1).ToString();
        wantedLevelName.text = allNotorietyLevel[currentNotorietyLevel].stageName;

        // pick a spawn location
        Vector3 spawnPoint = Vector3.zero;
        float dist = 0;
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            float calcDistance = Vector3.SqrMagnitude(spawnLocations[i] - MiniGameGlobalRef.playerShip.transform.position);
            if (dist < calcDistance)
            {
                spawnPoint = spawnLocations[i];
                dist = calcDistance;
            }
        }

        // generate the enemy list
        enemyList.Clear();
        int collectiveEnemyGenerationValue = 0;
        bool listFilled = false;

        while (!listFilled)
        {

            AIBrain picked = attackLevelTires[currentNotorietyLevel].enemyList[Random.Range(0, attackLevelTires[currentNotorietyLevel].enemyList.Length)];

            if (collectiveEnemyGenerationValue + picked.threatLevel <= allNotorietyLevel[currentNotorietyLevel].offecncePower)
            {
                collectiveEnemyGenerationValue += picked.threatLevel;
                enemyList.Add(picked);
            }
            else
            {
                listFilled = true;
            }

        }

        GameObject newEnemy = null;

        for (int i = 0; i < enemyList.Count; i++)
        {

            while (true)
            {
                Vector2 randomVec = Random.insideUnitCircle * 2;
                Vector3 rayCastPoist = spawnPoint + new Vector3(randomVec.x, 0, randomVec.y) * 0.5f;
                if (Physics.Raycast(rayCastPoist, Vector3.down * 5, out hit))
                {
                    Debug.DrawRay(rayCastPoist, Vector3.up * 10, Color.red, 20);
                    break;
                }
            }

            newEnemy = MiniGameUtilFunctions.GetObjFromDictonary(enemyList[i].gameObject);
            newEnemy.transform.parent = enemyContainer;
            newEnemy.transform.position = hit.point;
            newEnemy.SetActive(true);
        }


    }

    public void AddNotoriety(int value)
    {
        MiniGameGlobalRef.playerStat.notoriety += value;
        notorietyUpdated = true;
    }

    public void Stolen(int value)
    {
        AddNotoriety((int)(value * notorietyPerValue));
    }

    public void Attacked(int importance)
    {
        AddNotoriety((int)(importance * notorietyPerImportance));
    }

}
