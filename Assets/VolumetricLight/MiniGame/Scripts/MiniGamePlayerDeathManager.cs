using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MiniGamePlayerDeathManager : MonoBehaviour
{
    public static MiniGamePlayerDeathManager instance;

    [SerializeField] float healthRegenSpeed, tensinDropSpeed, curtainCloseSpeed;
    [SerializeField] int totalHitEffect;
    [SerializeField] UnityEvent OnDeathEvent, OnScreenOutEvent, OnGameOverShownEvent;
    [SerializeField] Color hurtColor;
    [SerializeField] Material playerDeathMaterial;
    [SerializeField] TextMeshProUGUI gameOverText;
    Color tempColor;


    float playerHealth, tensionEffect, tempFloat;
    bool alive;
    bool screenClosed, gameOverShown;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    void OnDestroy()
    {
        Init();
    }

    public void Init()
    {
        playerHealth = 1;
        alive = true;
        tempFloat = 0;
        tensionEffect = 0;
        screenClosed = false;
        gameOverShown = false;

        gameOverText?.gameObject?.SetActive(false);
        tempFloat += Time.deltaTime * curtainCloseSpeed;
        tempColor = gameOverText.color;
        tempColor.a = 0;
        gameOverText.color = tempColor;

        playerDeathMaterial.SetFloat("_VingetCtrl", (1 - playerHealth));
        playerDeathMaterial.SetFloat("_HitImpactCtrl", tensionEffect);
        playerDeathMaterial.SetFloat("_StackingLayer", tempFloat);
    }

    void Update()
    {
        tensionEffect -= tensionEffect > 0 ? Time.deltaTime * tensinDropSpeed : 0;
        if (alive)
        {
            MaterialControl();
            playerHealth += Time.deltaTime * healthRegenSpeed;
            playerHealth = playerHealth > 1 ? 1 : playerHealth;
        }
        else if (!screenClosed)
        {
            tempFloat += Time.deltaTime * curtainCloseSpeed;
            playerDeathMaterial.SetFloat("_HitImpactCtrl", tensionEffect);
            playerDeathMaterial.SetFloat("_StackingLayer", tempFloat);
            if (tempFloat >= 1)
            {
                gameOverText.gameObject.SetActive(true);
                OnScreenOutEvent.Invoke();
                screenClosed = true;
                tempFloat = 0;
            }
        }

        else if (!gameOverShown)
        {
            tempFloat += Time.deltaTime * curtainCloseSpeed;
            tempColor = gameOverText.color;
            tempColor.a = tempFloat;
            gameOverText.color = tempColor;
            if (tempFloat > 1)
            {
                OnGameOverShownEvent.Invoke();
                gameOverShown = true;
            }
        }

    }

    void MaterialControl()
    {
        playerDeathMaterial.SetFloat("_VingetCtrl", (1 - playerHealth));
        playerDeathMaterial.SetFloat("_HitImpactCtrl", tensionEffect);
    }

    public void CalculateHitDamage()
    {
        tensionEffect = 1;
        if (alive)
        {
            playerHealth -= 1.0f / (float)MiniGameGlobalRef.playerStat.shipParams.GetAttributeWithName("Health").GetValue();

            if (playerHealth < 0)
            {
                alive = false;
                OnDeathEvent.Invoke();
            }
        }
    }

}
