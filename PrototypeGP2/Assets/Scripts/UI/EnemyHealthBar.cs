using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class EnemyHealthBar : MonoBehaviour
{
    public Slider HealthBarSlider;
    public TextMeshProUGUI HealthBarText;
    public Enemy _enemy;

    void Start()
    {
        _enemy = FindAnyObjectByType(typeof(Enemy)) as Enemy;
        if (_enemy == null)
        {
            Debug.Log("_enemy not found");
        }
    }

    void Update()
    {

        HealthBarText.text = _enemy.CurrentHealth.ToString() + "/" + _enemy.maxHealth.ToString();
        HealthBarSlider.value = _enemy.CurrentHealth / 100f;
        HealthBarSlider.maxValue = _enemy.maxHealth;

    }
   
}
