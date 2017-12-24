using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ThrowInterface : MonoBehaviour
{
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
    private bool useInitialAngle = true;
    
    void Update()
    {
        if (useInitialAngle)
            throwController.SetTargetWithAngle(targetCursor.transform.position, initialFireAngle);
        else
            throwController.SetTargetWithSpeed(targetCursor.transform.position, initialFireSpeed, useLowAngle);

        //if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    throwController.Fire();
        //}
    }
}
