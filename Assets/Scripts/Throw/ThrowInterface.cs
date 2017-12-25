using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ThrowInterface : MonoBehaviour
{
    enum ThrowType
    {
        useInitialAngle,
        useInitialSpeed,
        useBothAngleAndSpeed
    }
    
    public ProjectArrowMover targetCursor;

    [SerializeField]
    public ThrowController throwController;
    
    [SerializeField]
    private float m_initialFireAngle = 45;
    [SerializeField]
    private float m_initialFireSpeed = 35;
    [SerializeField]
    private bool useLowAngle = true;

    public float minAngle = 0;
    public float maxAngle = 60;
    public float minSpeed = 1;
    public float maxSpeed = 15;
    private float m_dynamicMaxSpeed = 15;
    public float chargeSensitivity = 1f;

    [SerializeField]
    private ThrowType type = ThrowType.useInitialAngle;

    public float DynamicMaxSpeed
    {
        get
        {
            return m_dynamicMaxSpeed;
        }

        set
        {
            m_dynamicMaxSpeed = value;
        }
    }

    public float InitialFireAngle
    {
        get
        {
            return m_initialFireAngle;
        }

        set
        {
            value = Mathf.Clamp(value, minAngle, maxAngle);
            m_initialFireAngle = value;
        }
    }

    private void Start()
    {
        DynamicMaxSpeed = m_initialFireSpeed;
    }

    void Update()
    {
        DrawLine();

        //if (Input.GetButtonUp("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    throwController.Throw();
        //}
    }

    private void DrawLine()
    {
        if (type == ThrowType.useInitialAngle)
            throwController.SetTargetWithAngle(targetCursor.transform.position, m_initialFireAngle);
        else if (type == ThrowType.useInitialSpeed)
            throwController.SetTargetWithSpeed(targetCursor.transform.position, m_initialFireSpeed, useLowAngle);
        else if (type == ThrowType.useBothAngleAndSpeed)
            throwController.SetTargetWithBothAngleAndSpeed(targetCursor.transform.position, m_initialFireAngle, m_initialFireSpeed);
    }

    public void SetEnable(bool _isEnable)
    {
        throwController.SetEnable(_isEnable);
        this.enabled = _isEnable;
    }

    public void StartCharge()
    {
        m_initialFireSpeed = minSpeed;
    }

    public void StopChargeAndLaunch()
    {
        throwController.Throw();

        m_initialFireSpeed = minSpeed;
        // To clear the old line before next time it apears
        throwController.projectileArc.GetComponent<LineRenderer>().positionCount = 0;
    }

    public void SetThrowChargeTime(float _time)
    {
        m_initialFireSpeed = Mathf.Lerp(m_initialFireSpeed, DynamicMaxSpeed, chargeSensitivity * Time.deltaTime);
    }
}
