using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "imageLibrary", menuName = "myObjects")]
public class ImageHolder : ScriptableObject
{
    public Texture2D[] allImages;

    public Texture2D GetImage()
    {
        return allImages[Random.Range(0, allImages.Length)];
    }

}
