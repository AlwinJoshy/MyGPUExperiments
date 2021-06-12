using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRayMod : MonoBehaviour
{
    public Transform traceRay;

    void Update()
    {

    }

    int Step(float a, float x){
        if((int)(a * 1000) == (int)(x * 1000)) return -1;
        return 1;
    }

    void OnDrawGizmos()
    {

        Vector3 o = Vector3.zero;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(o, Vector3.one);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(traceRay.position, traceRay.forward * 20);

        float wallDist_X = o.x - traceRay.position.x;
        float wallDist_Y = o.y - traceRay.position.y;
        float wallDist_Z = o.z - traceRay.position.z;


        float rayFactX = traceRay.forward.x;
        float rayFactY = traceRay.forward.y;
        float rayFactZ = traceRay.forward.z;

        float closestWallX = wallDist_X + 0.5f * Mathf.Sign(rayFactX);
        float closestWallY = wallDist_Y + 0.5f * Mathf.Sign(rayFactY);
        float closestWallZ = wallDist_Z + 0.5f * Mathf.Sign(rayFactZ);

        float xStepFac = closestWallX / rayFactX;
        float yStepFac = closestWallY / rayFactY;
        float zStepFac = closestWallZ / rayFactZ;

        float shortestStep = xStepFac < yStepFac ? xStepFac < zStepFac ? xStepFac : zStepFac < yStepFac ? zStepFac : yStepFac : yStepFac < zStepFac ? yStepFac : zStepFac < xStepFac ? zStepFac : xStepFac;

     //   Debug.Log(xStepFac + " " + yStepFac + " " + zStepFac + " " + shortestStep);
        
        Vector3 hitPoint = traceRay.forward * shortestStep + traceRay.position;
        Vector3 modPoint = new Vector3(
            hitPoint.x * Step(shortestStep, xStepFac), 
            hitPoint.y * Step(shortestStep, yStepFac), 
            hitPoint.z * Step(shortestStep, zStepFac));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitPoint, Vector3.one * 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(modPoint, Vector3.one * 0.1f);

        /*
        Gizmos.color = Color.red;
        Gizmos.DrawRay(traceRay.position, Vector3.right * rayFactX);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(traceRay.position, Vector3.up * rayFactY);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(traceRay.position, Vector3.forward * rayFactZ);
        */


        Gizmos.color = Color.red;
        Gizmos.DrawRay(traceRay.position, Vector3.right * closestWallX);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(traceRay.position, Vector3.up * closestWallY);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(traceRay.position, Vector3.forward * closestWallZ);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(traceRay.position, traceRay.forward * shortestStep);


    }

}
