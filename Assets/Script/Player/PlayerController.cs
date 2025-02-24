
using UnityEngine;
using TMPro;
using System.Collections;
using System;



public class PlayerController : MonoBehaviour
{
    [Header("Tmt")]
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    [Header("Body")]
    [SerializeField] private Rigidbody playerRigibody;
    [SerializeField] private Transform oritantionTransform;
    private Vector3 movementDirection;
    public MovementState State;

    [Header("Movement")]
    private float horizontalInput, verticalInput;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float slideSpeed;

    [SerializeField] private float desiredMoveSpeed;
    [SerializeField] private float lastDesiredMoveSpeed;


    public bool sliding;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopehit;
    private bool exitingSlope;

    [Header("KeyBinds")]
    [SerializeField] private KeyCode Jump;
    [SerializeField] private KeyCode Run;

    [Header("Ground")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDrag;

    [Header("Jump")]
    private bool canJump;
    [SerializeField] private float JumpForce;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float jumpCooldown;



    private void Awake()
    {
        playerRigibody = GetComponent<Rigidbody>();
        playerRigibody.freezeRotation = true;
        canJump = true;
    }
    private void Update()
    {
        SetInputs();
        Slippery();
        StateHandler();

    }
    private void FixedUpdate()
    {
        SetPlayerMovment();
    }
    private void SetInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(Jump) && canJump && isGrounded())
        {
            canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), jumpCooldown);
        }
    }
    private void SetPlayerMovment()
    {
        movementDirection = oritantionTransform.forward * verticalInput + oritantionTransform.right * horizontalInput;

        if (OnSlope()&&!exitingSlope)
        {
            playerRigibody.AddForce(GetSlopeMoveDirection(movementDirection) * movementSpeed * 10f, ForceMode.Force);

            if (playerRigibody.linearVelocity.y > 0)
            {
                playerRigibody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (isGrounded())
        {
            playerRigibody.AddForce(movementDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded())
        {
            playerRigibody.AddForce(movementDirection.normalized * movementSpeed * airMultiplier, ForceMode.Force);
        }

       
        playerRigibody.useGravity = !OnSlope();

    }
    private void SetPlayerJumping()
    {
        exitingSlope = true;
        playerRigibody.linearVelocity = new Vector3(playerRigibody.linearVelocity.x, 0f, playerRigibody.linearVelocity.z);
        playerRigibody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private void ResetJumping()
    {
        canJump = true;
        exitingSlope = false;

    }
    private bool isGrounded()
    {
        return
            Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

    }


    private void Slippery()
    {
        if (isGrounded())
            playerRigibody.linearDamping = groundDrag;
        else
            playerRigibody.linearDamping = 0;

    }

    private void StateHandler()
    {
        if (sliding)
        {
            State = MovementState.sliding;
            if (OnSlope() && playerRigibody.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        

        else if (isGrounded() && Input.GetKey(Run))
        {
            State = MovementState.sprinting;
            desiredMoveSpeed= sprintSpeed;
        }
        else if (isGrounded())
        {
            State = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            State = MovementState.air;

        }
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 2f && movementSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else {movementSpeed = desiredMoveSpeed; }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        sliding,
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopehit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopehit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {return Vector3.ProjectOnPlane(direction,slopehit.normal).normalized;
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (playerRigibody.linearVelocity.magnitude > movementSpeed)
                playerRigibody.linearVelocity = playerRigibody.linearVelocity.normalized * movementSpeed;
        }
        else 
        { 

                Vector3 flatvel = new Vector3(playerRigibody.linearVelocity.x,0f,playerRigibody.linearVelocity.z);
        if(flatvel.magnitude >movementSpeed)
        {
            Vector3 limitedVel = flatvel.normalized*movementSpeed;
            playerRigibody.linearVelocity = new Vector3(limitedVel.x,playerRigibody.linearVelocity.y,limitedVel.z);
        }
         }
        _textMeshPro.text = desiredMoveSpeed.ToString();
    }
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time =  0;
        float difference = Mathf.Abs(desiredMoveSpeed -movementSpeed);
        float startValue = movementSpeed;
        while (time<difference)
        {
            movementSpeed = Mathf.Lerp(startValue,desiredMoveSpeed,time/difference);
            time += Time.deltaTime;
            yield return null;    
        }
        movementSpeed = desiredMoveSpeed;
    }

}

