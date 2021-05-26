using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColissionObject : MonoBehaviour
{
    RenderObject castBox;
    
    [SerializeField]float extend;
    [SerializeField]int type;
    [SerializeField] Color glow;
    
    void Start()
    {
        castBox = new RenderObject(new Bounds(transform.position, extend), type, new Vector3(glow.r, glow.g, glow.b));
        RTSpatialMapper.sceneCastObjects.Add(castBox);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - Vector3.one * 0.5f * extend, 0.05f);
        Gizmos.DrawWireSphere(transform.position - Vector3.one * 0.5f * extend + Vector3.one * extend, 0.05f);
        Gizmos.DrawWireCube(transform.position - Vector3.one * 0.5f * extend + Vector3.one * extend * 0.5f, Vector3.one * extend);
    }

    private void Update() {
        castBox.bounds.position = transform.position - Vector3.one * 0.5f * extend;
        castBox.bounds.boundSize = extend;
    }

}
