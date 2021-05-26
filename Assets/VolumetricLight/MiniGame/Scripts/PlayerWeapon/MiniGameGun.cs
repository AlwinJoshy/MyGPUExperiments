using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameGun : MiniGamePlayerWeapon
{

    public override void Fire(Vector3 hitPoint, int clickedSurfaceID)
    {
        Vector3 fireDir = (Vector3)hitPoint - firePoint.position;
        float straigDirhtLength = fireDir.magnitude;
        temp.y = 0;
        Vector3 midPoint = firePoint.position + (fireDir * 0.5f) + Vector3.up * straigDirhtLength * 0.05f;
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
}
