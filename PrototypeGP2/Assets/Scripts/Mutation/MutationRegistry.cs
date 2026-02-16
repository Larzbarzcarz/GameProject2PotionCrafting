using System;
using UnityEngine;

public class MutationRegistry : MonoBehaviour
{
    public static MutationRegistry Instance { get; private set; }

    public event Action<BaseKeyword, int> OnMutationUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        MutationData.Load();
    }

    public void OnPotionConsumed(string variantKey)
    {
        if (string.IsNullOrEmpty(variantKey))
            return;

        if (!TryParseBaseKeyword(variantKey, out BaseKeyword baseKeyword))
            return;

        OnPotionConsumed(baseKeyword);
    }

    public void OnPotionConsumed(BaseKeyword baseKeyword)
    {
        int previousCount = MutationData.GetCount(baseKeyword);
        
        bool tierUnlocked = MutationData.IncrementKeyword(baseKeyword, out int newTier);
        
        int newCount = MutationData.GetCount(baseKeyword);
        Debug.Log($"[MutationRegistry] Consumed potion with BaseKeyword: {baseKeyword}. Count: {previousCount} -> {newCount}");

        if (tierUnlocked)
        {
            Debug.Log($"[MutationRegistry] *** TIER {newTier} MUTATION UNLOCKED: {baseKeyword}! ***");
            
            if (MutationManager.Instance != null)
            {
                MutationManager.Instance.RefreshEffects();
            }

            OnMutationUnlocked?.Invoke(baseKeyword, newTier);
        }
    }

    private bool TryParseBaseKeyword(string variantKey, out BaseKeyword baseKeyword)
    {
        baseKeyword = default;

        string[] parts = variantKey.Split('_');
        if (parts.Length < 2)
            return false;

        string baseKeywordStr = parts[1];
        
        if (Enum.TryParse(baseKeywordStr, out baseKeyword))
        {
            return true;
        }

        return false;
    }

    public void LogMutationStatus()
    {
        MutationData.LogStatus();
    }

    public void ResetAllMutations()
    {
        MutationData.Reset();
    }
}
