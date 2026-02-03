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
            Debug.LogError("CombatantHealthbar: No target assigned!");
            target = GetComponentInParent<Combatant>();
            return;
        }

        healthSlider.maxValue = target.MaxHealth;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        healthSlider.value = target.CurrentHealth;
        healthBarText.text = $"{target.CurrentHealth} / {target.MaxHealth}";
    }
    public void Bind(Combatant combatant)
    {
        target = combatant;
        healthSlider.maxValue = target.MaxHealth;
        UpdateUI();
    }
}
