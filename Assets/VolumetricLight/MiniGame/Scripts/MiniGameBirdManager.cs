using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MiniGameBirdManager : MonoBehaviour
{

    [SerializeField] Transform[] pointOfIntrests;
    [SerializeField] const int birdCount = 50, skipBatchSize = 4;
    [SerializeField] GameObject birdObject;
    [SerializeField] float birdSpeed, heightCorrectionEffect, pointOfIntrestEffect, wanderStrength, wanderRadius;
    [SerializeField] float yMax, yMin;
    [SerializeField] Vector3 planeSize;

    BirdBoid[] allBirds;
    Vector3 tempVec3;
    int countStart = 0;
    float nextPointOfIntrestTime;
    Transform currentPointOfIntrest;



    void Start()
    {
        tempVec3 = Vector3.zero;
        allBirds = new BirdBoid[birdCount];
        GetPOI();
        for (int i = 0; i < birdCount; i++)
        {
            allBirds[i] = new BirdBoid();
            allBirds[i].birdMesh = Instantiate(birdObject, GetRandomPointInVolume(), Quaternion.identity, transform).transform;
            allBirds[i].velocity = Vector3.right;
        }
    }

    void GetPOI()
    {
        currentPointOfIntrest = pointOfIntrests[Random.Range(0, pointOfIntrests.Length)];
        nextPointOfIntrestTime = Time.time + (float)Random.Range(5, 20);
    }

    void Update()
    {
        if (nextPointOfIntrestTime < Time.time)
        {
            GetPOI();
        }

        if (allBirds != null)
        {
            for (int i = countStart; i < birdCount; i += skipBatchSize)
            {
                tempVec3 = (allBirds[i].GetWanderDir(ref tempVec3, wanderRadius) + (allBirds[i].birdMesh.forward * 2));
                allBirds[i].velocity += (tempVec3 * wanderStrength + Vector3.up * ((transform.position.y + (planeSize.y * 0.5f))
                 - allBirds[i].birdMesh.position.y) * heightCorrectionEffect +
                 (currentPointOfIntrest.position - allBirds[i].birdMesh.transform.position).normalized * pointOfIntrestEffect) * Time.deltaTime;
                allBirds[i].velocity = Vector3.ClampMagnitude(allBirds[i].velocity, birdSpeed);
                allBirds[i].birdMesh.forward = allBirds[i].velocity;
                allBirds[i].birdMesh.Translate(allBirds[i].velocity * Time.deltaTime, Space.World);
                ManageContainMent(allBirds[i].birdMesh);
            }
            countStart++;
            if (countStart > skipBatchSize) countStart = 0;
        }
    }

    public class BirdBoid
    {
        public Transform birdMesh;
        public Vector3 velocity = Vector3.right;
        public float speed = 1;

        public Vector3 GetWanderDir(ref Vector3 outVector, float wanderRadius)
        {
            outVector.x = (float)Random.Range(-100, 100) / 100;
            outVector.y = (float)Random.Range(-100, 100) / 100;
            outVector.z = (float)Random.Range(-100, 100) / 100;
            outVector.Normalize();
            outVector *= wanderRadius;

            return outVector;
        }
    }

    Vector3 GetRandomPointInVolume()
    {
        tempVec3.x = transform.position.x + Random.Range(-planeSize.x, planeSize.x) * 0.5f;
        tempVec3.y = transform.position.y + planeSize.y * (float)Random.Range(0, 100) / 100;
        tempVec3.z = transform.position.z + Random.Range(-planeSize.z, planeSize.z) * 0.5f;
        return tempVec3;
    }

    void ManageContainMent(Transform boid)
    {
        tempVec3 = boid.position;
        if (boid.position.x > transform.position.x + planeSize.x * 0.5f ||
        boid.position.x < transform.position.x - planeSize.x * 0.5f) tempVec3.x += (transform.position.x - boid.position.x) * 0.99f;
        if (boid.position.z > transform.position.z + planeSize.z * 0.5f ||
        boid.position.z < transform.position.z - planeSize.z * 0.5f) tempVec3.z += (transform.position.z - boid.position.z) * 0.99f;
        boid.position = tempVec3;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        tempVec3 = planeSize;
        tempVec3.y = 0.01f;
        Gizmos.DrawCube(transform.position, tempVec3);

        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up * planeSize.y, tempVec3);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * planeSize.y);
    }

}
