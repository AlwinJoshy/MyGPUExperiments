using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryCameraRTSetup : MonoBehaviour
{
    [SerializeField] RenderTexture secondCameraRTColor, secondCameraRTDepth;
    [SerializeField] Vector2Int rtResolution;
    Camera cam;

    void Start(){
        cam = GetComponent<Camera>();
        secondCameraRTColor = new RenderTexture(rtResolution.x, rtResolution.y, 16, RenderTextureFormat.Default);
        secondCameraRTDepth = new RenderTexture(rtResolution.x, rtResolution.y, 24, RenderTextureFormat.Depth);
        cam.SetTargetBuffers(secondCameraRTColor.colorBuffer, secondCameraRTDepth.depthBuffer);

        Shader.SetGlobalVector("SHADOW_CAMERA_PROJPARAMS", new Vector4(1, cam.nearClipPlane, cam.farClipPlane, 1/cam.farClipPlane));
    }

    private void OnPostRender() {
        Shader.SetGlobalVector("SHADOW_CAMERA_FORWARD", transform.forward);
        Shader.SetGlobalVector("SHADOW_CAMERA_POSITION", transform.position);
        Shader.SetGlobalTexture("SHADOW_CAMERA_DEPTH", secondCameraRTDepth);
        Shader.SetGlobalMatrix("SHADOW_CAMERA_MATRIX", cam.projectionMatrix * cam.worldToCameraMatrix);
    }
}
