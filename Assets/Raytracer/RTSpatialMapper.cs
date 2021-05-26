using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds{
    public Vector3 position;
    public float boundSize;

    public Bounds(Vector3 pos, float extend){
        position = pos;
        boundSize = extend;
    }
}

public class RenderObject{
    public Bounds bounds;
    public int type;
    public Vector3 glow;
    public RenderObject(Bounds bounds, int type, Vector3 glow){
        this.bounds = bounds;
        this.type = type;
        this.glow = glow;
    }
}

    public struct RTRendererObject
    {
        public Vector4 data;
        public Vector3 glow;
        public int type; // 0 normal, 1 reflective, 2 refractive
    }

    [System.Serializable]
    public struct RenderStrip
    {
        public int startIndex;
        public int count;
    }

    public struct RenderStripInternal
    {
        public int startIndex;
        public int count;
    }

    public struct GPUOctBlock
    {
        public Vector3 origin;
        public float size;
        public int id;
        public int renderID;
   //     public int id;
    }

public class RTSpatialMapper : MonoBehaviour
{
    public static RTSpatialMapper instance;
    public static int leadIndex;
    public static int renderListIndex;
    public float boxSize;
    public static List<RenderObject> sceneCastObjects = new List<RenderObject>();
  /// OctBox rootBox = new OctBox();
    public OctBoxBlock[] octBlockArray = new OctBoxBlock[512];
    public GPUOctBlock[] gpuOctBlockData = new GPUOctBlock[512];
    public RTRendererObject[] gpuRTRenderer = new RTRendererObject[64];
    public RenderStrip[] gpuRenderStrip = new RenderStrip[64];
    public RenderStripInternal[] renderStripInternal = new RenderStripInternal[64];
    public int[] renderIDHolder = new int[4 * 100];
    bool phaser;


    public static Queue<OctBoxBlock> freeBlocks = new Queue<OctBoxBlock>();
    public static Queue<OctBoxBlock> usedBlocks = new Queue<OctBoxBlock>();
    public OctBoxBlock debugSelectedBlock;
    OctBoxBlock tempOctane;
    Vector3 tempVec3 = Vector3.zero;
    Vector4 tempVec4 = Vector4.zero;

    public class OctBoxBlock{
        public bool inUse;
        public Bounds bounds;
        public bool haveChild;
        public int index;
        public int childBoxIndex;
        public int renderObjectID;

        public void Init(int index, float size, Vector3 pos){
            this.index = index;
            bounds = new Bounds(pos, size);
            inUse = true;
        }
        // the recursive function that generates and adds the node blocks in children 
        // while checking
        public void Add(int renderObject, int level){

            if(RTSpatialMapper.AABBCheck(bounds, RTSpatialMapper.sceneCastObjects[renderObject].bounds)){
            // create child nodes
            // if there are no children
                if(level < 4){
                    if(!haveChild){
                        float boundSize = bounds.boundSize * 0.5f;
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 1] = RTSpatialMapper.GetBlock(leadIndex + 1, boundSize, bounds.position);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 2] = RTSpatialMapper.GetBlock(leadIndex + 2, boundSize, bounds.position + Vector3.right * boundSize);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 3] = RTSpatialMapper.GetBlock(leadIndex + 3, boundSize, bounds.position + Vector3.forward * boundSize);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 4] = RTSpatialMapper.GetBlock(leadIndex + 4, boundSize, bounds.position + Vector3.up * boundSize);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 5] = RTSpatialMapper.GetBlock(leadIndex + 5, boundSize, bounds.position + (Vector3.right + Vector3.forward) * boundSize);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 6] = RTSpatialMapper.GetBlock(leadIndex + 6, boundSize, bounds.position + (Vector3.right + Vector3.up) * boundSize);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 7] = RTSpatialMapper.GetBlock(leadIndex + 7, boundSize, bounds.position + (Vector3.forward + Vector3.up) * boundSize);
                        RTSpatialMapper.instance.octBlockArray[leadIndex + 8] = RTSpatialMapper.GetBlock(leadIndex + 8, boundSize, bounds.position + Vector3.one * boundSize);
                        childBoxIndex = leadIndex + 1;
                        leadIndex += 8;
                        haveChild = true;
                    }

                    int subLevel = level + 1;
                    // go through child nodes and check if it is inside the 
                    for (int i = 0; i < 8; i++)
                    {
                        if(RTSpatialMapper.AABBCheck(RTSpatialMapper.instance.octBlockArray[childBoxIndex + i].bounds, RTSpatialMapper.sceneCastObjects[renderObject].bounds)){
                            RTSpatialMapper.instance.octBlockArray[childBoxIndex + i].Add(renderObject, subLevel);
                        }
                    }
                }
                else{
                    if(renderObjectID < 0){
                        renderObjectID = RTSpatialMapper.renderListIndex;
                        RTSpatialMapper.instance.renderStripInternal[renderObjectID] = new RenderStripInternal(){startIndex = (renderObjectID * 4)};
                        RTSpatialMapper.instance.renderIDHolder[renderObjectID * 4] = renderObject;
                        RTSpatialMapper.instance.renderStripInternal[renderObjectID].count++;
                        RTSpatialMapper.renderListIndex += 1;
                    }
                    else{
                            RTSpatialMapper.instance.renderIDHolder[RTSpatialMapper.instance.renderStripInternal[renderObjectID].startIndex + RTSpatialMapper.instance.renderStripInternal[renderObjectID].count] = renderObject;
                            RTSpatialMapper.instance.renderStripInternal[renderObjectID].count++;
                    }
                    
                }
            }
        }

        public void Wash(){
            haveChild = false;
            index = -1;
            inUse = false;
            childBoxIndex = -1;
            renderObjectID = -1;
        }


    }
