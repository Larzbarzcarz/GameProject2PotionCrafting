using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class MonsterHealthBar : MonoBehaviour
{
   public Slider HealthBarSlider;
   public TextMeshProUGUI HealthBarText;
   public Monster _monster;

   void Start()
   {
      _monster = FindAnyObjectByType(typeof(Monster)) as Monster;
      if (_monster == null)
      {
         Debug.Log("Monster not found");
      }
   }

   void Update()
   {

      HealthBarText.text = _monster.CurrentHealth.ToString() + "/" + _monster.maxHealth.ToString();
      HealthBarSlider.value = _monster.CurrentHealth / 100f;
      HealthBarSlider.maxValue = _monster.maxHealth;

   }
   
}
