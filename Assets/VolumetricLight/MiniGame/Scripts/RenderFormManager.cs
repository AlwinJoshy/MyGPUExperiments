using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderFormManager : MonoBehaviour
{
    static public RenderFormManager instance;
    [SerializeField] CameraEvent drawAt;
    [SerializeField] Camera cam;
    [SerializeField] Material foamMaterial;
    CommandBuffer _Foam_Buffer;
    Renderer rendererR;
    List<Renderer> foamRenderer = new List<Renderer>();
    Dictionary<Camera, CommandBuffer> cammandBufferDict = new Dictionary<Camera, CommandBuffer>();
    bool initialized;
    private int _globalDistortionTexID;

    private void Awake()
    {
        instance = this;
    }

    void Cleanup()
    {
        foreach (var cam in cammandBufferDict)
        {
            if (cam.Key) cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, _Foam_Buffer);
        }
        cammandBufferDict.Clear();
    }

    private void OnEnable()
    {
        Cleanup();
    }

    private void OnDisable()
    {
        Cleanup();
    }

    public void AddRenderer(Renderer r)
    {
        if (initialized && !foamRenderer.Contains(r)) _Foam_Buffer.DrawRenderer(r, foamMaterial);
        if (!foamRenderer.Contains(r)) foamRenderer.Add(r);
    }

    public void RemoveRenderer(Renderer r)
    {
        foamRenderer.Remove(r);
    }

    private void OnWillRenderObject()
    {
        bool isObjectActive = gameObject.activeInHierarchy;

        if (!isObjectActive)
        {
            Cleanup();
            return;
        }

        Camera cameraRef = Camera.current;

        if (!cameraRef.gameObject.CompareTag("MainCamera")) return;

        if (!cameraRef) return;

        if (cammandBufferDict.ContainsKey(cameraRef)) return;

        Debug.Log("Draw to buffer");

        // creating a command buffer
        _Foam_Buffer = new CommandBuffer();
        _Foam_Buffer.name = "FOAM_PARTICLES";
        cammandBufferDict[cameraRef] = _Foam_Buffer;

        // creating texture and assigning
        int texID = Shader.PropertyToID("_FoamTexture");
        _Foam_Buffer.GetTemporaryRT(texID, cam.pixelWidth / 3, cam.pixelHeight / 3, 24, FilterMode.Bilinear);
        _Foam_Buffer.SetRenderTarget(texID);
        _Foam_Buffer.ClearRenderTarget(true, true, Color.black);

        // draw all renderers
        foreach (Renderer foamSrc in foamRenderer)
        {
            _Foam_Buffer.DrawRenderer(foamSrc, foamMaterial);
        }

        _Foam_Buffer.SetGlobalTexture("_FoamTex", texID);
        cam.AddCommandBuffer(drawAt, _Foam_Buffer);

        initialized = true;
    }

}
