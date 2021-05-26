using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBomber : MonoBehaviour
{

    public static MiniGameBomber instance;

    public string beepSoundClipName, explossionSoundClipName;
    [SerializeField] ParticleSystem blastEffect;
    List<AIBrain> bomberBoats = new List<AIBrain>();

    [SerializeField] float senceSpeed, decaySpeed, sceneRange, beepDelay;

    float explossionChance, nextBeep;
    Vector3 tempVec3;
    AIBrain closestBoat;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

        float rangeSquare = Mathf.Pow(sceneRange, 2),
        dirSquarMag;
        bool inRange = false;
        for (int i = 0; i < bomberBoats.Count; i++)
        {
            tempVec3 = MiniGameGlobalRef.playerShip.position - bomberBoats[i].transform.position;
            dirSquarMag = tempVec3.sqrMagnitude;
            if (dirSquarMag < rangeSquare)
            {
                explossionChance += (1 - dirSquarMag / rangeSquare) * senceSpeed * Time.deltaTime;
                inRange = true;
                closestBoat = bomberBoats[i];
            }
        }

        // if its above thrushold
        if (explossionChance > 0.95f)
        {
            Blast(closestBoat);
            // reset after blast
            explossionChance = 0;
            Debug.Log("Explode");
        }

        else if (explossionChance > 0.1f)
        {
            if (Time.time > nextBeep)
            {
                SoundManager.instance.PlaySound(beepSoundClipName, 0.1f, 1);
                nextBeep = Time.time + (1 - explossionChance) * beepDelay;
            }
        }

        if (!inRange) explossionChance = explossionChance <= 0 ? 0 : explossionChance - decaySpeed * Time.deltaTime;

    }

    void Blast(AIBrain blastBoat)
    {
        blastBoat.Kill();
        // damage the player
        //    MiniGameGlobalRef.playerStat.RemoveHeart();
        MiniGamePlayerDeathManager.instance.CalculateHitDamage();
    }

    public void AddBoat(AIBrain bombBoat)
    {
        bomberBoats.Add(bombBoat);
    }

    public void RemoveBoat(AIBrain bombBoat)
    {
        bomberBoats.Remove(bombBoat);
    }

}
