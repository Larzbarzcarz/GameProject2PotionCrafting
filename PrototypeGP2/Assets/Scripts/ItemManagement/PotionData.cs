/// runtime representation of crafted potion
/// created from two ingredients — first ingredient provides MainKeyword, second provides BaseKeyword
/// not a ScriptableObject; purely in-memory data
[System.Serializable]
public class PotionData
{
    public MainKeyword mainKeyword;
    public BaseKeyword baseKeyword;
    public PotionEffectInfo effect;
    public string playerName;  // default "Unknown Potion", player can rename

    /// <summary>
    /// variant key used by the mutation system, e.g. "Blood_Mineral"
    /// </summary>
    public string VariantKey => $"{mainKeyword}_{baseKeyword}";

    public PotionData(MainKeyword main, BaseKeyword baseKw)
    {
        mainKeyword = main;
        baseKeyword = baseKw;
        effect = PotionEffectTable.Lookup(main, baseKw);
        playerName = "Unknown Potion";
    }

    public override string ToString()
    {
        return $"[{playerName}] ({mainKeyword}+{baseKeyword}) -> {effect.effectId}: {effect.description}";
    }
}
