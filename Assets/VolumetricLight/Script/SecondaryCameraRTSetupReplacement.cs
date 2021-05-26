using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryCameraRTSetupReplacement : MonoBehaviour
{
    [SerializeField] Shader replacementShader;
    [SerializeField] string replacementKey;
    [SerializeField] RenderTexture secondCameraRTmain;
    [SerializeField] Vector2Int rtResolution;
    [SerializeField] Color clearColor;
    Camera cam;

    void Start(){
        cam = GetComponent<Camera>();
        cam.backgroundColor = clearColor;
        cam.SetReplacementShader(replacementShader, replacementKey);
        // set base clear color
        Shader.SetGlobalColor("_ClearColor", clearColor);
        secondCameraRTmain = new RenderTexture(rtResolution.x, rtResolution.y, 8, RenderTextureFormat.DefaultHDR);
        secondCameraRTmain.antiAliasing = 2;
        cam.targetTexture = secondCameraRTmain;

        Shader.SetGlobalVector("SHADOW_CAMERA_PROJPARAMS", new Vector4(1, cam.nearClipPlane, cam.farClipPlane, 1/cam.farClipPlane));
    }

    private void OnDisable() {
        cam.ResetReplacementShader();
    }

    private void OnPostRender() {
        Shader.SetGlobalVector("SHADOW_CAMERA_FORWARD", transform.forward);
        Shader.SetGlobalVector("SHADOW_CAMERA_POSITION", transform.position);
        Shader.SetGlobalTexture("SHADOW_CAMERA_TOTAL", secondCameraRTmain);
        Shader.SetGlobalMatrix("SHADOW_CAMERA_MATRIX", cam.projectionMatrix * cam.worldToCameraMatrix);
    }
}
