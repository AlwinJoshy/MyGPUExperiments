using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PointCloudGenerator : MonoBehaviour
{
    [SerializeField] Transform scanStarterPoint;
    [SerializeField] int checkCount;
    List<PointCloudData> points = new List<PointCloudData>();
    Vector3 rayStart, rayDir;

    RaycastHit hit;

    public struct PointCloudData
    {
        public Vector3 pos;
        public int colorID;
    }

    void Start()
    {
        rayStart = scanStarterPoint.position;
        rayDir = scanStarterPoint.forward;
    }

    int i = 0, emergency = 0;
    void FixedUpdate()
    {
        
        while (i < checkCount && emergency < 1000000)
        {
            if (Physics.Raycast(rayStart, rayDir, out hit, 20))
            {
                if (!hit.collider.gameObject.CompareTag("Wall"))
                {
                    Color color = hit.collider.gameObject.GetComponent<MeshRenderer>().material.color;
                    int colInt = ColorToInt(color, 10);
                    Debug.Log(colInt);
                    AddPointInfo(hit, colInt);
                    i++;
                }
                else
                {

                }
                rayStart = hit.point;
                rayDir = Vector3.Reflect(rayDir + Random.insideUnitSphere.normalized * 0.1f, hit.normal);
            }
            emergency++;
        }
    }

    int ColorToInt(Color colorValue, int resolution){
        int x = (int)(colorValue.r * resolution);
        int y = (int)(colorValue.g * resolution);
        int z = (int)(colorValue.b * resolution);

        return (y * resolution) + (z * resolution * resolution) + x;
    }

    void AddPointInfo(RaycastHit hit, int colorID){
        //points.Add(hit.point);
      //  point.Add(new PointCloudData{})
      points.Add(
          new PointCloudData(){
              pos = hit.point,
              colorID = colorID
              }
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawWireCube(points[i].pos, Vector3.one * 0.1f);
        }
    }
}
