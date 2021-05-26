using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SeaWaveProvider : MonoBehaviour
{
    static public SeaWaveProvider instance;
    [SerializeField] [Range(-1, 4)] float waveSpeed, waveAmplitude, waveFrequency, ringRadius;
    [SerializeField] float ringUpdateRate;
    [SerializeField] Material seaWaterMat;
    Vector2 offsetTemp;
    Vector4 transMissionVector = Vector4.zero, tempVec4;
    float prevLength = 0, nextRingUpdate;

    public List<Transform> vessalObjects = new List<Transform>();
    public List<Vector4> ringArray = new List<Vector4>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        seaWaterMat.SetVectorArray("_DangerRingArray", ringArray);
        seaWaterMat.SetInt("_DangerRingCount", 0);
    }

    public Vector3 WavePointHeight(Vector3 pos)
    {
        offsetTemp.x = Mathf.Cos(pos.z * waveFrequency + Time.time * waveSpeed) * waveAmplitude;
        offsetTemp.y = Mathf.Sin(pos.z * waveFrequency + Time.time * waveSpeed) * waveAmplitude;
        pos.y = transform.position.y;
        return pos + Vector3.up * Vector2.Dot(Vector2.up, offsetTemp);
    }

    public void AddMarkerRing(Transform vesselObj)
    {
        if (!vessalObjects.Contains(vesselObj)) vessalObjects.Add(vesselObj);
    }

    public void RemoveMarkerRing(Transform vesselObj)
    {
        vessalObjects.Remove(vesselObj);
        if (vessalObjects.Count == 0)
        {
            seaWaterMat.SetVectorArray("_DangerRingArray", ringArray);
            seaWaterMat.SetInt("_DangerRingCount", vessalObjects.Count);
        }
    }

    private void Update()
    {
        transMissionVector.x = waveSpeed;
        transMissionVector.y = waveAmplitude;
        transMissionVector.z = waveFrequency;
        transMissionVector.w = transform.position.y;

        float vecMag = transMissionVector.SqrMagnitude();

        if (!Mathf.Approximately(vecMag, prevLength))
        {
            prevLength = vecMag;
            seaWaterMat.SetVector("_WaveParams", transMissionVector);
        }

        if (Time.time > nextRingUpdate && vessalObjects.Count > 0)
        {
            for (int i = 0; i < vessalObjects.Count; i++)
            {
                if (ringArray.Count < vessalObjects.Count) ringArray.Add(Vector3.zero);
                tempVec4 = vessalObjects[i].transform.position;
                tempVec4.w = ringRadius;
                ringArray[i] = tempVec4;
            }
            seaWaterMat.SetVectorArray("_DangerRingArray", ringArray.ToArray());
            seaWaterMat.SetInt("_DangerRingCount", vessalObjects.Count);
            nextRingUpdate = Time.time + (1 / ringUpdateRate);
        }
    }

}
