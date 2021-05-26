using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMainSample : MonoBehaviour
{
    [SerializeField] ComputeShader RTXShader;
    [SerializeField] Texture skyBox;

    public GPUOctBlock[] octblockData;
    ComputeBuffer octBlockBuffer;
    ComputeBuffer renderObjectBuffer;
    ComputeBuffer renderIDHolders;
    ComputeBuffer objectIndexList;

    RenderTexture dispTex;
    Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        RTXShader.SetTexture(0, "_SkyboxTexture", skyBox);
        octBlockBuffer = new ComputeBuffer(512, 24);
        renderObjectBuffer = new ComputeBuffer(64, 4 * 8);
        renderIDHolders = new ComputeBuffer(64, 8);
        objectIndexList = new ComputeBuffer(4 * 100, 4);
    }

    private void SetShaderParameters()
    {
        RTXShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RTXShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        SetShaderParameters();
        Render(dest);
    }

    void Render(RenderTexture dest){
        // initialize the texture
        Init();
        GetBufferData();
        RTXShader.SetTexture(0, "Result", dispTex);
        RTXShader.SetBuffer(0, "octBlockBuffer", octBlockBuffer);
        RTXShader.SetBuffer(0, "RenderList", renderObjectBuffer);
        RTXShader.SetBuffer(0, "RenderStrips", renderIDHolders);
        RTXShader.SetBuffer(0, "arrayData", objectIndexList);

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RTXShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        Graphics.Blit(dispTex, dest);
    }

    private void GetBufferData()
    {
       octBlockBuffer.SetData(RTSpatialMapper.instance.gpuOctBlockData);
       renderObjectBuffer.SetData(RTSpatialMapper.instance.gpuRTRenderer);
       renderIDHolders.SetData(RTSpatialMapper.instance.gpuRenderStrip);
       objectIndexList.SetData(RTSpatialMapper.instance.renderIDHolder);
    }

    void Init(){
        if(dispTex == null || dispTex.width != Screen.width || dispTex.height != Screen.height){
            dispTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            dispTex.enableRandomWrite = true;
            dispTex.Create();
        }
    }

}
