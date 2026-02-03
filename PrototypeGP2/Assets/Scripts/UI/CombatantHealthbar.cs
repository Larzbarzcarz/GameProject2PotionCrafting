using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CombatantHealthbar : MonoBehaviour
{
    [Header("Target")]
    public Combatant target;

    [Header("UI")]
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarText;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CombatantHealthbar: No target assigned!");
            enabled = false;
            return;
        }

        healthBarSlider.maxValue = target.MaxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        healthBarSlider.value = target.CurrentHealth;
        healthBarText.text = $"{target.CurrentHealth} / {target.MaxHealth}";
    }
}
