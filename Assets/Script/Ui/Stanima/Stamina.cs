using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider;
    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaRegenRate = 5f; // Stamina yenilenme h�z�
    public float attackCost = 20f; // Sald�r� ba��na stamina d�����

    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
   

        // Stamina zamanla yenilensin
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            staminaSlider.value = currentStamina;
        }
    }

    public void Attack()
    {
        if (currentStamina >= attackCost)
        {
            currentStamina -= attackCost;
            staminaSlider.value = currentStamina;
            Debug.Log("Attack yap�ld�! Stamina d��t�.");
        }
        else
        {
            Debug.Log("Yetersiz stamina!");
        }
    }
}



