using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayIntersectionTest : MonoBehaviour
{

    public Transform dirObj;

    Vector3 minPt, maxPt;
    
    // reach vec
    Vector3 dirXReach, dirYReach, dirZReach;

    Vector3 touchVector;

    void Start()
    {
        minPt = Vector3.zero;
        maxPt = Vector3.one * 2;
    }

    // Update is called once per frame
    void Update()
    {
        float dirXVal = Vector3.Dot(Vector3.right, dirObj.forward);
        float dirYVal = Vector3.Dot(Vector3.up, dirObj.forward);
        float dirZVal = Vector3.Dot(Vector3.forward, dirObj.forward);

        float dirXLength = ((maxPt.x - dirObj.position.x) * Step(0, dirXVal) + (minPt.x - dirObj.position.x) * Step(dirXVal, 0));
        float dirYLength = ((maxPt.y - dirObj.position.y) * Step(0, dirYVal) + (minPt.y - dirObj.position.y) * Step(dirYVal, 0));
        float dirZLength = ((maxPt.z - dirObj.position.z) * Step(0, dirZVal) + (minPt.z - dirObj.position.z) * Step(dirZVal, 0));

        float xDiv = dirXLength/ dirXVal;
        float yDiv = dirYLength/ dirYVal;
        float zDiv = dirZLength/ dirZVal;

        float smallDiv = xDiv < yDiv ? xDiv < zDiv ? xDiv : zDiv : yDiv < zDiv ? yDiv : zDiv;

        touchVector = dirObj.forward * smallDiv;

        dirXReach = Vector3.right * dirXLength;
        dirYReach = Vector3.up * dirYLength;
        dirZReach = Vector3.forward * dirZLength;
    }

    float Step(float a, float x){
        return x >= a ? 1 : 0;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(Vector3.one * 2 * 0.5f, Vector3.one * 2);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(dirObj.position, dirObj.forward * 20);
        Gizmos.color = Color.white;
        Gizmos.DrawRay(dirObj.position, dirObj.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(dirObj.position, dirXReach);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(dirObj.position, dirYReach);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(dirObj.position, dirZReach);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(dirObj.position + touchVector, 0.05f);


        

    }

}
