using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraEnableDepthNormal : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    void Update() {
        Shader.SetGlobalMatrix("_VWMatrix", cam.cameraToWorldMatrix);
    }

}
