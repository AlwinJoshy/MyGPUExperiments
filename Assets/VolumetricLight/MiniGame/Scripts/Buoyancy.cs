using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{   
    [SerializeField] float maxForce, depthTensionGradient;
    [SerializeField] Vector3[] submerssionTestPoints;
    [SerializeField] Rigidbody rigidbody;
    Vector3 wPos, surfacePoint, tempVec3;
    public float strainOnWaterEffect;
    

    void FixedUpdate()
    {
        strainOnWaterEffect = 0;
        for (int i = 0; i < submerssionTestPoints.Length; i++)
        {
            wPos = transform.position + transform.rotation * submerssionTestPoints[i];
            surfacePoint = SeaWaveProvider.instance.WavePointHeight(wPos);
            if(wPos.y < surfacePoint.y){
                float strainEffect = Mathf.Min((surfacePoint.y - wPos.y)/depthTensionGradient, 1);
                strainOnWaterEffect += strainEffect;
                rigidbody.AddForceAtPosition(Vector3.up * maxForce * strainEffect, wPos, ForceMode.Force);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if(submerssionTestPoints?.Length > 0){
            Gizmos.color = Color.red;
            for (int i = 0; i < submerssionTestPoints.Length; i++)
            {
                tempVec3 = transform.position + transform.rotation * submerssionTestPoints[i];
                Gizmos.DrawSphere(tempVec3, 0.05f);
            }
        }
    }

}
