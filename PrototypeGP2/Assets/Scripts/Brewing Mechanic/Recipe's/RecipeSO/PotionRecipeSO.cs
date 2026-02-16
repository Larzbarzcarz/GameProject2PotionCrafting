using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum PotionEffectType { Heal, Damage, Utility, Special, Fail }

[CreateAssetMenu(fileName = "New Recipe", menuName = "Potions/Recipe Map")]
public class PotionRecipeSO : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public MainKeyword mainKey;
        public BaseKeyword baseKey;

        public PotionEffectType effectType;

        //remember to add same variables to BrewResult.cs and PotionBrewSystem -> TryBrew();
        [Header("Potion Effects")]
        public bool     instant;
        public int      turns;
        public float    percentOfMaxHP;
        public float    damage;
        public float    defence;
        public int      multiplier;

        [TextArea]
        public string effectDescription;
        public Sprite icon;
    }

    public List<Entry> entries = new();
    private Dictionary<(MainKeyword, BaseKeyword), Entry> lookup;

    private void OnEnable()
    {
        lookup = new();
        foreach (var e in entries)
            lookup[(e.mainKey, e.baseKey)] = e;
    }

    public bool TryGet(MainKeyword mainKey, BaseKeyword baseKey, out Entry entry)
    {
        if (lookup == null)
            OnEnable();
        return lookup.TryGetValue((mainKey, baseKey), out entry);
    }

}
