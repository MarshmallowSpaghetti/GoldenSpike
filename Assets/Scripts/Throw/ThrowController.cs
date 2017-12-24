using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    [SerializeField]
    Transform firePoint;
    [SerializeField]
    Transform fireBase;

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
        SetThrowPoint(direction, currentAngle);
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
        SetThrowPoint(direction, currentAngle * Mathf.Rad2Deg);
    }

    public void Throw()
    {
        Player player = GetComponent<Player>();
        if (player.holdingItem != null)
        {
            print("fire");
            player.holdingItem.SetParent(null);
            player.holdingItem.GetComponent<Rigidbody>().isKinematic = false;
            player.holdingItem.GetComponent<Rigidbody>().velocity = 
                firePoint.up * currentSpeed;
            player.holdingItem = null;
        }
    }

    private void SetThrowPoint(Vector3 planarDirection, float turretAngle)
    {
        fireBase.rotation = Quaternion.LookRotation(planarDirection) * Quaternion.Euler(-90, -90, 0);
        firePoint.localRotation = Quaternion.Euler(90, 90, 0) * Quaternion.AngleAxis(turretAngle, Vector3.forward);
    }
}
