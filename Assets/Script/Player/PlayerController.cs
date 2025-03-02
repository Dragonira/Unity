
using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEditor.Rendering;



public class PlayerController : MonoBehaviour
{
    [Header("Tmt")]
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    [Header("References")]
    [SerializeField] private Transform PlayerObj;
    [SerializeField] private Rigidbody playerRigibody;
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform oritantionTransform;
    [SerializeField] private StaminaBars stanima;

    private Vector3 movementDirection;
    private MovementState State;
    private Vector3 delayedForceToApply;
 
    
    

    [Header("Movement")]
    private float horizontalInput, verticalInput;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float dashSpeed;


    [Header("Bools")]
    [SerializeField] private bool iswalking;
    [SerializeField] private bool sliding;
    [SerializeField] private bool dashing;
    [SerializeField] private bool canJump;
    [SerializeField] private bool sprinting;

    [Header("Slope Handling")]
    private float maxSlopeAngle;
    private RaycastHit slopehit;
    private bool exitingSlope;

    [Header("Dashing")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashUpwardForce;
    [SerializeField] private float dashDuration;

    [Header("Dashing Timer")]
    [SerializeField] private float dashCd;
    [SerializeField] private float dashCdTimer;

    [Header("KeyBinds")]
    [SerializeField] private KeyCode Jump;
    [SerializeField] private KeyCode Run;
    [SerializeField] private KeyCode dashKey;
    [SerializeField] private KeyCode slideKey;

    [Header("Ground")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDrag;

    [Header("Jump")]
    [SerializeField] private float JumpForce;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float jumpCooldown;

    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float SlideForce;
    [SerializeField] private float SlideTimer;
    [SerializeField] private float slideYscale;
    [SerializeField] private float startYScale;

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopehit.normal).normalized;
    }
    private void Awake()
    {
        stanima = GetComponent<StaminaBars>();
        playerRigibody = GetComponent<Rigidbody>();
        playerRigibody.freezeRotation = true;
        canJump = true;
        startYScale = PlayerObj.localScale.y;
    }
 
    private void Update()
    {
       
        SetInputs();
        Slippery();
        StateHandler();
        isGrounded();
        SpeedControl();
      

    }

    private void FixedUpdate()
    {
        SetPlayerMovment();
        SetSlindingInput();

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
        if (Input.GetKeyDown(dashKey))
        {
            dash();
        }
        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        }
        if (Input.GetKeyUp(slideKey) && sliding)
        {
            StopSlide();
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
    public bool isGrounded()
    {
        return
            Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

    }
    private void Slippery()
    {
        if (State == MovementState.walking|| State == MovementState.sprinting)
            playerRigibody.linearDamping = groundDrag;
        else
            playerRigibody.linearDamping = 0;

    }
    private void StateHandler()
    {
        if(dashing)
        {
            State = MovementState.dash;
            movementSpeed = dashSpeed;
        }

        else if (sliding)
        {
            State = MovementState.sliding;
            if (OnSlope() && playerRigibody.linearVelocity.y < 0.1f)
                movementSpeed = slideSpeed;
            else
                movementSpeed = sprintSpeed;
        }
        

        else if (isGrounded() && Input.GetKey(Run))
        {
           
            State = MovementState.sprinting;
            movementSpeed = sprintSpeed;
        }
        else if (isGrounded())
        {
            State = MovementState.walking;
            movementSpeed = walkSpeed;
            
            
        }
        else
        {
            State = MovementState.air;

        }
  
     
    }
    private enum MovementState
    {
        walking,
        sprinting,
        air,
        sliding,
        dash,
        idle,
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopehit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopehit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
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
        _textMeshPro.text = movementSpeed.ToString();
    }
    private void dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        dashing = true;
        Vector3 forceToApply = oritantionTransform.forward * dashForce + oritantionTransform.up * dashUpwardForce;
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);

    }
    private void DelayedDashForce()
    {
        playerRigibody.AddForce(delayedForceToApply, ForceMode.Impulse);
    }
  
    private void ResetDash()
    {
        dashing = false;
    }
    private void StartSlide()
    {
        canJump = false;
        sprinting = false;
        sliding = true;
        PlayerObj.localScale = new Vector3(PlayerObj.localScale.x, slideYscale, PlayerObj.localScale.z);
        playerRigibody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        SlideTimer = maxSlideTime;
    }
    private void slidingMovement()
    {
        Vector3 inputDirection = oritantionTransform.forward * verticalInput + oritantionTransform.right * horizontalInput;

        if (!OnSlope() || playerRigibody.linearVelocity.y > -0.1f)
        {

            playerRigibody.AddForce(inputDirection.normalized * SlideForce, ForceMode.Force);

            SlideTimer -= Time.deltaTime;
        }
        else
        {
            playerRigibody.AddForce(GetSlopeMoveDirection(inputDirection) * SlideForce, ForceMode.Force);
        }

        if (SlideTimer <= 0)
            StopSlide();

    }
    private void StopSlide()
    {
        canJump = true;
        sprinting = true;
        sliding = false;
        PlayerObj.localScale = new Vector3(PlayerObj.localScale.x, startYScale, PlayerObj.localScale.z);
    }
    private void SetSlindingInput() {

        if (sliding)
            slidingMovement();

     
    }
    public bool CanJump() { return canJump; }
    

  
}
