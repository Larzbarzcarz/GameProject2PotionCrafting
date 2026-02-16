using System;
using System.Collections.Generic;
using UnityEngine;

public class PotionVariantRegistry : MonoBehaviour
{
    [SerializeField] private List<BrewResult> variants = new();
    private Dictionary<string, BrewResult> lookup;

    private void Awake() => Build();

    private void Build()
    {
        lookup = new Dictionary<string, BrewResult>();
        foreach (var v in variants)
        {
            if (string.IsNullOrEmpty(v.variantKey))
                continue;
            lookup[v.variantKey] = v;
        }
    }

    public bool TryGet(string variantKey, out BrewResult result)
    {
        if (lookup == null)
            Build();
        return lookup.TryGetValue(variantKey, out result);
    }

    public BrewResult GetOrCreate(string variantKey)
    {
        if (lookup == null)
            Build();

        if (lookup.TryGetValue(variantKey, out var existing))
            return existing;

        var r = new BrewResult { variantKey = variantKey };
        variants.Add(r);
        lookup[variantKey] = r;
        return r;
    }
}
