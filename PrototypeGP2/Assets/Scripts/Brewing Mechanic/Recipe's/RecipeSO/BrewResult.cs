using System;
using UnityEngine;

[Serializable]
public class BrewResult
{
    public string           variantKey;
    public PotionEffectType effect;
    public Sprite           icon;

    // Keywords from the ingredients used to brew this potion
    public MainKeyword mainKeyword;
    public BaseKeyword baseKeyword;
    public bool     instant;
    public int      turns;
    public float    percentOfMaxHP;
    public float    damage;
    public float    defence;
    public int      multiplier;

}
