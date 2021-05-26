using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SmokeSimulationGame : MonoBehaviour
{
    public Texture2D initialMask, testDisolveBlocks;
    public float rate;
    public Vector2 dyeTextureSize, velocityTextureSize;
    public RenderTexture dyeTexture, dyeResultTexture;
    public RenderTexture velocityTexture, velocityResultTexture;
    public RenderTexture moveBlockTexture, moveBlockMaskTexture, moveBlockReciveTexture;
    public Material displayMaterial, dyeSimulator, velSimulator, drawNormal, drawDye, blockMask, blockCtrl, blockSpawn, blockDisolve, alphaShift, pixelValueReader;
    public Texture2D gradientRamp, colorRamp;

    public float nextUpdate, addBlockUpdate;

    public Texture2D readSheet;

    public bool simulate, spawn;

    public UnityEvent gameOverEvent;

    public ImageHolder imageLibrary;
    public RawImage screenImage;

    void Start()
    {

        // make an applay texture
        readSheet = new Texture2D(128, 1, TextureFormat.RGB24, false);
        readSheet.wrapMode = TextureWrapMode.Clamp;
        readSheet.anisoLevel = 0;
        readSheet.filterMode = FilterMode.Point;

        

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

        // enemy color texture
        moveBlockTexture = new RenderTexture((int)velocityTextureSize.x, (int)velocityTextureSize.y, 0);
        moveBlockTexture.wrapMode = TextureWrapMode.Clamp;
        moveBlockTexture.filterMode = FilterMode.Point;

        // enemy flow mask
        moveBlockMaskTexture = new RenderTexture((int)velocityTextureSize.x, (int)velocityTextureSize.y, 0);
        moveBlockMaskTexture.wrapMode = TextureWrapMode.Clamp;
        moveBlockMaskTexture.filterMode = FilterMode.Point;

        // enemy recive texture
        moveBlockReciveTexture = new RenderTexture((int)velocityTextureSize.x, (int)velocityTextureSize.y, 0);
        moveBlockReciveTexture.wrapMode = TextureWrapMode.Clamp;
        moveBlockReciveTexture.filterMode = FilterMode.Point;

        // add initial dye
        //Graphics.Blit(testImage, dyeTexture);
        Graphics.Blit(initialMask, velocityTexture);

        // initializing test blocks
        Graphics.Blit(testDisolveBlocks, moveBlockTexture);
        

        // set velocity texture to display
        //displayMaterial.SetTexture("_MainTex", velocityTexture);


        // set flow texture to due simulator
        dyeSimulator.SetTexture("_FlowMap", velocityTexture);
        drawDye.SetTexture("_FlowTex", velocityTexture);
        displayMaterial.SetTexture("_MainTex", dyeTexture);
        displayMaterial.SetTexture("_BadBlocks", moveBlockTexture);
        velSimulator.SetTexture("_FlowMask", moveBlockMaskTexture);
        blockCtrl.SetTexture("_DencityTex", dyeTexture);
        blockDisolve.SetTexture("_ObjectTex", moveBlockTexture);


        blockCtrl.SetFloat("_Shift", 0);

        simulate = true;

    }

    void Update()
    {
        if (simulate)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                drawDye.SetVector("_Color", (Vector4)Random.ColorHSV(0, 1, 0.8f, 1));
            }

            if (Input.GetAxis("Fire1") > 0) DrawPointNormal();

            // simulate fluid physics
            if (Time.time > addBlockUpdate && spawn)
            {
                Spawn();
                addBlockUpdate = Time.time + 1;
            }

            // simulate fluid physics
            if (Time.time > nextUpdate)
            {
                Simulate();
                nextUpdate = Time.time + 1 / rate;
            }
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

        // simulate Move Block
        Graphics.Blit(moveBlockTexture, moveBlockReciveTexture, blockCtrl, 0);
        Graphics.Blit(dyeTexture, dyeResultTexture, blockDisolve, 0);
        Graphics.Blit(dyeResultTexture, dyeTexture);


        Graphics.Blit(moveBlockReciveTexture, moveBlockMaskTexture, blockMask, 0);
        

        Graphics.Blit(moveBlockReciveTexture, moveBlockTexture);

        StartCoroutine(CheckPieceReach());
        // simulate Move Block
        //   Graphics.Blit(moveBlockMaskTexture, moveBlockTexture);
    }

    void Spawn()
    {
        blockSpawn.SetTexture("_SpwanTex", imageLibrary.GetImage());
        blockSpawn.SetFloat("_xPos", (float)Random.Range(0, 800) / 999.99f);
        Graphics.Blit(moveBlockTexture, moveBlockReciveTexture, blockSpawn, 0);
        Graphics.Blit(moveBlockReciveTexture, moveBlockTexture);
    }

    IEnumerator CheckPieceReach()
    {
        Rect readRect = new Rect(0,127, 128, 1);

        
        RenderTexture.active = moveBlockMaskTexture;

        readSheet.ReadPixels(readRect, 0, 0);
        yield return new WaitForEndOfFrame();
        readSheet.Apply();
        

        RenderTexture.active = null;
        
        Color[] pixelColors = readSheet.GetPixels(0, 0, 128, 1);


        for (int i = 0; i < pixelColors.Length; i++)
        {
            Color value = pixelColors[i];         
            if (value.r < 1)
            {
                GameOver();
                break;
            }
            
        }
        
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

    void GameOver()
    {
        simulate = false;
        spawn = false;
        gameOverEvent.Invoke();
        Debug.Log("Game Over");
    }

    public void StartGame()
    {
        simulate = true;
        blockCtrl.SetFloat("_Shift", 0.004f);
        StartCoroutine(StartSpawning());
    }

    public void RestartGame()
    {
        simulate = true;
        alphaShift.SetFloat("_AlphaValue", 0.0f);
        Graphics.Blit(moveBlockReciveTexture, moveBlockTexture, alphaShift);

        StartCoroutine(StartSpawning());
    }

    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(5);
        spawn = true;
    }

}
