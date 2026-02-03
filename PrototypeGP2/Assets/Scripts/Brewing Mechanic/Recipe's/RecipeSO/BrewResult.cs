using System;
using UnityEngine;

[Serializable]
public struct BrewResult
{
    public string           variantKey;
    public int              seed;
    public PotionEffectType effect;
    public float            potency;
    public float            duration;
    public Sprite           icon;
}
