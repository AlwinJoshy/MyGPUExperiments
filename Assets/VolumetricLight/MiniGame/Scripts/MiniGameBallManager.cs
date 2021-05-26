using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBallManager : MonoBehaviour
{
    public static MiniGameBallManager instance;
    [SerializeField] GameObject ballBody;
    List<BezierProjectile> allActiveBalls = new List<BezierProjectile>();
    Vector3 temp1, temp2, temp3;
    public float speed, speedFactor;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        for (int i = 0; i < allActiveBalls.Count; i++)
        {
            if (allActiveBalls[i].alive)
            {
                temp3 = allActiveBalls[i].objBody.position;
                temp1 = Vector3.Lerp(allActiveBalls[i].startPoint, allActiveBalls[i].midPoint, allActiveBalls[i].phase);
                temp2 = Vector3.Lerp(allActiveBalls[i].midPoint, allActiveBalls[i].endPoint, allActiveBalls[i].phase);
                allActiveBalls[i].objBody.position = Vector3.Lerp(temp1, temp2, allActiveBalls[i].phase);
                allActiveBalls[i].objBody.forward = temp3 - allActiveBalls[i].objBody.position;
                allActiveBalls[i].phase += Time.deltaTime * allActiveBalls[i].curveTime * speedFactor;
                // death check
                if (MiniGameColissionManager.instance.CheckColission(allActiveBalls[i]))
                {
                    allActiveBalls[i].deathType = 2;
                    allActiveBalls[i].Kill();
                }
                else if (allActiveBalls[i].phase > 1) allActiveBalls[i].Kill();
            }
        }
    }

    public void AddNewCannonBall(BezierProjectile newBall, ColissionSourceType srcType, GameObject ammoObjTemplate)
    {
        allActiveBalls.Add(newBall);
        newBall.SetBody(MiniGameUtilFunctions.GetObjFromDictonary(ammoObjTemplate).transform, srcType);
    }

}

public class BezierProjectile
{
    public bool alive;
    public float phase, curveTime;
    public int deathType;
    public Transform objBody;
    public Vector3 startPoint, midPoint, endPoint;
    public ColissionSourceType colissionSourceType;

    public void Ready(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float length, int deathType, ColissionSourceType sourceType, float speed)
    {
        this.startPoint = startPoint;
        this.midPoint = midPoint;
        this.endPoint = endPoint;
        this.deathType = deathType;
        alive = true;
        objBody.gameObject.SetActive(true);
        phase = 0;
        curveTime = 1 / (length / speed);
        colissionSourceType = sourceType;
    }

    public void SetBody(Transform ballBody, ColissionSourceType sourceType)
    {
        objBody = ballBody;
        colissionSourceType = sourceType;
    }

    public void Kill()
    {
        alive = false;
        objBody.gameObject.SetActive(false);
        MiniGameGlobalRef.allBalls.Enqueue(this);
        switch (deathType)
        {
            // hit water
            case 0:
                MiniGameParticleLib.instance.PlayEffect(ParticleType.CannonWaterRipple, objBody.position, Vector3.up, false);
                break;

            // hit land
            case 1:
                MiniGameParticleLib.instance.PlayEffect(ParticleType.GroundHit, objBody.position, Vector3.up, false);
                break;

            // hit hit boat
            case 2:
                MiniGameParticleLib.instance.PlayEffect(ParticleType.BoatHit, objBody.position, Vector3.up, false);
                break;

        }
    }

}