using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDTextureGenerator : MonoBehaviour
{
    public int textureRes = 32;
    public int colorRes;
    public string textureSaveLocation;
    public string textureName;
    public string pcdFolderLocation;
    public string pcdFileName;

    public ALPCDData pcdData = new ALPCDData(new List<PointCloudData>());

    public void LoadAndGeneratePCDTex()
    {
        pcdData = FileSaveLoadManager.Deserialize<ALPCDData>(
          FileSaveLoadManager.LoadFile(
              pcdFolderLocation,
              pcdFileName,
              "pcd"));

        SaveAsTexture(CreateTexture(pcdData.pcdData));
        Debug.Log("Texture Generated");
    }

    public Texture2D CreateTexture(List<PointCloudData> points)
    {
        Texture2D tex = new Texture2D(textureRes, textureRes, TextureFormat.RGBA64, 0, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        Color[] positionColors = new Color[textureRes * textureRes];

        for (int i = 0; i < positionColors.Length; i++)
        {
            Vector4 pos;
            if (i < points.Count)
            {
                pos = (points[i].pos.ShowAsVector() / 20) * 0.5f + Vector3.one * 0.5f;
                pos.w = (float)points[i].colorID / (
                    Mathf.Pow((float)colorRes, 3) +
                    Mathf.Pow((float)colorRes, 2) + 
                    colorRes
                    );
                if(pos.w > 1)Debug.Log("Greater than 1 : " + pos.w);
            }
            else
            {
                pos = Vector4.one * 0.5f;
            }
            positionColors[i] = new Color(pos.x, pos.y, pos.z, pos.w);
        }

        tex.SetPixels(0, 0, textureRes, textureRes, positionColors);
        tex.Apply();

        Debug.Log("Generated...");
        return tex;
    }

    void SaveAsTexture(Texture2D tex)
    {
        if (!System.IO.Directory.Exists(textureSaveLocation))
        {
            System.IO.Directory.CreateDirectory(textureSaveLocation);
        }
        byte[] _bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{textureSaveLocation}/{textureName}.png", _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + textureSaveLocation);
        UnityEditor.AssetDatabase.Refresh();
    }
}
