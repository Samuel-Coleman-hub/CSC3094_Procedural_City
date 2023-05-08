using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float movementSpeed;
    public float runSpeed;
    public float walkSpeed;

    [SerializeField] private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;

    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public Transform orientation;
    private bool readyToJump = true;


    [Header("Keybinds")]
    public KeyCode runKey = KeyCode.R;
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool onGround;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public Rigidbody rb;

    public MovementState movementState;
    public enum MovementState
    {
        walk,
        run,
        air,
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        onGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        SpeedControl();
        StateHandler();

        if (onGround)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && onGround)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        if (onGround && Input.GetKey(runKey))
        {
            movementState = MovementState.run;
            desiredMoveSpeed = runSpeed;
        }
        else if (onGround)
        {
            movementState = MovementState.walk;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            movementState = MovementState.air;
        }

        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f)
        {
            StopAllCoroutines();
            StartCoroutine(LerpMovementSpeed());
        }
        else
        {
            movementSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator LerpMovementSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - movementSpeed);
        float startValue = movementSpeed;

        while(time < difference)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time/difference);
            time += Time.deltaTime * speedIncreaseMultiplier;
            

            yield return null;
        }

        movementSpeed = desiredMoveSpeed;
    }

    private void MoveCharacter()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (onGround)
        {
            rb.AddForce(10f * movementSpeed * moveDirection.normalized, ForceMode.Force);
        }
        else if(!onGround)
        {
            rb.AddForce(10f * airMultiplier * movementSpeed * moveDirection.normalized, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > movementSpeed)
        {
             Vector3 limitedVel = flatVel.normalized * movementSpeed;
             rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

}
