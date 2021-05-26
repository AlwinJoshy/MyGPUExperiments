using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameColissionManager : MonoBehaviour
{
    public static MiniGameColissionManager instance;
    List<ColliderSphere> allCOlliders = new List<ColliderSphere>();
    Vector3 temp;

    void Awake()
    {
        instance = this;
    }

    public bool CheckColission(BezierProjectile pointPos){
        for (int i = 0; i < allCOlliders.Count; i++)
        {
            temp = allCOlliders[i].pos.position - pointPos.objBody.position;
            if(temp.sqrMagnitude < Mathf.Pow(allCOlliders[i].r, 2) && allCOlliders[i].colissionSourceType != pointPos.colissionSourceType){
                allCOlliders[i].ballEnterObj.onBallEnter(pointPos);
                return true;
            }
        }
        return false;
    }

    public void AddCollider(float r, Transform pos, IColissionActions ballEnterObj, ColissionSourceType sourceType){
        allCOlliders.Add(new ColliderSphere(r, pos, ballEnterObj, sourceType));
    }

    public void RemoveCollider(Transform pos){
        for (int i = 0; i < allCOlliders.Count; i++)
        {
            if(allCOlliders[i].pos != pos) continue;
            else{
                allCOlliders.Remove(allCOlliders[i]);
                break;
            }
        }
    }

}

public class ColliderSphere {
    public float r;
    public Transform pos;
    public IColissionActions ballEnterObj;
    public ColissionSourceType colissionSourceType;

    public ColliderSphere(float r, Transform pos, IColissionActions ballEnterObj, ColissionSourceType sourceType){
        this.r = r;
        this.pos = pos;
        this.ballEnterObj = ballEnterObj;
        this.colissionSourceType = sourceType;
    }
}

public enum ColissionSourceType
{
    basic,
    player,
    enemy,
    trader,
}
