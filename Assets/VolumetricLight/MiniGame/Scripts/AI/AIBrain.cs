using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AIBrain : MonoBehaviour, IColissionActions
{
    public int threatLevel = 3, importance, maxHealth;
    public EnemyType enemyType;
    public string stateDisplay;
    [SerializeField] UnityEvent OnInitialize;
    [SerializeField] MiniGameEnemyGun enemyWeapon;
    [SerializeField] string blastSoundName;
    [SerializeField] Vector2Int dropAmounts;
    [SerializeField]
    float detectionRange,
    combatRange,
    attackRange,
    brainTickRate,
    pathReCalculationThruchold,
    combatRangeThrushold,
    waypointDetachRange,
    neibourPushawayStrength,
    frontVectorScale,
    restSpeedThrushold,
    navMeshSampleRange,
    reverseTurnStrength,
    hitDeathRange,
    attackDelay;
    [SerializeField] AIShipControler aIShipControler;
    Vector3 tempVec3,
    lastPlayerPos,
    seporationVector;

    NavMeshPath travelPoints;
    NavMeshHit navMeshHit;
    float nextTick = 0,
    sqDist,
    nextAttackTime;
    int wayPointIndex, currentHealth;
    AIStates aiState;
    bool alive = false;

    enum AIStates
    {
        idle,
        reachTarget
    }

    public enum EnemyType
    {
        bomber,
        shooter
    }

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        Initialize();
    }

    void OnDisable()
    {
        Kill();
    }

    void OnDestroy()
    {
        Kill();
    }

    public void Initialize()
    {
        if (!alive)
        {
            Invoke("DelayedCall", 0.1f);
        }
    }

    void DelayedCall()
    {
        currentHealth = maxHealth;
        nextTick = Time.time + 1 / brainTickRate;
        if (travelPoints == null) travelPoints = new NavMeshPath();
        if (!navMeshHit.Equals(null)) navMeshHit = new NavMeshHit();
        lastPlayerPos = Vector3.up * 100000;
        alive = true;
        wayPointIndex = 0;
        gameObject.SetActive(true);

        // assign vessel to ArmadaManager
        ArmadaSeporator.instance.AddVessel(transform);
        if (enemyType == EnemyType.bomber)
        {
            // assign to ring drawer
            SeaWaveProvider.instance.AddMarkerRing(transform);
            // assign to explossion manager
            MiniGameBomber.instance.AddBoat(this);
        }
        // setup the simplified collission system
        MiniGameColissionManager.instance.AddCollider(0.5f, transform, this, ColissionSourceType.enemy);
        OnInitialize.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive && MiniGameGlobalRef.BeInControl())
        {
            tempVec3 = MiniGameGlobalRef.playerShip.position - transform.position;
            sqDist = Vector3.SqrMagnitude(tempVec3);

            // Deciding making section
            if (Time.time > nextTick)
            {

                // getting in combat range
                if (sqDist < Mathf.Pow(combatRange, 2) && nextAttackTime < Time.time)
                {
                    enemyWeapon?.Fire(Vector3.zero, Vector3.zero);
                    nextAttackTime = Time.time + attackDelay;
                }

                // calculate path to player
                else if (sqDist < Mathf.Pow(detectionRange, 2))
                {
                    // get the waypoints
                    if (Vector3.SqrMagnitude(lastPlayerPos - MiniGameGlobalRef.playerShip.position) > Mathf.Pow(pathReCalculationThruchold, 2) * sqDist * 0.2)
                    {
                        bool pathFound = NavMesh.CalculatePath(transform.position, MiniGameGlobalRef.playerShip.position, NavMesh.AllAreas, travelPoints);
                        aiState = pathFound ? AIStates.reachTarget : AIStates.idle;
                        lastPlayerPos = MiniGameGlobalRef.playerShip.position;
                        wayPointIndex = 0;
                    }
                    else
                    {
                        // when a path is ready
                    }
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
                            else aiState = AIStates.idle;
                        }
                        else
                        {
                            seporationVector = ArmadaSeporator.instance.AvoidanceVector(transform.position);
                            Debug.DrawRay(transform.position, seporationVector * 2, Color.blue);
                            float steerValue = Vector3.Dot(transform.right, tempVec3) + Vector3.Dot(transform.right, seporationVector) * neibourPushawayStrength;
                            float forwardPushAwayEffect = Vector3.Dot(transform.forward, seporationVector);
                            if (Vector3.Dot(aIShipControler.rigidbody.velocity, transform.forward) < restSpeedThrushold && !NavMesh.SamplePosition(transform.position + transform.forward * frontVectorScale, out navMeshHit, navMeshSampleRange, NavMesh.AllAreas))
                            {
                                forwardPushAwayEffect = -0.5f;
                                aIShipControler.SteerVesal(steerValue * reverseTurnStrength, forwardPushAwayEffect);
                            }
                            else
                            {
                                aIShipControler.SteerVesal(steerValue, Mathf.Min(Mathf.Min(tempVec3.sqrMagnitude * 0.2f, 1) + forwardPushAwayEffect, 1));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void HItObject()
    {
        if (sqDist < Mathf.Pow(hitDeathRange, 2) && enemyType == EnemyType.bomber)
        {
            Kill();
        }
    }


    public void Kill()
    {
        if (alive)
        {
            alive = false;
            SoundManager.instance?.PlaySound(blastSoundName, 0.5f, 1);
            aiState = AIStates.idle;
            MiniGameParticleLib.instance.PlayEffect(ParticleType.BoatBlastSmoke, transform.position, Vector3.up, true);
            // distroy or disable the boat
            gameObject.SetActive(false);

            ArmadaSeporator.instance.RemoveVessel(transform);
            if (enemyType == EnemyType.bomber)
            {
                // remove to ring drawer
                SeaWaveProvider.instance.RemoveMarkerRing(transform);
                // remove to explossion manager
                MiniGameBomber.instance.RemoveBoat(this);
            }
            // remove the simplified collission system
            MiniGameColissionManager.instance.RemoveCollider(transform);

            // return the enemy notoriety manager
            MiniGameUtilFunctions.ReturnObjToDictonary(gameObject);

            if (MiniGameGlobalRef.BeInControl())
            {
                MiniGameSpawnItemManager.instance.DropItems(dropAmounts, transform.position, 1f);
            }

        }

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
        MiniGameParticleLib.instance.PlayEffect(ParticleType.boatExplossion, transform.position, Vector3.up, true);
        if (--currentHealth < 0)
        {
            Kill();
            if (ball.colissionSourceType == ColissionSourceType.player)
            {
                MiniGameNotorietyManager.instance.Attacked(importance);
            }
        }
    }

}