/*
        public class EndBlock : OctBoxBlock{
        public int objectCount;
        public int[] objectIndex;

        
    }
*/

    public static OctBoxBlock GetBlock(int index, float size, Vector3 pos){
        OctBoxBlock respBlock = null;

        // get an oct block from collection
        if(freeBlocks.Count > 0){
            respBlock = freeBlocks.Peek();
            freeBlocks.Dequeue();
            usedBlocks.Enqueue(respBlock);
        }
        // make an oct block
        else{
            respBlock = new OctBoxBlock();
            usedBlocks.Enqueue(respBlock);
        }

        respBlock.Init(index, size, pos);

        return respBlock;
    }

    void Prepare(){
        leadIndex = 0;
        renderListIndex = 0;

        for (int i = 0; i < 80; i++)
        {
            renderIDHolder[i] = -1000;
        }


        while (usedBlocks.Count > 0)
        {
            tempOctane = usedBlocks.Peek();
            tempOctane.Wash();
            freeBlocks.Enqueue(tempOctane);
            usedBlocks.Dequeue();
        }

        octBlockArray[0] = RTSpatialMapper.GetBlock(0, boxSize, Vector3.zero); 

       

        for (int i = 0; i < sceneCastObjects.Count; i++)
        {
            octBlockArray[0].Add(i, 1);
        }
        // convert quadtree data into transferable buffer
        for (int i = 0; i <= leadIndex; i++)
        {
            gpuOctBlockData[i] = new GPUOctBlock(){origin = octBlockArray[i].bounds.position, id = octBlockArray[i].childBoxIndex, size = octBlockArray[i].bounds.boundSize, renderID = octBlockArray[i].renderObjectID};
        }

        for (int i = 0; i < sceneCastObjects.Count; i++)
        {
            tempVec4 = sceneCastObjects[i].bounds.position + Vector3.one * 0.5f * sceneCastObjects[i].bounds.boundSize;
            tempVec4.w = 0.22f;
            tempVec3 = Vector3.zero;
            gpuRTRenderer[i] = new RTRendererObject(){data = tempVec4, glow = sceneCastObjects[i].glow, type = sceneCastObjects[i].type};
        }
       
        for (int i = 0; i < renderListIndex; i++)
        {
            gpuRenderStrip[i] = new RenderStrip(){count = renderStripInternal[i].count, startIndex = renderStripInternal[i].startIndex};
        }

        phaser = true;
    }
/*
    static bool AABBCheck(Bounds boxA, Bounds boxB){
        return(
            boxA.position.x < boxB.position.x &&
            boxA.position.x + boxA.boundSize >  boxB.position.x &&
            boxA.position.y < boxB.position.y &&
            boxA.position.y + boxA.boundSize >  boxB.position.y &&
            boxA.position.z < boxB.position.z &&
            boxA.position.z + boxA.boundSize >  boxB.position.z
        );
    }
*/

    static bool AABBCheck(Bounds boxA, Bounds boxB){
        return(
            boxA.position.x < boxB.position.x + boxB.boundSize &&
            boxA.position.x + boxA.boundSize >  boxB.position.x &&
            boxA.position.y < boxB.position.y + boxB.boundSize &&
            boxA.position.y + boxA.boundSize >  boxB.position.y &&
            boxA.position.z < boxB.position.z + boxB.boundSize &&
            boxA.position.z + boxA.boundSize >  boxB.position.z
        );
    }

    private void Awake() {
        instance = this;
        gpuOctBlockData = new GPUOctBlock[256];
    }

    private void Start() {
        
    }


    int currentIndex = 0;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            currentIndex++;
            debugSelectedBlock = octBlockArray[currentIndex];
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)){
            currentIndex--;
            currentIndex = currentIndex < 0 ? 0 : currentIndex;
            debugSelectedBlock = octBlockArray[currentIndex];
        }
        if(!phaser)Prepare();
        else phaser = false;
    }

    private void OnDrawGizmos(){
        if(octBlockArray != null){
            Gizmos.color = Color.gray;
            for (int i = 0; i < octBlockArray.Length; i++)
            {
                if(octBlockArray[i] != null && octBlockArray[i].inUse){
                    Gizmos.DrawWireCube(octBlockArray[i].bounds.position + Vector3.one * octBlockArray[i].bounds.boundSize * 0.5f, Vector3.one * octBlockArray[i].bounds.boundSize);
                }
            }
        }
        if(debugSelectedBlock != null){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(debugSelectedBlock.bounds.position + Vector3.one * debugSelectedBlock.bounds.boundSize * 0.5f, Vector3.one * debugSelectedBlock.bounds.boundSize);
        }
    }

}
