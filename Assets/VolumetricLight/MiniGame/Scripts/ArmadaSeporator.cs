using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadaSeporator : MonoBehaviour
{
    static public ArmadaSeporator instance;
    [SerializeField] [Range(0, 5)] float spacing;
    [SerializeField] [Range(0, 2)] float seporationControle;
    List<Transform> allVessels = new List<Transform>();
    Vector3 pushVector, dir;
    float magValue, compareMag;
    void Awake()
    {
        instance = this;
    }

    public void AddVessel(Transform vessel){
        allVessels.Add(vessel);
    }

    public void RemoveVessel(Transform vessel){
        allVessels.Remove(vessel);
    }

    public Vector3 AvoidanceVector(Vector3 pos){
        pushVector = Vector3.zero;
        compareMag = Mathf.Pow(spacing, 2);
        for (int i = 0; i < allVessels.Count; i++)
        {
            dir = pos - allVessels[i].position;
            magValue = dir.sqrMagnitude;

            if(magValue < compareMag){
                pushVector += dir * ((compareMag - magValue)/compareMag) * seporationControle;
            }
        }
        return pushVector;
    }


}
