using UnityEngine;

public class ShipController : MonoBehaviour, IColissionActions
{
    [SerializeField] float rudderEffect, shipDiversionSlip, rudderMaxAngle;
    [SerializeField] ShipDataParam speedParam, steerParam;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Vector3 rudderPoint;
    Vector3 steerForce;

    // ship assignment
    private void Awake()
    {
        MiniGameGlobalRef.playerShip = this.transform;
        MiniGameGlobalRef.playerRB = rigidbody;
    }

    private void Start()
    {
        Init();
        GetParams();
    }

    public void GetParams()
    {
        speedParam = MiniGameGlobalRef.playerStat.shipParams.GetAttributeWithName("Speed");
        steerParam = MiniGameGlobalRef.playerStat.shipParams.GetAttributeWithName("Steer");
    }

    void Init()
    {
        MiniGameColissionManager.instance.AddCollider(0.5f, transform, this, ColissionSourceType.player);
    }

    private void FixedUpdate()
    {
        if (MiniGameGlobalRef.BeInControl())
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                rigidbody.AddForce(transform.forward * speedParam.GetValue() * Input.GetAxis("Vertical"), ForceMode.Force);
            }

            Quaternion rudderRot = Quaternion.Euler(0, -Input.GetAxis("Horizontal") * rudderMaxAngle, 0);
            Vector3 rudderRight = transform.rotation * rudderRot * Vector3.right;
            steerForce = -rudderRight * Vector3.Dot(rigidbody.GetPointVelocity(transform.position + transform.rotation * rudderPoint), rudderRight);
            rigidbody.AddForceAtPosition(steerForce * rudderEffect, transform.position + transform.rotation * rudderPoint, ForceMode.Force);
            float sidewayResistance = Vector3.Dot(rigidbody.velocity, transform.right);
            float velocityDirection = Vector3.Dot(rigidbody.velocity, transform.forward);
            rigidbody.AddForce((transform.right * -sidewayResistance * steerParam.GetValue()) + transform.forward * Mathf.Sign(velocityDirection) * Mathf.Abs(sidewayResistance) * shipDiversionSlip, ForceMode.Force);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 tempVec3 = transform.position + transform.rotation * rudderPoint;
        Gizmos.DrawSphere(tempVec3, 0.05f);
    }

    public void onBallEnter(BezierProjectile ball)
    {
        MiniGamePlayerDeathManager.instance.CalculateHitDamage();
    }
}


