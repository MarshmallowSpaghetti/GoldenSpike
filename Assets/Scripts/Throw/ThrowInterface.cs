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

    [SerializeField]
    private ThrowType type = ThrowType.useInitialAngle;
    
    void Update()
    {
        if (type == ThrowType.useInitialAngle)
            throwController.SetTargetWithAngle(targetCursor.transform.position, initialFireAngle);
        else if (type == ThrowType.useInitialSpeed)
            throwController.SetTargetWithSpeed(targetCursor.transform.position, initialFireSpeed, useLowAngle);
        else if (type == ThrowType.useBothAngleAndSpeed)
            throwController.SetTargetWithBothAngleAndSpeed(targetCursor.transform.position, initialFireAngle, initialFireSpeed);

        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            throwController.Throw();
        }
    }
}
