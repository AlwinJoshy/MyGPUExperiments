using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGAmeLookArCamera : MonoBehaviour
{
    public float lookUpDateRate;
    float nextUpdateTime;
    Transform mainCamera;

    void OnEnable() {
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if(Time.time > nextUpdateTime){
            transform.LookAt(mainCamera);
            nextUpdateTime = Time.time + 1/lookUpDateRate;
        }
    }
}
