using System;
using System.Collections.Generic;
using UnityEngine;

public class PotionNameRegistry : MonoBehaviour
{
    [Serializable] private class Entry {  public string key; public string name; }
    [Serializable] private class SaveData { public List<Entry> entries = new(); }

    private Dictionary<string, string> names = new();
    private const string PlayerPrefsKey = "PotionNames_v1";

    private void Awake() => Load();

    public bool HasName(string variantKey) => names.ContainsKey(variantKey);

    public string GetName(string variantKey, string fallback)
        => names.TryGetValue(variantKey, out var n) ? n : fallback;

    public void SetName(string variantKey, string newName)
    {
        names[variantKey] = newName;
        Save();
    }

    public void Save()
    {
        var data = new SaveData();
        foreach (var kv in names)
            data.entries.Add(new Entry { key = kv.Key, name = kv.Value });

        PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public void Load()
    {
        names.Clear();
        if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            return;

        var json = PlayerPrefs.GetString(PlayerPrefsKey);
        var data = JsonUtility.FromJson<SaveData>(json);
        if (data?.entries == null)
            return;

        foreach (var e in data.entries)
            if (!string.IsNullOrEmpty(e.key))
                names[e.key] = e.name ?? "";
    }
}
