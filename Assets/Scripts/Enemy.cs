using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator m_animator;
    private bool m_isActive = false;
    private bool m_isFrozen = false;

    private Vector3 leftDown;
    private Vector3 rightUp;
    private Vector3 targetPos;

    public float speed = 1;
    public float boundaryThickness = 0.3f;

    public TextMesh countDownTxt;

    public bool IsActive
    {
        get
        {
            return m_isActive;
        }

        set
        {
            m_isActive = value;
            if (m_isActive)
            {
                targetPos = AssignPosInRoom();
                StartCoroutine(WalkingAround());
            }
        }
    }

    public bool IsFrozen
    {
        get
        {
            return m_isFrozen;
        }

        set
        {
            m_isFrozen = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();

        FindRoomCornerPoints(out leftDown, out rightUp);

        targetPos = new Vector3(0, -999, 0);

        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive == false)
            return;
    }

    public IEnumerator WalkingAround()
    {
        Vector3 targetForTarget = new Vector3(0, -999, 0);
        while (IsActive)
        {
            if (m_isFrozen)
            {
                if (countDownTxt != null)
                    countDownTxt.gameObject.GetComponent<MeshRenderer>().enabled = true;
                int cd = 2;
                while (cd > 0)
                {
                    if (countDownTxt != null)
                        countDownTxt.text = cd.ToString();
                    yield return new WaitForSeconds(1);
                    cd--;
                }

                if (countDownTxt != null)
                    countDownTxt.gameObject.GetComponent<MeshRenderer>().enabled = false;
                m_isFrozen = false;

                targetPos = AssignPosInRoom();
            }
            if (targetPos.y > -1)
            {
                //print("dis " + (targetPos - transform.position).SetY(0).magnitude);
                float targetDistance = (targetPos.SetY(0) - transform.position.SetY(0)).magnitude;
                if (targetDistance > 0.1f)
                {

                    //transform.Translate((targetPos.SetY(0) - transform.position.SetY(0)).normalized * Time.deltaTime * speed, Space.World);
                    transform.position += (targetPos.SetY(0) - transform.position.SetY(0)).normalized * Time.deltaTime * speed;

                    transform.forward = (targetPos.SetY(0) - transform.position.SetY(0));
                    if (CheckIfObstacleAhead(targetDistance))
                    {
                        if (targetForTarget.y < 0)
                        {
                            // Turn around
                            //StartCoroutine(TurnAroundAndFindAWay());

                            // Assign new target pos
                            //targetPos = targetPos.SetY(-999);

                            // Instead of assign a new one directly, we try to move the existing target point to somewhere reachable.

                            targetForTarget = AssignPosInRoom();
                        }
                        else
                        {
                            //print("Moving target pos from " + targetPos + " to " + targetForTarget);
                            targetPos = 
                                Vector3.Lerp(targetPos, targetForTarget,
                                0.5f * Time.deltaTime);
                            if ((targetPos - targetForTarget).magnitude < 0.1f)
                                targetForTarget = targetForTarget.SetY(-999);
                        }
                    }
                }
                else
                    targetPos = targetPos.SetY(-999);
            }
            else
            {
                yield return StartCoroutine(FindNewTargetAndTurnTo());
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetPos, 0.2f);
    }

    IEnumerator FindNewTargetAndTurnTo()
    {
        targetPos = AssignPosInRoom();

        float angleBetween = Vector3.SignedAngle(
            transform.forward.SetY(0), (targetPos.SetY(0) - transform.position.SetY(0)), Vector3.up);
        int cnt = 0;
        int frameToRotate = (int)(Mathf.Abs(angleBetween) / 3);
        while (cnt < frameToRotate)
        {
            //transform.forward = (player.position - transform.position).SetY(0);
            transform.Rotate(transform.up, angleBetween / frameToRotate);
            cnt++;

            yield return null;
        }
    }

    IEnumerator TurnAroundAndFindAWay()
    {
        // Step1. Find an angle where no more obstacle ahead
        float targetDistance = (targetPos.SetY(0) - transform.position.SetY(0)).magnitude;

        RaycastHit hitInfo;
        Physics.Raycast(transform.position, transform.forward, out hitInfo, 100, 1 << LayerMask.NameToLayer("Wall"));

        int randSgn = Random.Range(1, 3) == 1 ? 1 : -1;

        while (hitInfo.point != null
            && (hitInfo.point - transform.position).magnitude < targetDistance)
        {
            transform.Rotate(transform.up, 3 * randSgn);
            Physics.Raycast(transform.position, transform.forward, out hitInfo, 100, 1 << LayerMask.NameToLayer("Wall"));

            yield return null;
        }

        // Step2. Keep going and check if reached a place 
    }

    private bool CheckIfObstacleAhead(float _targetDistance)
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, transform.forward, out hitInfo, 100, 1 << LayerMask.NameToLayer("Wall"));

        // There's obstacle on way to target
        if (hitInfo.point != null
            && (hitInfo.point - transform.position).magnitude - boundaryThickness < _targetDistance)
        {
            return true;
        }
        return false;
    }

    private Vector3 AssignPosInRoom()
    {
        //print("new target pos");
        Vector3 targetPos = new Vector3(
            Random.Range(leftDown.x, rightUp.x), 0, Random.Range(leftDown.y, rightUp.y));
        //while ((targetPos.SetY(0) - player.position.SetY(0)).magnitude < 1.5f)
        //{
        //    targetPos = new Vector3(
        //    Random.Range(leftDown.x + 0.2f, rightUp.x - 0.2f), 0, Random.Range(leftDown.y + 0.2f, rightUp.y - 0.2f));
        //}
        return targetPos;
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
}
