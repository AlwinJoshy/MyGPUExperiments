using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VoxelDisplay : MonoBehaviour
{
    public string saveLocation;
    public string textureName;
    public Transform pointContainer;
    public float containerSize;
    public int texSize;


    public void CreateTexture()
    {
        Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, 0, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        MeshRenderer[] allChildren = pointContainer.GetComponentsInChildren<MeshRenderer>();
        Color[] positionColors = new Color[(texSize * texSize)];

        Debug.Log(allChildren.Length);

        for (int i = 0; i < positionColors.Length; i++)
        {
            Vector4 pos;
            if (i < allChildren.Length)
            {
                pos = (allChildren[i].gameObject.transform.position / containerSize) * 0.5f + Vector3.one * 0.5f;
                pos.w = 1;
            }
            else
            {
                pos = Vector4.one * 0.5f;
            }
            positionColors[i] = new Color(pos.x, pos.y, pos.z, pos.w);
        }

        tex.SetPixels(0, 0, texSize, texSize, positionColors);
        tex.Apply();

        SaveAsTexture(tex);


        Debug.Log("Generated...");
    }

    void SaveAsTexture(Texture2D tex)
    {
        if (!System.IO.Directory.Exists(saveLocation))
        {
            System.IO.Directory.CreateDirectory(saveLocation);
        }
        byte[] _bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{saveLocation}/{textureName}.png", _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + saveLocation);
        UnityEditor.AssetDatabase.Refresh();

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * containerSize);
    }

}
