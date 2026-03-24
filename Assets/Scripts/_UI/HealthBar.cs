using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Target")]
    [SerializeField] private StatHandler targetHP;

    private void Awake()
    {
        if (targetHP == null)
        {
            targetHP = GetComponentInParent<StatHandler>();
        }
    }

    // Unity Event
    private void OnEnable()
    {
        if (targetHP != null)
        {
            targetHP.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(targetHP.CurrentHP, targetHP.MaxHP);
        }
    }

    private void OnDisable()
    {
        if (targetHP != null) targetHP.OnHealthChanged -= UpdateHealthUI;
    }

    // function
    private void UpdateHealthUI(int current, int max)
    {
        if (hpSlider != null)
        {
            if (max <= 0)
                hpSlider.value = 0;
            else
                hpSlider.value = (float)current / max;
        }

        if (hpText != null)
        {
            hpText.text = $"{current} / {max}";
        }
    }
}