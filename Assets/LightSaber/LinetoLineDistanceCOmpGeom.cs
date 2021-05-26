using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class LinetoLineDistanceCOmpGeom : MonoBehaviour
{
    [SerializeField] Transform saberPoint, viewPoint;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3
  p1 = saberPoint.position,
  p2 = viewPoint.position,
  v1 = saberPoint.forward,
  v2 = viewPoint.forward;

        Debug.DrawRay(saberPoint.position, saberPoint.forward * 10, Color.white);
        Debug.DrawRay(viewPoint.position, viewPoint.forward * 10, Color.white);

        Vector3 nVec = Vector3.Cross(v1, v2);
        
            //    nVec /= nVec.magnitude;

                float distance = Vector3.Dot(nVec, p1 - p2)/ nVec.magnitude;

                Debug.Log(distance);
                


   //     Vector3 n1 = Vector3.Cross(v2, nVec);

  //      Vector3 c1 = p1 + v1 * (Vector3.Dot((p2 - p1), n1) / Vector3.Dot(n1, v1));

    //    Debug.DrawRay(c1, Vector3.up, Color.yellow);


    }
}
