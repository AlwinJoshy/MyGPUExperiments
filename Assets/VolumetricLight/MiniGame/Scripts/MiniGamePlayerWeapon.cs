using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePlayerWeapon : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] protected int bulletSpeed;
    [SerializeField] protected Transform firePoint;
    [SerializeField] LayerMask layerMask;
    [SerializeField] FireType fireType;

    [Header("Interval Fire")] [SerializeField] float fireRate;

    private Camera mainCamera;
    private Vector3 tempVec2;
    private Ray mouseRay;
    private RaycastHit hit;

    [HideInInspector] protected Vector3 temp;
    float nextFire;
    public enum FireType
    {
        interval,
        continues,
        burst,
        charge
    }

    public virtual void Update()
    {
        if (MiniGameGlobalRef.BeInControl() && Input.GetAxis("Fire1") > 0)
        {
            switch (fireType)
            {
                case FireType.interval:
                    if (Time.time > nextFire)
                    {
                        ReadyAndFire();
                        nextFire = Time.time + 1 / fireRate;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public virtual void ReadyAndFire()
    {
        System.Object checkData = GetHitLocation();
        if (checkData != null)
        {
            Vector3? hitPoint = checkData.GetType().GetProperty("point").GetValue(checkData, null) as Vector3?;
            int? clickedSurfaceID = checkData.GetType().GetProperty("surfaceID").GetValue(checkData, null) as int?;
            Fire((Vector3)hitPoint, (int)clickedSurfaceID);
        }
    }

    public virtual void Start()
    {
        mainCamera = Camera.main;
    }

    public virtual System.Object GetHitLocation()
    {
        tempVec2 = Input.mousePosition;
        mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mainCamera.transform.position, mouseRay.direction, out hit, 100, layerMask))
        {
            MiniGameMouseOverSence mouseOverObject = hit.collider.gameObject.GetComponent<MiniGameMouseOverSence>();
            if (mouseOverObject)
            {
                return new { point = hit.point, surfaceID = mouseOverObject.strikeSurfaceType } as System.Object;
            }
        }
        return null;
    }

    public virtual void Fire(Vector3 hitPoint, int clickedSurfaceID)
    {
        Vector3 fireDir = (Vector3)hitPoint - firePoint.position;
        float straigDirhtLength = fireDir.magnitude;
        temp.y = 0;
        Vector3 midPoint = firePoint.position + (fireDir * 0.5f) + Vector3.up * straigDirhtLength * 0.3f;

        // finding the length of the curve
        float midPointToStart = (midPoint - firePoint.position).magnitude;
        float midPointToEnd = (midPoint - (Vector3)hitPoint).magnitude;

        // based on an approximation method
        // https://raphlinus.github.io/curves/2018/12/28/bezier-arclength.html
        // 0.666667 = 2/3
        // 0.333333334 = 1/3
        float curveLength = straigDirhtLength * 0.666667f + (straigDirhtLength + midPointToStart + midPointToEnd) * 0.3333334f;

        GetAnAmmo().Ready(firePoint.position, midPoint, (Vector3)hitPoint, curveLength, (int)clickedSurfaceID, ColissionSourceType.player, bulletSpeed);
    }


    protected BezierProjectile GetAnAmmo()
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
            MiniGameBallManager.instance.AddNewCannonBall(pickedBall, ColissionSourceType.player, bulletPrefab);
            return pickedBall;
        }
    }

}