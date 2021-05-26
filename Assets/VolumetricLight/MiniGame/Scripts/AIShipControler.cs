using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShipControler : MonoBehaviour
{
    public Rigidbody rigidbody;
    [SerializeField] float moveSpeed, rudderEffect, shipSidewayDarg, shipDiversionSlip, rudderMaxAngle;
    [SerializeField] Vector3 rudderPoint;
    Vector2 drivingValue = Vector2.zero;
    Vector3 steerForce;
    bool onBreak;

    public void SteerVesal(float h, float v){
        drivingValue.x = Mathf.Clamp(h, -1, 1);
        drivingValue.y = Mathf.Clamp(v, -1, 1);
    }

    public void Break(){
        drivingValue = Vector2.zero;
        onBreak = true;
    }
    public void Unbreak(){
        drivingValue = Vector2.zero;
        onBreak = true;
    }

    private void FixedUpdate() {
        if(drivingValue.y != 0){
            rigidbody.AddForce(transform.forward * moveSpeed * drivingValue.y, ForceMode.Force);
        }
        
        Quaternion rudderRot = Quaternion.Euler(0, -drivingValue.x * rudderMaxAngle,0);
        Vector3 rudderRight = transform.rotation * rudderRot * Vector3.right;
        steerForce = -rudderRight * Vector3.Dot(rigidbody.GetPointVelocity(transform.position + transform.rotation * rudderPoint), rudderRight);
        rigidbody.AddForceAtPosition(steerForce * rudderEffect, transform.position + transform.rotation * rudderPoint, ForceMode.Force);
        float sidewayResistance = Vector3.Dot(rigidbody.velocity, transform.right);
        float velocityDirection = Vector3.Dot(rigidbody.velocity, transform.forward);
        if(!onBreak){
            rigidbody.AddForce((transform.right * -sidewayResistance * shipSidewayDarg) + transform.forward * Mathf.Sign(velocityDirection) * Mathf.Abs(sidewayResistance) * shipDiversionSlip, ForceMode.Force);
        }
        else{
            rigidbody.AddForce( -transform.forward * velocityDirection * 0.5f, ForceMode.Force);
        }
    }
      private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Vector3 tempVec3 = transform.position + transform.rotation * rudderPoint;
            Gizmos.DrawSphere(tempVec3, 0.05f);
        }
    }


