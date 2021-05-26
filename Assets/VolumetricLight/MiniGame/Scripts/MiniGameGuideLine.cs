using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniGameGuideLine : MonoBehaviour
{

    [SerializeField] float stepSize = 1f, moveSpeed, t, maxStepLimit;
    [SerializeField] ParticleSystem guidanceParticle;
    [SerializeField] bool loop = true;
    Vector3 targetPos, tempDir;
    MiniGameCurveGen.Curve newCurve;
    NavMeshPath navMeshPath;
    NavMeshHit navMeshHit;
    bool run;

    void Start()
    {
        navMeshPath = new NavMeshPath();
        newCurve = new MiniGameCurveGen.Curve();
    }


    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            tempDir = targetPos - transform.position;
            float forwardProjection = Vector3.Dot(tempDir, transform.forward);

            if (t > maxStepLimit && forwardProjection < 0)
            {
                if (loop)
                {
                   SetAt();
                }
            }

            if (forwardProjection < 0)
            {
                AddT(stepSize * Time.deltaTime);
                targetPos = newCurve.GetPositionAtT(t);
                transform.forward = targetPos - transform.position;
            }
            else
            {
                transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
                // transform.Translate((transform.forward + tempDir * forwardProjection * 0.01f) * moveSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    void AddT(float value){
        t += value;
        t = t > 1 ? 1 : t;
    }

    public void Stop()
    {
        run = false;
        guidanceParticle.Stop();
    }

    void SetAt()
    {
        guidanceParticle.Stop();
        targetPos = newCurve.GetPositionAtT(0.02f);
        transform.position = newCurve.GetPositionAtT(0);
        transform.forward = targetPos - transform.position;
        guidanceParticle.Play();
        run = true;
        t = 0;
    }

    public void Setup(Vector3 startPoint, Vector3 endPoint)
    {
        t = 0;
        NavMesh.SamplePosition(startPoint, out navMeshHit, 2, NavMesh.AllAreas);
        Vector3 naveMeshStartPoint = navMeshHit.position;
        NavMesh.SamplePosition(endPoint, out navMeshHit, 2, NavMesh.AllAreas);
        Vector3 naveMeshEndPoint = navMeshHit.position;

        if (NavMesh.CalculatePath(naveMeshStartPoint, naveMeshEndPoint, NavMesh.AllAreas, navMeshPath))
        {
            run = true;
            newCurve.pointList = navMeshPath.corners;
            for (int i = 0; i < newCurve.pointList.Length; i++)
            {
                newCurve.pointList[i] += Vector3.up * ((float)Random.Range(1, 100) / 100) * 2;
            }
            SetAt();
        }
        else
        {
            Debug.Log("Cant generate path...");
        }
    }

}
