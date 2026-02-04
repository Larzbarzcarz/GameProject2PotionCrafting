using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionIconLibrary", menuName = "Potions/Icon Library")]
public class PotionIconLibrary : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public PotionEffectType effect;
        public Sprite icon;
    }

    public List<Entry> entries = new();

    public Sprite GetIcon(PotionEffectType effect, Sprite fallback)
    {
        foreach (var e in entries)
            if (e.effect == effect && e.icon != null)
                return e.icon;
        return fallback;
    }
}
