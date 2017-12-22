using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 m_motion;
    public float speed;
    private Vector3 m_lookPos;

    private Vector3 leftDown;
    private Vector3 rightUp;
    public float boundaryThickness = 0.3f;
    [SerializeField]
    private bool m_isConfused = false;

    private Animator m_animator;

    private Vector2 firstBoundary = new Vector2(0.4f, 0.6f);
    private Vector2 secondaryBoundary = new Vector2(0.1f, 0.9f);

    public bool IsConfused
    {
        get
        {
            return m_isConfused;
        }

        set
        {
            m_isConfused = value;
        }
    }

    public Animator Animator
    {
        get
        {
            if (m_animator == null)
                m_animator = GetComponentInChildren<Animator>();
            return m_animator;
        }

        set
        {
            m_animator = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        //FindRoomCornerPoints(out leftDown, out rightUp);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseHitPos();
        MovementTypeB();

        //ClampPlayerPos();

        AttackCheck();

        CheckScreenPosition();
    }

    private void CheckScreenPosition()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        //print("pos " + screenPos);

        if (screenPos.x < secondaryBoundary.x || screenPos.x > secondaryBoundary.y
            || screenPos.y < secondaryBoundary.x || screenPos.y > secondaryBoundary.y)
        {
            screenPos = new Vector3(
                Mathf.Clamp(screenPos.x, secondaryBoundary.x, secondaryBoundary.y),
                Mathf.Clamp(screenPos.y, secondaryBoundary.x, secondaryBoundary.y),
                screenPos.z);

            Vector3 worldPos = Camera.main.ViewportToWorldPoint(screenPos);
            Vector3 offset = worldPos - transform.position;
            //print("offset " + offset);

            Camera.main.transform.position -= offset.SetY(0);
        }
        else if (screenPos.x < firstBoundary.x || screenPos.x > firstBoundary.y
            || screenPos.y < firstBoundary.x || screenPos.y > firstBoundary.y)
        {
            screenPos = new Vector3(
                Mathf.Clamp(screenPos.x, firstBoundary.x, firstBoundary.y),
                Mathf.Clamp(screenPos.y, firstBoundary.x, firstBoundary.y),
                screenPos.z);

            Vector3 worldPos = Camera.main.ViewportToWorldPoint(screenPos);
            Vector3 offset = worldPos - transform.position;
            //print("offset " + offset);

            Camera.main.transform.position -= offset.SetY(0) * 0.5f;
        }
    }

    private void MovementTypeB()
    {
        m_motion = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.position += m_motion * speed;
        if (m_motion.sqrMagnitude.Sgn() > 0)
            transform.forward = m_motion.normalized;

        if (m_isConfused == true)
            transform.forward = Quaternion.AngleAxis(10, Vector3.up) * transform.forward;

        print("speed " + (m_motion * speed).magnitude);
        if ((m_motion * speed).magnitude > 0.01f)
        {
            Animator.SetBool("Move", true);
            Animator.SetTrigger("Run");
        }
        else
        {
            Animator.SetBool("Move", false);
            Animator.SetTrigger("Idle");

            if (m_isConfused == false)
            {
                //transform.forward = (m_lookPos - transform.position).SetY(0);
                transform.forward = Vector3.Slerp(transform.forward, (m_lookPos - transform.position).SetY(0), 0.1f);
                //print("angle between " + Vector3.Angle(transform.forward, (m_lookPos - transform.position).SetY(0)));
            }
        }
    }

    private void MovementTypeA()
    {
        m_motion = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (m_isConfused == false)
            transform.forward = (m_lookPos - transform.position).SetY(0);
        else
            transform.forward = Quaternion.AngleAxis(10, Vector3.up) * transform.forward;

        Vector3 faceDir = (m_lookPos - transform.position).SetY(0);

        if (faceDir.magnitude > 0.5f)
        {
            transform.position += transform.forward * m_motion.z * speed;
        }
        else if (faceDir.magnitude > 0.1f)
        {
            transform.position += transform.forward * m_motion.z * speed * faceDir.magnitude;
        }
    }

    private void UpdateMouseHitPos()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            if (hit.transform.CompareTag("Ground"))
                m_lookPos = hit.point;
    }

    private void AttackCheck()
    {
    }

    public void FindRoomCornerPoints(out Vector3 leftDown, out Vector3 rightUp)
    {
        RaycastHit hitInfoLeft;
        Physics.Raycast(transform.position, Vector3.left, out hitInfoLeft, 100, 1 << LayerMask.NameToLayer("Wall"));
        Debug.DrawLine(transform.position, hitInfoLeft.transform.position, Color.red);

        RaycastHit hitInfoRight;
        Physics.Raycast(transform.position, Vector3.right, out hitInfoRight, 100, 1 << LayerMask.NameToLayer("Wall"));
        Debug.DrawLine(transform.position, hitInfoRight.transform.position, Color.red);

        RaycastHit hitInfoFoward;
        Physics.Raycast(transform.position, Vector3.forward, out hitInfoFoward, 100, 1 << LayerMask.NameToLayer("Wall"));
        Debug.DrawLine(transform.position, hitInfoFoward.transform.position, Color.red);

        RaycastHit hitInfoBackward;
        Physics.Raycast(transform.position, Vector3.back, out hitInfoBackward, 100, 1 << LayerMask.NameToLayer("Wall"));
        Debug.DrawLine(transform.position, hitInfoBackward.transform.position, Color.red);

        leftDown = new Vector3(hitInfoLeft.transform.position.x + boundaryThickness, hitInfoBackward.transform.position.z + boundaryThickness);
        rightUp = new Vector3(hitInfoRight.transform.position.x - boundaryThickness, hitInfoFoward.transform.position.z - boundaryThickness);
        return;
    }

    private void ClampPlayerPos()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, leftDown.x, rightUp.x),
            0,
            Mathf.Clamp(transform.position.z, leftDown.y, rightUp.y));
    }
}
