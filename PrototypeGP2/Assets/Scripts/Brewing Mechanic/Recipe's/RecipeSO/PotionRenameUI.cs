using UnityEngine;
using TMPro;

public class PotionRenameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PotionNameRegistry nameRegistry;

    [Header("Prompt panel Yes/No")]
    [SerializeField] private GameObject promptPanel;

    [Header("Rename Panel (input)")]
    [SerializeField] private GameObject renamePanel;
    [SerializeField] private TMP_InputField renameInput;

    private string currentVariantKey;

    private void Awake()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        if (renamePanel != null) renamePanel.SetActive(false);
    }

    public void StartRename(string variantKey)
    {
        currentVariantKey = variantKey;

        if (promptPanel != null)
            promptPanel.SetActive(true);
    }

    public void Yes()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        if (renamePanel != null) renamePanel.SetActive(true);

        if (nameRegistry != null && renameInput != null)
        {
            renameInput.text = nameRegistry.GetName(currentVariantKey, "Unnamed Potion");
            renameInput.Select();
            renameInput.ActivateInputField();
        }
    }

    public void No()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        currentVariantKey = null;
    }

    public void RenameOK()
    {
        if (string.IsNullOrEmpty(currentVariantKey))
            return;

        string newName = renameInput != null ? renameInput.text.Trim() : "";
        if (string.IsNullOrEmpty(newName))
            return;


        nameRegistry.SetName(currentVariantKey, newName);

        CloseRenamePanel();
    }

    public void RenameCancel()
    {
        CloseRenamePanel();
    }

    private void CloseRenamePanel()
    {
        if (renamePanel != null) renamePanel.SetActive(false);
        currentVariantKey = null;
    }
}
