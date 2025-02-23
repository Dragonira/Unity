using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform oritantion;
    [SerializeField] private Transform PlayerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerController pm;
    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float SlideForce;
    [SerializeField] private float SlideTimer;
    [SerializeField] private float slideYscale;
    [SerializeField] private float startYScale;
    [Header("Input")]
    [SerializeField] private KeyCode slideKey;
    private float horizontalInput;
    private float verticalInput;
 

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if(Input.GetKeyDown(slideKey)&&(horizontalInput !=0 || verticalInput != 0))
        {
            StartSlide();
        }
        if (Input.GetKeyUp(slideKey) && pm.sliding)
        {
           StopSlide();
        }

    }
    private void FixedUpdate()
    {
        if (pm.sliding)
            slidingMovement();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerController>();
        startYScale = PlayerObj.localScale.y;
    }

    private void StartSlide()
    {
        pm.sliding = true;
        PlayerObj.localScale = new Vector3 (PlayerObj.localScale.x,slideYscale,PlayerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        SlideTimer = maxSlideTime;
    }
    private void slidingMovement()
    {
        Vector3 inputDirection = oritantion.forward * verticalInput + oritantion.right * horizontalInput;

        if (!pm.OnSlope() || rb.linearVelocity.y > -0.1f) { 

            rb.AddForce(inputDirection.normalized * SlideForce, ForceMode.Force);

            SlideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * SlideForce, ForceMode.Force);
        }
       
        if(SlideTimer <= 0) 
            StopSlide();
        
    }
    private void StopSlide()
    {
        pm.sliding = false;
        PlayerObj.localScale = new Vector3(PlayerObj.localScale.x, startYScale, PlayerObj.localScale.z);
    }
}
