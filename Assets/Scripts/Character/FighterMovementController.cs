using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles fighter movements and actions
public class FighterMovementController : MonoBehaviour
{
    public Transform otherFighter;
    public LayerMask groundMask;
    public float moveSpeed, jumpForce, pushBackForce;

    private Rigidbody rb;
    internal Vector3 _currentDirection, _jumpMoveDirection;
    public bool isMoving, groundPlayer, lockMovement, _canSet, rotate;
    internal float _walkDirection, _xAxis, _yAxis;


    [Header("AI Settings")]
    public float minimumDistance, defenseDistance;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FaceTarget()
    {
        if (otherFighter == null)
        {
            return;
        }

        Vector3 diff = otherFighter.position - transform.position;
        diff.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff), 20 * Time.deltaTime);
    }

    public void MovementUpdate()
    {
        if (rotate)
            FaceTarget();
    }

    private void FixedUpdate()
    {
        if (isMoving)
            MoveLogic(_walkDirection, _xAxis, _yAxis, _canSet);
        else
            _walkDirection = 0;

        if (canGround() && groundPlayer)
        {
            GroundPlayer();
        }
    }

    public void GroundPlayer()
    {
        //Spawn Audio here
        var b = rb.velocity;
        b.y = 0;
        rb.velocity = b;
        rb.Sleep();
        rb.WakeUp();
        groundPlayer = false;
    }

    public void MoveLogic(float walkDirection, float xAxis, float yAxis, bool canSet)
    {
        rotate = true;

        if (!lockMovement)
        {
            float targetAngle = Mathf.Atan2(xAxis, yAxis) * Mathf.Rad2Deg + transform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward * walkDirection;
            if(canSet)
                _currentDirection = moveDirection;

            Vector3 newPos = rb.position + moveDirection;
            rb.position = Vector3.MoveTowards(rb.position, newPos, moveSpeed * Time.deltaTime);
        }
    }

    public void JumpLogic() { rb.AddForce((_jumpMoveDirection + Vector3.up).normalized * jumpForce, ForceMode.Impulse); }
    public void DropLogic() { rb.AddForce((_jumpMoveDirection + Vector3.down).normalized * jumpForce, ForceMode.Impulse); groundPlayer = true; }
    public void PushForwardLogic() { rb.AddForce(transform.forward * jumpForce, ForceMode.Impulse); }

    public void PushBackwardsLogic()
    {
        rb.Sleep();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 direction = otherFighter.position - transform.position;
        direction.y = 0;
        rb.AddForce(-direction.normalized * pushBackForce, ForceMode.Impulse);
    }

    public void LockMovement()
    {
        lockMovement = true;
    }

    public void UnlockMovement()
    {
        lockMovement = false;
    }

    public bool canGround()
    {
        return Physics.CheckSphere(transform.position, 0.3f, groundMask, QueryTriggerInteraction.Ignore);
    }

    public bool isGrounded()
    {
        if (canGround() && !groundPlayer)
            return true;

        return false;
    }

    public float DistanceFromTarget()
    {
        return Vector3.Distance(new Vector3(otherFighter.position.x, 0, otherFighter.position.z), new Vector3(transform.position.x, 0, transform.position.z));
    }

    public bool FighterWithinRange(float minimumDistance)
    {
        return DistanceFromTarget() < minimumDistance;
    }
}
