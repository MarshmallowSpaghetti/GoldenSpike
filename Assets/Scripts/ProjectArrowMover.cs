using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectArrowMover : MonoBehaviour
{
    public Transform arrowMesh;
    private Vector3 m_initPos;
    public float speed;
    public float amplitude;

    // Use this for initialization
    void Start()
    {
        m_initPos = arrowMesh.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        FindProjectPosition();
        UpdateArrowVertically();
    }
    
    private void FindProjectPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            transform.position = hit.point;
        }
    }

    private void UpdateArrowVertically()
    {
        arrowMesh.localPosition = m_initPos.AddY(Mathf.Sin(Time.time * speed) * amplitude);
    }
}
