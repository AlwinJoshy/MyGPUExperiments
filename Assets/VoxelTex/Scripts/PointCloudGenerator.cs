using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PointCloudGenerator : MonoBehaviour
{
    public Transform scanStarterPoint;
    public int checkCount;
    public int colorRes;
    public Texture2D surfaceTexture;
    public string fileSaveLocation;
    public string fileName;
    List<PointCloudData> points = new List<PointCloudData>();
    Vector3 rayStart, rayDir;
    Color[] colorArray;

    RaycastHit hit;

    void Start()
    {
        rayStart = scanStarterPoint.position;
        rayDir = scanStarterPoint.forward;
        colorArray = surfaceTexture.GetPixels(0);
        Debug.Log("pixel count : " + colorArray.Length);
    }

    int i = 0, emergency = 0;

    Color GetPointColor(Vector2 uv)
    {
        int x = (int)(uv.x * surfaceTexture.width);
        int y = (int)(uv.y * surfaceTexture.height);
        return colorArray[x + y * surfaceTexture.width];
    }

    void FixedUpdate()
    {

        while (i < checkCount && emergency < 1000000)
        {
            if (Physics.Raycast(rayStart, rayDir, out hit, 20))
            {
                if (!hit.collider.gameObject.CompareTag("Wall"))
                {
                    Vector2 uvCoord = hit.textureCoord;

                    Color color1 = GetPointColor(uvCoord);
                    int colInt = ColorToInt(color1, colorRes);
                    AddPointInfo(hit, colInt, color1);
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

    public void GeneratePCD()
    {
        points.Clear();
        i = 0;
        emergency = 0;
    }

    public void GeneratePCDFile()
    {
        Debug.Log("Generating PCD");
        FileSaveLoadManager.SaveFile(
              FileSaveLoadManager.Serialize(new ALPCDData(points)),
              fileSaveLocation,
              fileName,
              "pcd"
          );
        Debug.Log("PCD Succesfully Generated.");
    }

    int ColorToInt(Color colorValue, int resolution)
    {
        int x = (int)(colorValue.r * resolution);
        int y = (int)(colorValue.g * resolution);
        int z = (int)(colorValue.b * resolution);
        return (z * resolution * resolution) + (y * resolution) + x;
    }

    int crountLimit = 0;

    Color IntToColor(int intValue, int resolution)
    {
        int resX2 = resolution * resolution;
        int zValue = intValue / resX2;
        zValue = resolution - zValue < 0 ? resolution : zValue;

        int yxValue = intValue - (zValue * resX2);

        int yValue = yxValue / resolution;
        yValue = resolution - yValue < 0 ? resolution : yValue;

        int xValue = yxValue - (yValue * resolution);

        float r = (float)xValue / (float)resolution;
        float g = (float)yValue / (float)resolution;
        float b = (float)zValue / (float)resolution;

        return new Color(r, g, b);
    }

    void AddPointInfo(RaycastHit hit, int colorID, Color color)
    {
        points.Add(
            new PointCloudData()
            {
                pos = new ALVector3(hit.point),
                normal = new ALVector3(hit.normal),
                colorID = colorID,
                color = new ALColor(color)
            }
          );
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
        {
            //Gizmos.color = points[i].color;
            Gizmos.color = IntToColor(points[i].colorID, colorRes);
            Gizmos.DrawLine(points[i].pos.ShowAsVector(), points[i].pos.ShowAsVector() + points[i].normal.ShowAsVector() * 0.1f);
        }
    }



}

[System.Serializable]
public class ALPCDData
{
    public List<PointCloudData> pcdData;

    public ALPCDData(List<PointCloudData> pcdData)
    {
        this.pcdData = pcdData;
    }
}

[System.Serializable]
public class ALVector3
{
    public float x, y, z;
    public static Vector3 result = Vector3.zero;
    public ALVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public ALVector3(Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    public Vector3 ShowAsVector()
    {
        result.x = x;
        result.y = y;
        result.z = z;
        return result;
    }
}

[System.Serializable]
public class ALColor
{
    public float r, g, b, a;
    public static Color result = Color.black;
    public ALColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public ALColor(Color col)
    {
        this.r = col.r;
        this.g = col.g;
        this.b = col.b;
        this.a = col.a;
    }

    public Color ShowAsColor()
    {
        result.r = r;
        result.g = g;
        result.b = b;
        result.a = a;
        return result;
    }
}

[System.Serializable]
public struct PointCloudData
{
    public ALVector3 pos;
    public ALVector3 normal;
    public int colorID;
    public ALColor color;
}