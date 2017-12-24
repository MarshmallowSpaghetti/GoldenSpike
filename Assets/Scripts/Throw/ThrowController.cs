using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    [SerializeField]
    Transform firePoint;

    [SerializeField]
    ProjectileArc projectileArc;
    
    private float currentSpeed;
    private float currentAngle;

    private Vector3 targetPoint;

    private void Start()
    {
        //Physics.gravity = new Vector3(0f, -50f, 0f);
        //print("gravity " + Physics.gravity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPoint, 0.2f);
    }

    public void SetTargetWithAngle(Vector3 point, float angle)
    {
        currentAngle = angle;

        targetPoint = point;
        //GizmosHelper.DrawBox(point, Vector3.one * 0.2f, Color.yellow);
        Vector3 direction = point - firePoint.position;

        if(Vector3.Angle(direction, transform.forward) > 10)
        {
            // Haven't face the right direction.
            projectileArc.gameObject.SetActive(false);
            return;
        }
        else
        {
            projectileArc.gameObject.SetActive(true);
        }

        float yOffset = -direction.y;
        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        currentSpeed = ProjectileMath.CalculateLaunchSpeed(distance, yOffset, Physics.gravity.magnitude, angle * Mathf.Deg2Rad);

        projectileArc.UpdateArc(currentSpeed, distance, Physics.gravity.magnitude, currentAngle * Mathf.Deg2Rad, direction, true);
    }

    public void SetTargetWithSpeed(Vector3 point, float speed, bool useLowAngle)
    {
        currentSpeed = speed;

        Vector3 direction = point - firePoint.position;
        float yOffset = direction.y;
        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        float angle0, angle1;
        bool targetInRange = ProjectileMath.CalculateLaunchAngle(speed, distance, yOffset, Physics.gravity.magnitude, out angle0, out angle1);

        if (targetInRange)
            currentAngle = useLowAngle ? angle1 : angle0;

        projectileArc.UpdateArc(speed, distance, Physics.gravity.magnitude, currentAngle, direction, targetInRange);
    }
}
