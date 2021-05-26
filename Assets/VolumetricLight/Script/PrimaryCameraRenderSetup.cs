using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryCameraRenderSetup : MonoBehaviour
{
    [SerializeField] Material renderMaterial;
    Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }

    private void OnPostRender() {
         // Draw Screen Quad
        DrawGLQuad();
    }

    void DrawGLQuad(){
        GL.PushMatrix();
        renderMaterial.SetPass(0);
      //  renderMaterial.SetMatrix("MAIN_CAMERA_INVERTION_MATRIX", cameraProjectionInverterMtx);
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0,0);
        GL.Vertex3(0,0,0);
        GL.TexCoord2(0,1);
        GL.Vertex3(0,1,0);
        GL.TexCoord2(1,1);
        GL.Vertex3(1,1,0);
        GL.TexCoord2(1,0);
        GL.Vertex3(1,0,0);
        GL.End();
        GL.PopMatrix();

         GL.PushMatrix();
        renderMaterial.SetPass(1);
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0,0);
        GL.Vertex3(0,0,0);
        GL.TexCoord2(0,1);
        GL.Vertex3(0,1,0);
        GL.TexCoord2(1,1);
        GL.Vertex3(1,1,0);
        GL.TexCoord2(1,0);
        GL.Vertex3(1,0,0);
        GL.End();
        GL.PopMatrix();
    }

}
