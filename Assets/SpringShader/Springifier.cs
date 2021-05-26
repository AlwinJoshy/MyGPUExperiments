using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Springifier : MonoBehaviour
{
    MeshFilter mR;
    Mesh localMesh;
    [SerializeField] Transform reachPoint;
    [SerializeField] Material material;
    float springHeight;

    Color[] vertCols;

    void Start()
    {
        springHeight = mR.mesh.bounds.size.y;
        /*
        mR = GetComponent<MeshFilter>();
        localMesh = mR.mesh;
        mR.mesh = localMesh;
        vertCols = new Color[localMesh.vertices.Length];
        */
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 springDir = reachPoint.position - transform.position;

        transform.up = springDir;

        material.SetFloat("_Height", springDir.magnitude/springHeight);


        /*
        for (int i = 0; i < vertCols.Length; i++)
        {
            vertCols[i] = new Color(springHeight / springDir.magnitude, 0, 0);
        }
        localMesh.colors = vertCols;
        */

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, reachPoint.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(reachPoint.position, 0.5f);
    }

}
