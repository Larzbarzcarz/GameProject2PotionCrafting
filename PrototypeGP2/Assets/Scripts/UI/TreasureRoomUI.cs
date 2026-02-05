using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class TreasureRoomUI : MonoBehaviour
{
    public GameObject treasurePanel;
    public TextMeshProUGUI lootListText;
    public Button returnButton;

    private void Start()
    {
        treasurePanel.SetActive(false);
        returnButton.onClick.AddListener(OnReturnToLab);
    }

    public void ShowTreasure(List<Item> loot)
    {
        Debug.Log("DEBUG: ShowTreasure called!");

        if (treasurePanel == null) Debug.LogError("DEBUG: treasurePanel is NOT assigned in Inspector!");
        else Debug.Log($"DEBUG: Activating treasurePanel (Current active state: {treasurePanel.activeSelf})");

        if (lootListText == null) Debug.LogError("DEBUG: lootListText is NOT assigned in Inspector!");

        if (treasurePanel != null) treasurePanel.SetActive(true);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Expedition Success!");
        sb.AppendLine("Rewards found:");

        foreach (var item in loot)
        {
            sb.AppendLine($"- {item.Name}");
        }

        if (lootListText != null) lootListText.text = sb.ToString();
    }

    private void OnReturnToLab()
    {
        if (EncounterManager.Instance != null)
        {
            EncounterManager.Instance.EndRun(true);
        }
    }
}
