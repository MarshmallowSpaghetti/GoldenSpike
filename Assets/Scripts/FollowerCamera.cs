using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowerCamera : MonoBehaviour
{
    public Transform target;
    public Rect hardEdge;
    public Rect softEdge;
    private Camera m_camera;

    public Camera ThisCamera
    {
        get
        {
            if (m_camera == null)
                m_camera = GetComponent<Camera>();
            return m_camera;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Due to render order, the rect may not be the exact one.
        //DrawRect();

        CheckTargetViewportPos();
    }

    private void CheckTargetViewportPos()
    {
        Vector3 targetViewportPos = ThisCamera.WorldToViewportPoint(target.position);

        if (hardEdge.Contains(targetViewportPos) == false)
        {
            // Clamp it
            targetViewportPos = new Vector3(
                Mathf.Clamp(targetViewportPos.x, hardEdge.xMin, hardEdge.xMax),
                Mathf.Clamp(targetViewportPos.y, hardEdge.yMin, hardEdge.yMax),
                targetViewportPos.z
                );

            Vector3 clampedWorldPos = ThisCamera.ViewportToWorldPoint(targetViewportPos);
            // Fix camera to a certain height
            ThisCamera.transform.position -= (clampedWorldPos - target.position).SetY(0);
        }
        else if (softEdge.Contains(targetViewportPos) == false)
        {
            float offset =
                Mathf.Max(softEdge.xMin - targetViewportPos.x, 0)/(softEdge.xMin - hardEdge.xMin)
                + Mathf.Max(targetViewportPos.x - softEdge.xMax, 0) / (hardEdge.xMax -softEdge.xMax)
                + Mathf.Max(softEdge.yMin - targetViewportPos.y, 0) / (softEdge.yMin - hardEdge.yMin)
                + Mathf.Max(targetViewportPos.y - softEdge.yMax, 0) / (hardEdge.yMax - softEdge.yMax);
            offset = Mathf.Max(offset * 5, 0.4f);

            Vector3 lerpWorldPos = ThisCamera.ViewportToWorldPoint(
                Vector3.Lerp(targetViewportPos, new Vector3(0.5f, 0.5f, targetViewportPos.z), offset * Time.deltaTime));
            // Fix camera to a certain height
            ThisCamera.transform.position -= (lerpWorldPos - target.position).SetY(0);
        }
    }
    
    private void OnDrawGizmos()
    {
        DrawRect(hardEdge, Color.red);
        DrawRect(softEdge, Color.yellow);
    }

    private void DrawRect(Rect _rect, Color _color)
    {
        Vector3 leftDown = ThisCamera.ViewportToWorldPoint(((Vector3)_rect.min).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightUp = ThisCamera.ViewportToWorldPoint(((Vector3)_rect.max).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 leftUp = ThisCamera.ViewportToWorldPoint(new Vector3(_rect.xMin, _rect.yMax, ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightDown = ThisCamera.ViewportToWorldPoint(new Vector3(_rect.xMax, _rect.yMin, ThisCamera.nearClipPlane + 0.1f));

        Gizmos.color = _color;
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightUp, rightDown);
        Gizmos.DrawLine(rightDown, leftDown);
    }
}
