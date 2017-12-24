using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomCamera : MonoBehaviour
{
    [SerializeField]
    private float incline = 45;

    [SerializeField]
    private float minDistance = 10;

    [SerializeField]
    private float maxDistance = 20;

    [SerializeField]
    private float zoomSensitivity = 1;

    private float zoomLevel;

    public Transform target;
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

    void Awake()
    {
        transform.rotation = Quaternion.AngleAxis(incline, Vector3.right);
        zoomLevel = 0.5f;
        transform.position = target.position
            - transform.forward * (minDistance * zoomLevel + maxDistance * (1 - zoomLevel));
    }

    void Update()
    {
        float previousZoomLevel = zoomLevel;
        float zoomInput = -Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        if (zoomInput.Sgn() > 0)
            zoomLevel = Mathf.Lerp(zoomLevel, 0, Mathf.Abs(zoomInput));
        else if (zoomInput.Sgn() < 0)
            zoomLevel = Mathf.Lerp(zoomLevel, 1, Mathf.Abs(zoomInput));

        transform.position += transform.forward * (zoomLevel - previousZoomLevel) * (maxDistance - minDistance);
    }
}
