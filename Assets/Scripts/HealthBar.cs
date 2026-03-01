using System.Threading;
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

    // Unity Event
    private void Start()
    {
        if (targetHP != null) targetHP.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDestroy()
    {
        if (targetHP != null) targetHP.OnHealthChanged -= UpdateHealthUI;
    }

    // function
    private void UpdateHealthUI(int current, int max)
    {
        if (hpSlider != null)
        {
            hpSlider.value = (float)current / max;
        }

        if (hpText != null)
        {
            hpText.text = $"{current} / {max}";
        }
    }
}