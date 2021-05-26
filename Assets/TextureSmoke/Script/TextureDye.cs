using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureDye : MonoBehaviour
{
    public Texture2D testImage;
    public float rate;
    public Vector2 dyeTextureSize, velocityTextureSize;
    public RenderTexture dyeTexture, dyeResultTexture;
    public RenderTexture velocityTexture, velocityResultTexture;
    public Material displayMaterial, dyeSimulator, velSimulator, drawNormal, drawDye;
    public Texture2D gradientRamp, colorRamp;

    public float nextUpdate;

    public float colorShiftUpdate;

    void Start()
    {
        // Dye simulation texture
        dyeTexture = new RenderTexture((int)dyeTextureSize.x, (int)dyeTextureSize.y, 0);
        dyeTexture.wrapMode = TextureWrapMode.Clamp;
        dyeTexture.filterMode = FilterMode.Point;

        dyeResultTexture = new RenderTexture((int)dyeTextureSize.x, (int)dyeTextureSize.y, 0);
        dyeResultTexture.wrapMode = TextureWrapMode.Clamp;
        dyeResultTexture.filterMode = FilterMode.Point;

        // velocity simulation texture
        velocityTexture = new RenderTexture((int)velocityTextureSize.x, (int)velocityTextureSize.y, 0);
        velocityTexture.wrapMode = TextureWrapMode.Clamp;
        velocityTexture.filterMode = FilterMode.Point;

        velocityResultTexture = new RenderTexture((int)velocityTextureSize.x, (int)velocityTextureSize.y, 0);
        velocityResultTexture.wrapMode = TextureWrapMode.Clamp;
        velocityResultTexture.filterMode = FilterMode.Point;

        // add initial dye
        //Graphics.Blit(testImage, dyeTexture);
        Graphics.Blit(testImage, velocityTexture);

        // set velocity texture to display
        //displayMaterial.SetTexture("_MainTex", velocityTexture);


        // set flow texture to due simulator
        dyeSimulator.SetTexture("_FlowMap", velocityTexture);
        drawDye.SetTexture("_FlowTex", velocityTexture);
        displayMaterial.SetTexture("_MainTex", dyeTexture);

    }

    void Update()
    {

        if (Time.time >= colorShiftUpdate)
        {
            drawDye.SetVector("_Color", (Vector4)Random.ColorHSV(0, 1, 0.8f, 1));
            colorShiftUpdate = Time.time + 0.2f;
        }

    if(Input.GetAxis("Fire1") > 0)DrawPointNormal();
    
    // gradient ramp
    else if(Input.GetKeyDown(KeyCode.Alpha1))
    {
        displayMaterial.SetTexture("_ColorTex", gradientRamp);
    }

       // gradient ramp
    else if(Input.GetKeyDown(KeyCode.Alpha2))
    {
        displayMaterial.SetTexture("_ColorTex", colorRamp);
    }


    if(Time.time > nextUpdate)
    {
        Simulate();
        nextUpdate = Time.time + 1/ rate;
    }
       
    }

    void Simulate()
    {
        // velocity simulation
        Graphics.Blit(velocityTexture, velocityResultTexture, velSimulator, 0);
        Graphics.Blit(velocityResultTexture, velocityTexture);

        // Dye simulation
        Graphics.Blit(dyeTexture, dyeResultTexture, dyeSimulator, 0);
        Graphics.Blit(dyeResultTexture,dyeTexture);
    }

    void DrawPointNormal()
    {
        // get the mouse point vector
        Vector3 pointDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;
        // correct length
        float compVec = Vector3.Dot(Camera.main.transform.forward, pointDir);
        pointDir *= 1 + (1 - compVec);
        // turn it into uv
        pointDir = (pointDir + Vector3.one/2);

        // set the uv point
        
        drawNormal.SetVector("_DrawPoint", new Vector2(pointDir.x, pointDir.y));
        // draw normal using the normal draw shader
        Graphics.Blit(velocityTexture, velocityResultTexture, drawNormal, 0);
        Graphics.Blit(velocityResultTexture, velocityTexture);

        drawDye.SetVector("_DrawPoint", new Vector2(pointDir.x, pointDir.y));
        Graphics.Blit(dyeTexture, dyeResultTexture, drawDye, 0);
        Graphics.Blit(dyeResultTexture, dyeTexture);
    }

}
