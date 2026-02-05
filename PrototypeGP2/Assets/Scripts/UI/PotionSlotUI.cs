using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// 
public class PotionSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI potionNameText;
    [SerializeField] private Image potionIcon;
    [SerializeField] private Image selectionIndicator;
    [SerializeField] private Button button;
    
    public Item Potion { get; private set; }
    
    private Action<PotionSlotUI> onClickCallback;
    private bool isSelected;
    
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
            
        if (button != null)
            button.onClick.AddListener(OnClick);
    }
    
    public void Setup(Item potion, Action<PotionSlotUI> onClick)
    {
        Potion = potion;
        onClickCallback = onClick;
        
        if (potionNameText != null)
            potionNameText.text = potion.Name ?? "Unknown Potion";
            
        SetSelected(false);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectionIndicator != null)
            selectionIndicator.enabled = selected;
            
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = selected ? new Color(0.5f, 1f, 0.5f) : Color.white;
            button.colors = colors;
        }
    }
    
    private void OnClick()
    {
        onClickCallback?.Invoke(this);
    }
}
