using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCameraLoc : MonoBehaviour
{

    public static MiniGameCameraLoc instance;
    public float followSpeed = 3, maxLength = 1.5f;
    public bool updated;
    Stack<CamLocInfo> locationInfo = new Stack<CamLocInfo>();
    Vector3 dir = Vector3.zero;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (updated)
        {
            if (locationInfo.Peek().isPositionBased)
            {
                dir = locationInfo.Peek().position - transform.position;
                transform.Translate(Vector3.ClampMagnitude(dir * Time.deltaTime * followSpeed, maxLength), Space.World);
                transform.forward = Vector3.Lerp(transform.forward, locationInfo.Peek().target - locationInfo.Peek().position, Time.deltaTime * followSpeed);
            }

            else
            {
                dir = locationInfo.Peek().transformTemplate.position - transform.position;
                transform.Translate(Vector3.ClampMagnitude(dir * Time.deltaTime * followSpeed, maxLength), Space.World);
                transform.forward = Vector3.Lerp(transform.forward, locationInfo.Peek().transformTemplate.forward, Time.deltaTime * followSpeed);
            }

        }
    }

    public void SetNewTarget(CamLocInfo newCamPos)
    {
        transform.parent = null;
        locationInfo.Push(newCamPos);
        updated = true;
    }

    public void RemoveTheLastOne()
    {
        locationInfo.Pop();
    }

    public class CamLocInfo
    {
        public bool isPositionBased;
        public Vector3 position, target;
        public Transform transformTemplate;
        public Transform parent;

        public CamLocInfo(Vector3 pos, Transform transData, Transform parent, bool isPositionBased)
        {
            this.isPositionBased = isPositionBased;
            this.position = pos;
            this.target = pos;
            transformTemplate = transData;
            this.parent = parent;
        }
    }

}
