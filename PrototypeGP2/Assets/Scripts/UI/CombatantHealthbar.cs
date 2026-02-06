using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CombatantHealthbar : MonoBehaviour
{
    [Header("Target")]
    public Combatant target;

    [Header("UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthBarText;

    void Start()
    {
        if (target == null)
        {
            target = GetComponentInParent<Combatant>();
        }

        if (target != null && healthSlider != null)
        {
            healthSlider.maxValue = target.MaxHealth;
        }
        UpdateUI();
    }

    public void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (target == null || healthSlider == null) return;
        
        if (healthSlider.maxValue != target.MaxHealth)
        {
            healthSlider.maxValue = target.MaxHealth;
        }

        healthSlider.value = target.CurrentHealth;
        
        if (healthBarText != null)
        {
            healthBarText.text = $"{Mathf.Max(0, target.CurrentHealth):0} / {target.MaxHealth:0}";
        }
    }

    public void Bind(Combatant combatant)
    {
        target = combatant;
        if (target != null && healthSlider != null)
        {
            healthSlider.maxValue = target.MaxHealth;
            UpdateUI();
        }
    }
}
