using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBrainTrade : MonoBehaviour, IColissionActions
{
    public string stateDisplay;
    [SerializeField] Transform targetLocation;
    [SerializeField] string blastSoundName;
    [SerializeField]
    float detectionRange,
    brainTickRate,
    combatRangeThrushold,
    waypointDetachRange,
    neibourPushawayStrength,
    frontVectorScale,
    restSpeedThrushold,
    navMeshSampleRange,
    reverseTurnStrength,
    destinationReachRange,
    idleTime;
    [SerializeField] AIShipControler aIShipControler;
    Vector3 tempVec3,
    lastPlayerPos,
    seporationVector;


    Transform targetDestination;
    NavMeshPath travelPoints;
    NavMeshHit navMeshHit;
    float nextTick = 0;
    int wayPointIndex;
    AIStates aiState;
    bool alive;

    enum AIStates
    {
        idle,
        reachTarget
    }

    void Start()
    {
        Initialize();
    }


    public void Initialize()
    {
        nextTick = Time.time + 1 / brainTickRate;
        travelPoints = new NavMeshPath();
        navMeshHit = new NavMeshHit();
        lastPlayerPos = Vector3.up * 100000;
        alive = true;
        gameObject.SetActive(true);

        // assign vessel to ArmadaManager
        ArmadaSeporator.instance.AddVessel(transform);
        // setup the simplified collission system
        MiniGameColissionManager.instance.AddCollider(0.5f, transform, this, ColissionSourceType.trader);
    }



    // Update is called once per frame
    void Update()
    {
        if (alive)
        {

            // Deciding making section
            if (Time.time > nextTick)
            {

                if (idleTime > 0)
                {
                    idleTime -= 0.3f;
                    aiState = AIStates.idle;
                }

                // calculate path to destination
                else if (travelPoints.corners.Length < 1)
                {
                    // get the waypoints
                    NavMesh.SamplePosition(MIniGameShopLocationManager.instance.GetMeAShop().position, out navMeshHit, 3, NavMesh.AllAreas);
                    bool pathFound = NavMesh.CalculatePath(transform.position, navMeshHit.position, NavMesh.AllAreas, travelPoints);
                    aiState = pathFound ? AIStates.reachTarget : AIStates.idle;
                    aIShipControler.Unbreak();
                    wayPointIndex = 0;
                }
                nextTick = Time.time + 1 / brainTickRate;
            }

            else
            {
                switch (aiState)
                {
                    case AIStates.reachTarget:
                        tempVec3 = travelPoints.corners[wayPointIndex] - transform.position;
                        if (tempVec3.sqrMagnitude < Mathf.Pow(waypointDetachRange, 2))
                        {
                            if (wayPointIndex < travelPoints.corners.Length - 1)
                            {
                                wayPointIndex++;
                            }
                            else
                            {
                                aiState = AIStates.idle;
                                idleTime = 3;
                                travelPoints.ClearCorners();
                                aIShipControler.Break();
                            }
                        }
                        else
                        {
                            seporationVector = ArmadaSeporator.instance.AvoidanceVector(transform.position);
                            float steerValue = Vector3.Dot(transform.right, tempVec3) + Vector3.Dot(transform.right, seporationVector) * neibourPushawayStrength;
                            float forwardPushAwayEffect = Vector3.Dot(transform.forward, seporationVector);
                            if (Vector3.Dot(aIShipControler.rigidbody.velocity, transform.forward) < restSpeedThrushold &&
                            !NavMesh.SamplePosition(transform.position + transform.forward * frontVectorScale, out navMeshHit, navMeshSampleRange, NavMesh.AllAreas))
                            {
                                forwardPushAwayEffect = -0.5f;
                                aIShipControler.SteerVesal(steerValue * reverseTurnStrength, forwardPushAwayEffect);
                            }
                            else
                            {
                                aIShipControler.SteerVesal(steerValue, Mathf.Min(Mathf.Min(tempVec3.sqrMagnitude * 0.5f, 1) + forwardPushAwayEffect, 1));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }


    public void Kill()
    {
        KillShip(true);
    }

    public void SilentKill()
    {
        KillShip(false);
    }

    void KillShip(bool showVisual)
    {
        alive = false;
        if (showVisual)
        {
            SoundManager.instance.PlaySound(blastSoundName, 0.5f, 1);
            MiniGameParticleLib.instance.PlayEffect(ParticleType.BoatBlastSmoke, transform.position, Vector3.up, true);
        }
        // distroy or disable the boat
        gameObject.SetActive(false);

        ArmadaSeporator.instance.RemoveVessel(transform);

        // remove the simplified collission system
        MiniGameColissionManager.instance.RemoveCollider(transform);
    }

    void OnDrawGizmosSelected()
    {
        if (travelPoints != null)
        {
            Gizmos.color = Color.red;
            for (int i = 1; i < travelPoints.corners.Length; i++)
            {
                Gizmos.DrawLine(travelPoints.corners[i - 1], travelPoints.corners[i]);
            }
        }
    }

    public void onBallEnter(BezierProjectile ball)
    {
        Kill();
    }
}
