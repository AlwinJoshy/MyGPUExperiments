using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    public Transform cameraPoint;
    [SerializeField] float followSpeed, rotationSpeed, depthCameraDist;
    [SerializeField] Transform targetPoint, pivot, depthCamera;
    float currentYRotation;
    Vector3 rotationVector;

    void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        if (MiniGameGlobalRef.BeInControl())
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * followSpeed);
            depthCamera.position = transform.position + depthCamera.forward * -depthCameraDist;
            rotationVector.y = Input.GetAxis("Mouse X");
            pivot.Rotate(rotationVector * Time.deltaTime * rotationSpeed);
            Shader.SetGlobalVector("_AttentionPoint", transform.position);
        }
    }
}
