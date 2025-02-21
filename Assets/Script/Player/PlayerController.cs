using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform oritantionTransform;
    [SerializeField] private KeyCode Jump;
    [SerializeField] private float JumpForce;
    private Rigidbody playerRigibody;
    [SerializeField] private float movementSpeed;
    private float horizontalInput, verticalInput;
    private bool canJump;
    [SerializeField] private float jumpCooldown;

    private Vector3 movementDirection;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        playerRigibody = GetComponent<Rigidbody>();
        playerRigibody.freezeRotation = true;
        canJump = true;
    }
    private void Update()
    {
        SetInputs();

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
        playerRigibody.AddForce(movementDirection.normalized * movementSpeed, ForceMode.Force);
    }
    private void SetPlayerJumping()
    {
        playerRigibody.linearVelocity = new Vector3(playerRigibody.linearVelocity.x, 0f, playerRigibody.linearVelocity.z);
        playerRigibody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private void ResetJumping()
    {
        canJump = true;

    }
    private bool isGrounded()
    {
        return
            Physics.Raycast(transform.position, Vector3.down, playerHeight * 1 + 1, groundLayer);

    }
}

