using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameEnemyGun : MonoBehaviour
{
    public float hitPlaneHeight, mouseShiftThrushold, bulletSpeed;
    public LayerMask layerMask;
    [SerializeField] Transform firePointTransform;
    [SerializeField] GameObject cannonBall;
    [SerializeField] float maxFireDistance, predectiveDistance, fireDelay, fireSpread;
    [SerializeField] int burstFireCount;
    Camera mainCamera;
    Vector3 temp, hitPoint;
    Ray mouseRay;
    RaycastHit hit;
    bool fired;
    int clickedSurfaceID = 0, burstsRemaining;
    Vector2 lastPos, tempVec2;
    float nextFire;

    void Start()
    {
        mainCamera = Camera.main;
    }


    public void Update()
    {
        if (burstsRemaining > 0 && Time.time > nextFire)
        {
            CannonFire();
            burstsRemaining--;
            nextFire = Time.time + fireDelay;
        }
    }

    public void Fire(Vector3 firePoint, Vector3 hitPoint)
    {
        burstsRemaining = burstFireCount;
    }

    public void CannonFire()
    {
        Vector3 firePoint = firePointTransform.position;
        Vector2 randomCircle = Random.insideUnitCircle;
        Vector3 tempCircle = Vector3.zero;
        tempCircle.x = randomCircle.x;
        tempCircle.z = randomCircle.y;
        tempCircle *= fireSpread;
        Vector3 hitPoint = tempCircle + MiniGameGlobalRef.playerShip.position + MiniGameGlobalRef.playerRB.velocity * predectiveDistance;
        if (Physics.Raycast(firePoint + Vector3.up * 2, Vector3.down, out hit, 5, layerMask))
        {
            MiniGameMouseOverSence mouseOverObject = hit.collider.gameObject.GetComponent<MiniGameMouseOverSence>();
            clickedSurfaceID = mouseOverObject.strikeSurfaceType;
        }
        Vector3 fireDir = hitPoint - firePoint;
        float straigDirhtLength = fireDir.magnitude;
        temp = fireDir;
        temp.y = 0;
        Vector3 midPoint = firePoint + (fireDir * 0.5f) + Vector3.up * temp.magnitude * 0.3f;

        // finding the length of the curve
        float midPointToStart = (midPoint - firePoint).magnitude;
        float midPointToEnd = (midPoint - hitPoint).magnitude;

        // based on an approximation method
        // https://raphlinus.github.io/curves/2018/12/28/bezier-arclength.html
        // 0.666667 = 2/3
        // 0.333333334 = 1/3
        float curveLength = straigDirhtLength * 0.666667f + (straigDirhtLength + midPointToStart + midPointToEnd) * 0.3333334f;

        BezierProjectile getBall = GetABall();

        getBall.Ready(firePoint, midPoint, hitPoint, curveLength, clickedSurfaceID, ColissionSourceType.player, bulletSpeed);
    }

    public BezierProjectile GetABall()
    {
        BezierProjectile pickedBall = null;
        if (MiniGameGlobalRef.allBalls.Count > 0)
        {
            pickedBall = MiniGameGlobalRef.allBalls.Peek();
            MiniGameGlobalRef.allBalls.Dequeue();
            return pickedBall;
        }
        else
        {
            pickedBall = new BezierProjectile();
            MiniGameBallManager.instance.AddNewCannonBall(pickedBall, ColissionSourceType.player, cannonBall);
            return pickedBall;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint, 0.2f);
    }
}
