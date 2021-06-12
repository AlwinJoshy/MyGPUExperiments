using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneColissionTest : MonoBehaviour
{

    public Transform r;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawLine(transform.position + transform.right + transform.forward, transform.position + transform.right - transform.forward);
        Gizmos.DrawLine(transform.position - transform.right + transform.forward, transform.position - transform.right - transform.forward);
        Gizmos.DrawLine(transform.position - transform.right - transform.forward, transform.position + transform.right - transform.forward);
        Gizmos.DrawLine(transform.position - transform.right + transform.forward, transform.position + transform.right + transform.forward);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);


        Vector3 dirVP = r.position - transform.position;
        float perpVP = Vector3.Dot(transform.up, dirVP);
  
        float projVD = Vector3.Dot(r.forward, -transform.up);
        float minStep = perpVP / projVD;
        //  return length(projVD);
        Vector3 rayHitPoint = r.position + (r.forward * minStep);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(r.position, rayHitPoint);


    }
}
