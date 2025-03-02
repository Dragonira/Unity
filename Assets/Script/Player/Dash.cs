using UnityEngine;

public class Dash : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerCam;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerController pm;

    [Header("Dashing")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashUpwardForce;
    [SerializeField] private float dashDuration;

    [Header("Dashing")]
    [SerializeField] private float dashCd;
    [SerializeField] private float dashCdTimer;

    [Header("Input")]
    [SerializeField] private KeyCode dashKey;


    private void Update()
    {
        if(Input.GetKeyDown(dashKey))
        {
            dash();
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        pm = GetComponent<PlayerController>();
    }
    private void dash()
    {
        Vector3 forceToApply = orientation.forward* dashForce+orientation.up*dashUpwardForce;
        rb.AddForce(forceToApply,ForceMode.Impulse);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {

    }

}
