using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vertPosColor : MonoBehaviour
{

    MeshRenderer mR;
    Vector3 lastPos;


    void Start()
    {
        mR = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.SqrMagnitude(transform.position - lastPos) > 0.01f)
        {

            

            lastPos = transform.position;
        }
    }
}
