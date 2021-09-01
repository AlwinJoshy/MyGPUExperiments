using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDVisualizer : MonoBehaviour
{
    [SerializeField] string fileSaveLocation;
    [SerializeField] string fileName;
    [SerializeField] int colorRes;

    public ALPCDData pcdData = new ALPCDData(new List<PointCloudData>());

    private void Start()
    {
        LoadPCDFile();
    }

    public void LoadPCDFile()
    {
        pcdData = FileSaveLoadManager.Deserialize<ALPCDData>(
            FileSaveLoadManager.LoadFile(
                fileSaveLocation,
                fileName,
                "pcd"));
    }

    Color IntToColor(int intValue, int resolution)
    {

        int resX2 = resolution * resolution;
        int zValue = intValue / resX2;
        zValue = resolution - zValue < 0 ? resolution : zValue;

        int yxValue = intValue - (zValue * resX2);

        int yValue = yxValue / resolution;
        yValue = resolution - yValue < 0 ? resolution : yValue;

        int xValue = yxValue - (yValue * resolution);

        float r = ((float)xValue / (float)resolution);
        float g = (float)yValue / (float)resolution;
        float b = (float)zValue / (float)resolution;

        return new Color(r, g, b);
    }

    void OnDrawGizmos()
    {
        if (pcdData != null)
        {
            for (int i = 0; i < pcdData.pcdData.Count; i++)
            {
                //Gizmos.color = points[i].color;
                Gizmos.color = IntToColor(pcdData.pcdData[i].colorID, colorRes);
                Gizmos.DrawLine(
                    pcdData.pcdData[i].pos.ShowAsVector(),
                    pcdData.pcdData[i].pos.ShowAsVector() + pcdData.pcdData[i].normal.ShowAsVector() * 0.1f);
            }
        }
    }


}
