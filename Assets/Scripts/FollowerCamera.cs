using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowerCamera : MonoBehaviour
{
    public Transform target;
    public Rect hardEdge;
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
    }

    private void DrawRect()
    {
        Vector3 leftDown = ThisCamera.ViewportToWorldPoint(((Vector3)hardEdge.min).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightUp = ThisCamera.ViewportToWorldPoint(((Vector3)hardEdge.max).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 leftUp = ThisCamera.ViewportToWorldPoint(new Vector3(hardEdge.xMin, hardEdge.yMax, ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightDown = ThisCamera.ViewportToWorldPoint(new Vector3(hardEdge.xMax, hardEdge.yMin, ThisCamera.nearClipPlane + 0.1f));
        GizmosHelper.DrawLine(leftDown, leftUp, Color.red);
        GizmosHelper.DrawLine(leftUp, rightUp, Color.red);
        GizmosHelper.DrawLine(rightUp, rightDown, Color.red);
        GizmosHelper.DrawLine(rightDown, leftDown, Color.red);
    }

    private void OnDrawGizmos()
    {
        Vector3 leftDown = ThisCamera.ViewportToWorldPoint(((Vector3)hardEdge.min).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightUp = ThisCamera.ViewportToWorldPoint(((Vector3)hardEdge.max).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 leftUp = ThisCamera.ViewportToWorldPoint(new Vector3(hardEdge.xMin, hardEdge.yMax, ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightDown = ThisCamera.ViewportToWorldPoint(new Vector3(hardEdge.xMax, hardEdge.yMin, ThisCamera.nearClipPlane + 0.1f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightUp, rightDown);
        Gizmos.DrawLine(rightDown, leftDown);
    }
}
