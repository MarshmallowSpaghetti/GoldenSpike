﻿using System.Collections;
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

    //public Transform leftHandTrans;
    //public Transform rightHandTrans;
    public Transform holdingItem;
    [SerializeField]
    Transform firePoint;

    private ThrowInterface m_throwComponent;
    private bool m_isAiming = false;
    private float m_startAimTime = -1;

    public float keyboardSensitivity = 5;

    private PlayerAnim m_playerAnimController;
    private CharacterController m_charController;

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

    public ThrowInterface ThrowComponent
    {
        get
        {
            if (m_throwComponent == null)
                m_throwComponent = FindObjectOfType<ThrowInterface>();
            return m_throwComponent;
        }
    }

    public PlayerAnim PlayerAnimController
    {
        get
        {
            if (m_playerAnimController == null)
                m_playerAnimController = GetComponent<PlayerAnim>();
            return m_playerAnimController;
        }

        set
        {
            m_playerAnimController = value;
        }
    }

    public CharacterController CharController
    {
        get
        {
            if (m_charController == null)
                m_charController = GetComponent<CharacterController>();
            return m_charController;
        }

        set
        {
            m_charController = value;
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
        GlobalMoveFaceMouse();

        //ClampPlayerPos();

        CheckMouseStatus();
        CheckKeyboard();
    }

    private void CheckMouseStatus()
    {
        if (Input.GetMouseButton(0) && m_isAiming == false)
        {
            m_isAiming = true;
            m_startAimTime = Time.time;
            ThrowComponent.SetEnable(true);
            ThrowComponent.StartCharge();
            PlayerAnimController.Hold();
        }
        else if (Input.GetMouseButton(0) == false && m_isAiming == true)
        {
            m_isAiming = false;
            m_startAimTime = -1;
            ThrowComponent.SetEnable(false);
            ThrowComponent.StopChargeAndLaunch();
            PlayerAnimController.Throw();
        }

        if(m_startAimTime >= 0)
        {
            ThrowComponent.SetThrowChargeTime(Time.time - m_startAimTime);
            UpdateDynamicalMaxSpeed();
        }
    }

    private void CheckKeyboard()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            ThrowComponent.InitialFireAngle += keyboardSensitivity * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.E))
        {
            ThrowComponent.InitialFireAngle += -keyboardSensitivity * Time.deltaTime;
        }
    }

    private void UpdateDynamicalMaxSpeed()
    {
        Vector3 direction2D = 
            (ThrowComponent.targetCursor.transform.position - firePoint.position).SetY(0);
        Vector3 extend = Mathf.Max(direction2D.magnitude * 0.5f, 1) * direction2D.normalized;

        float maxSpeed =
            ThrowComponent.throwController.GetRequiredSpeed(ThrowComponent.targetCursor.transform.position + extend, ThrowComponent.InitialFireAngle);

        //print("max speed " + maxSpeed);
        ThrowComponent.DynamicMaxSpeed = maxSpeed;
    }

    private void GlobalMoveAlwaysForward()
    {
        m_motion = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //print("motion " + m_motion.normalized);
        //if (m_motion.sqrMagnitude.Sgn() > 0)
        float angleInAFrame = 300 * Time.deltaTime;
        if (Vector3.Angle(transform.forward, m_motion) < angleInAFrame * 2)
        {
            transform.position += m_motion * speed;
            transform.forward = m_motion.normalized;
            GizmosHelper.DrawLine(transform.position, transform.position + m_motion * 10, Color.blue);
        }
        else if (m_motion.sqrMagnitude.Sgn() > 0)
        {
            transform.Rotate(Vector3.up, Vector3.SignedAngle(transform.forward, m_motion, Vector3.up).Sgn() * angleInAFrame);
            //transform.forward = Vector3.Slerp(transform.forward, m_motion.normalized,  5f * Time.deltaTime);

            //print("angle " + Vector3.Angle(transform.forward, m_motion));
        }
    }

    private void GlobalMoveFaceMouse()
    {
        m_motion = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //print("motion " + m_motion.normalized);
        //transform.position += m_motion * speed;
        CharController.Move(m_motion * speed);

        transform.forward =
            Vector3.Slerp(transform.forward, (m_lookPos - transform.position).SetY(0), 0.1f);
        
        PlayerAnimController.GetAnimOffset(transform.forward, m_motion);
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
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit))
        //    if (hit.transform.CompareTag("Ground"))
        //        m_lookPos = hit.point;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            m_lookPos = hit.point;
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.GetComponent<PickableItem>())
        //{
        //    //print("Pick " + collision.gameObject.name);
        //    if (leftHandTrans != null && holdingItem == null)
        //    {
        //        collision.transform.position = (leftHandTrans.position + rightHandTrans.position) * 0.5f;
        //        collision.transform.SetParent(transform);
        //        collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //        holdingItem = collision.transform;
        //    }
        //}
    }
}
