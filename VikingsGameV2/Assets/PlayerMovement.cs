using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 720f;

    [Header("Jump Settings")]
    public float jumpSpeed = 5f;
    public float gravityMultiplier = 1f;
    public float gracePeriod = 0.2f;

    [Header("Speed Boost Settings")]
    public float speedBoostMultiplier = 1.25f;

    [Header("Camera Settings")]
    public Camera mainCamera;

    [Header("Attack Settings")]
    public GameObject swordObject;
    public float attackMovementSlowdown = 0.4f;
    public float attackBufferTime = 0.3f;

    // Internal
    private CharacterController characterController;
    private Animator animator;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? lastJumpPressedTime;
    private float attackBufferTimer;
    private bool isAttacking;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        originalStepOffset = characterController.stepOffset;

        // Ensure sword is active
        if (swordObject) swordObject.SetActive(true);
    }

    void Update()
    {
        UpdateTimers();
        HandleAttackInput();

        ReadMovementInput(out Vector3 moveDir);

        UpdateAttackState();
        if (isAttacking)
        {
            moveDir *= attackMovementSlowdown;
        }

        HandleJumpAndGravity();

        Vector3 velocity = ApplyMovement(moveDir);
        UpdateAnimation(moveDir);

        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateTimers()
    {
        if (attackBufferTimer > 0f)
            attackBufferTimer -= Time.deltaTime;
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isAttacking)
            {
                attackBufferTimer = attackBufferTime;
            }
            else
            {
                TriggerAttack();
            }
        }
    }

    private void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    private void ReadMovementInput(out Vector3 moveDir)
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 raw = new Vector3(h, 0f, v);

        if (mainCamera)
        {
            // Use camera's forward and right vectors for movement direction
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            raw = camForward * v + camRight * h;
        }

        float mag = Mathf.Clamp01(raw.magnitude) * speed;
        moveDir = raw.normalized * mag;
    }

    private void HandleJumpAndGravity()
    {
        bool grounded = characterController.isGrounded;
        if (grounded)
        {
            lastGroundedTime = Time.time;
            ySpeed = -0.5f;
            characterController.stepOffset = originalStepOffset;
        }
        else
        {
            characterController.stepOffset = 0f;
        }

        if (Input.GetButtonDown("Jump"))
            lastJumpPressedTime = Time.time;

        ySpeed += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        float sinceGrounded = Time.time - (lastGroundedTime ?? float.NegativeInfinity);
        float sinceJumpPressed = Time.time - (lastJumpPressedTime ?? float.NegativeInfinity);
        if (sinceGrounded <= gracePeriod && sinceJumpPressed <= gracePeriod)
        {
            ySpeed = jumpSpeed;
            lastGroundedTime = null;
            lastJumpPressedTime = null;
        }
    }

    private Vector3 ApplyMovement(Vector3 horiz)
    {
        Vector3 vel = horiz;
        vel.y = ySpeed;
        return vel;
    }

    private void UpdateAnimation(Vector3 moveDir)
    {
        bool isMoving = moveDir.sqrMagnitude > 0.001f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAttackState()
    {
        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);

        if (st.IsTag("Attack"))
        {
            isAttacking = true;
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;

                if (attackBufferTimer > 0f)
                {
                    TriggerAttack();
                    attackBufferTimer = 0f;
                }
            }
        }
    }

    public void ActivateSpeedBoost(float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(duration));
    }

    private IEnumerator SpeedBoostCoroutine(float duration)
    {
        float oldSpeed = speed;
        speed *= speedBoostMultiplier;
        yield return new WaitForSeconds(duration);
        speed = oldSpeed;
    }
}