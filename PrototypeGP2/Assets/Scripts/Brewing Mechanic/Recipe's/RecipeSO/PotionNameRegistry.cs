using System;
using System.Collections.Generic;
using UnityEngine;

public class PotionNameRegistry : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string variantKey;
        public string customName;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();
    private Dictionary<string, string> map;

    private void Awake()
    {
        map = new Dictionary<string, string>();
        foreach (var e in entries)
            if (!string.IsNullOrEmpty(e.variantKey))
                map[e.variantKey] = e.customName;
    }

    public bool HasName(string variantKey) => map != null && map.ContainsKey(variantKey);

    public string GetName(string variantKey, string fallback)
    {
        if (map != null && map.TryGetValue(variantKey, out var n) && !string.IsNullOrEmpty(n))
            return n;
        return fallback;
    }

    public void SetName(string variantKey, string name)
    {
        if (map == null)
            map = new Dictionary<string, string>();
        map[variantKey] = name;

        int idx = entries.FindIndex(x => x.variantKey == variantKey);
        if (idx >= 0)
            entries[idx].customName = name;
        else entries.Add(new Entry { variantKey = variantKey, customName = name });
    }

    public void RenamePotion(string variantKey, string newName)
    {
        SetName(variantKey, newName);
    }
}
    