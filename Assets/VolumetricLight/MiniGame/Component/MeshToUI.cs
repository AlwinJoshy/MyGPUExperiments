using UnityEngine;
using UnityEngine.UI;

public class MeshToUI : Graphic
{
    public float meshScale;
    public Mesh mesh;
    Vector3 vertVec3;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if(mesh != null)
        
            vh.Clear();

            UIVertex vert = UIVertex.simpleVert;

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                vertVec3 = mesh.vertices[i];
                vert.position = new Vector2(vertVec3.x, vertVec3.y);
                vert.position *= meshScale;
                vert.color = Color.white;
                vert.uv0 = mesh.uv[i];
                vh.AddVert(vert);
            }
    
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                vh.AddTriangle(mesh.triangles[i], mesh.triangles[i + 1], mesh.triangles[i + 2]);
            }
        }
}
