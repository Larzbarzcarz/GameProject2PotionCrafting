using UnityEngine;
using TMPro;
using UnityEngine.UI;
using _Project._Scripts.Sound_and_Music;


public class CombatantHealthbar : MonoBehaviour
{
    [Header("Target")]
    public Combatant target;

    [Header("UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthBarText;

    void Awake()
    {
        //-----FMOD Implementation-----//
        AudioManager.Instance.PlayMusic(FMODEvents.instance.CombatMusic);
        //-----FMOD Implementation-----//

        if (target == null)
        {
            target = GetComponentInParent<Combatant>();
        }

        if (target != null && healthSlider != null)
        {
            healthSlider.maxValue = target.MaxHealth;
            UpdateUI();
        }
        else if (target == null)
        {
            Debug.LogWarning($"CombatantHealthbar on {gameObject.name}: No target assigned or found in parents.");
        }
    }

    public void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (target == null || healthSlider == null) return;

        healthSlider.value = target.CurrentHealth;
        
        if (healthBarText != null)
        {
            healthBarText.text = $"{target.CurrentHealth:F1} / {target.MaxHealth:F1}";
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
