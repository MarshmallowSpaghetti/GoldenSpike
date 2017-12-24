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

    [SerializeField]
    ProjectArrowMover targetCursor;

    [SerializeField]
    ThrowController throwController;
    
    [SerializeField]
    private float initialFireAngle = 45;
    [SerializeField]
    private float initialFireSpeed = 35;
    [SerializeField]
    private bool useLowAngle = true;

    public float minSpeed = 5;
    public float maxSpeed = 50;
    public float chargeSensitivity = 1f;

    [SerializeField]
    private ThrowType type = ThrowType.useInitialAngle;
    
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
            throwController.SetTargetWithAngle(targetCursor.transform.position, initialFireAngle);
        else if (type == ThrowType.useInitialSpeed)
            throwController.SetTargetWithSpeed(targetCursor.transform.position, initialFireSpeed, useLowAngle);
        else if (type == ThrowType.useBothAngleAndSpeed)
            throwController.SetTargetWithBothAngleAndSpeed(targetCursor.transform.position, initialFireAngle, initialFireSpeed);
    }

    public void SetEnable(bool _isEnable)
    {
        throwController.SetEnable(_isEnable);
        this.enabled = _isEnable;
    }

    public void StartCharge()
    {
        initialFireSpeed = minSpeed;
    }

    public void StopChargeAndLaunch()
    {
        throwController.Throw();

        initialFireSpeed = minSpeed;
        // To clear the old line before next time it apears
        DrawLine();
    }

    public void SetThrowChargeTime(float _time)
    {
        initialFireSpeed = Mathf.Lerp(initialFireSpeed, maxSpeed, chargeSensitivity * Time.deltaTime);
    }
}
