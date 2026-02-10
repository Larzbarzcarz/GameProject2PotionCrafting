using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MutationData
{
    private static readonly string SaveFileName = "mutation_data.json";
    private static readonly int[] TierThresholds = { 3, 6, 9 };

    private static Dictionary<BaseKeyword, int> keywordCounts = new();
    private static Dictionary<BaseKeyword, int> unlockedTiers = new();

    private static bool isLoaded = false;

    public static int GetCount(BaseKeyword keyword)
    {
        EnsureLoaded();
        return keywordCounts.TryGetValue(keyword, out int count) ? count : 0;
    }

    public static int GetTier(BaseKeyword keyword)
    {
        EnsureLoaded();
        return unlockedTiers.TryGetValue(keyword, out int tier) ? tier : 0;
    }

    public static bool IncrementKeyword(BaseKeyword keyword, out int newTier)
    {
        EnsureLoaded();

        if (!keywordCounts.ContainsKey(keyword))
            keywordCounts[keyword] = 0;
        
        keywordCounts[keyword]++;
        int currentCount = keywordCounts[keyword];

        int previousTier = GetTier(keyword);
        newTier = previousTier;

        for (int i = 0; i < TierThresholds.Length; i++)
        {
            int tierLevel = i + 1;
            if (currentCount >= TierThresholds[i] && previousTier < tierLevel)
            {
                newTier = tierLevel;
                unlockedTiers[keyword] = tierLevel;
            }
        }

        Save();

        bool tierUnlocked = newTier > previousTier;
        if (tierUnlocked)
        {
            Debug.Log($"[MutationData] TIER {newTier} UNLOCKED for {keyword}!");
        }

        return tierUnlocked;
    }

    public static void LogStatus()
    {
        EnsureLoaded();
        Debug.Log("=== MUTATION DATA STATUS ===");
        
        foreach (BaseKeyword keyword in Enum.GetValues(typeof(BaseKeyword)))
        {
            int count = GetCount(keyword);
            int tier = GetTier(keyword);
            if (count > 0 || tier > 0)
            {
                Debug.Log($"[MutationData] {keyword}: Count={count}, Tier={tier}");
            }
        }
        
        Debug.Log("=== END MUTATION STATUS ===");
    }

    public static void Reset()
    {
        keywordCounts.Clear();
        unlockedTiers.Clear();
        Save();
        Debug.Log("[MutationData] All mutation data reset.");
    }

    private static void EnsureLoaded()
    {
        if (!isLoaded)
            Load();
    }

    public static void Save()
    {
        var data = new MutationSaveData
        {
            entries = new List<MutationSaveData.Entry>()
        };

        foreach (BaseKeyword keyword in Enum.GetValues(typeof(BaseKeyword)))
        {
            int count = keywordCounts.TryGetValue(keyword, out int c) ? c : 0;
            int tier = unlockedTiers.TryGetValue(keyword, out int t) ? t : 0;

            if (count > 0 || tier > 0)
            {
                data.entries.Add(new MutationSaveData.Entry
                {
                    keyword = keyword.ToString(),
                    count = count,
                    tier = tier
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        
        File.WriteAllText(path, json);
        Debug.Log($"[MutationData] Saved to {path}");
    }

    public static void Load()
    {
        keywordCounts.Clear();
        unlockedTiers.Clear();
        isLoaded = true;

        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        
        if (!File.Exists(path))
        {
            Debug.Log("[MutationData] No save file found, starting fresh.");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<MutationSaveData>(json);

            foreach (var entry in data.entries)
            {
                if (Enum.TryParse<BaseKeyword>(entry.keyword, out var keyword))
                {
                    keywordCounts[keyword] = entry.count;
                    unlockedTiers[keyword] = entry.tier;
                }
            }

            Debug.Log($"[MutationData] Loaded from {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[MutationData] Failed to load: {e.Message}");
        }
    }
}

[Serializable]
public class MutationSaveData
{
    public List<Entry> entries = new();

    [Serializable]
    public class Entry
    {
        public string keyword;
        public int count;
        public int tier;
    }
}
