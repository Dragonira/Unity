using UnityEngine;
using UnityEngine.UI;

public class StaminaBars : MonoBehaviour
{
    [SerializeField] private float maxStanima;
    [SerializeField] private float stanima;
    [SerializeField] private float JumpCost;
    [SerializeField] private float RunCost;
    [SerializeField] private float DashCost;
    [SerializeField] private float SlideCost;
    [SerializeField] public bool weAreSprinting;
    [SerializeField] public bool Regenerate;

    [SerializeField] private Image stanimaProgress;
    public float attackCost = 20f;
    public float staminaRegenRate = 50f; 
    private PlayerController pm;
    public float maxStamina = 100f;
    private float currentStamina;
    [Header("Stanima Speed Paramenters")]
    [Range(0, 50)][SerializeField] private float stanimaDrain = 0.5f;

    [SerializeField] PlayerController PlayerController;

    private void Start()
    {
        stanima = maxStanima;
        UpdateStaminaBar();
        PlayerController.GetComponent<PlayerController>();
       
       
    }
    public void StanimaJump()
    {
        if (pm.CanJump())
        {
            if (currentStamina >= (JumpCost / maxStamina))
            {
                currentStamina -= JumpCost;
            }
        }
    }

    public void Stanimalose()
    {
        if (!weAreSprinting)
        {
            if (currentStamina < maxStamina - 0.01)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                UpdateStaminaBar();
                if (currentStamina >= maxStamina)
                {
                    Regenerate = true;
                }
            }
        }


    }
    void Update()
    {
        Stanimalose();
        Attack();
        StanimaJump();
    }

    public void Attack()
    {
        if (Regenerate)
        { weAreSprinting = true;
            currentStamina -=stanimaDrain * Time.deltaTime;
            UpdateStaminaBar();
            Debug.Log("Attack yapýldý! Stamina düþtü.");
        }
        if (currentStamina <= 0)   
        {
            Regenerate = false;
        }
    }
    private void UpdateStaminaBar()
    {
        stanimaProgress.fillAmount = currentStamina / maxStamina;
    }
}





