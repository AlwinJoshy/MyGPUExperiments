using UnityEngine;

[ExecuteInEditMode]
public class ExtractVolumeCameraData : MonoBehaviour
{
    Camera thisCamera;
    public Material volumeMaterial;


    void Start()
    {
        thisCamera = GetComponent<Camera>();
        thisCamera.depthTextureMode = DepthTextureMode.Depth;
        
    }

    // Update is called once per frame
    void Update()
    {
        volumeMaterial.SetFloat("_fov", thisCamera.fieldOfView * Mathf.Deg2Rad);
        volumeMaterial.SetMatrix("_WorldToVolumeCamera", thisCamera.worldToCameraMatrix);
    }
}
