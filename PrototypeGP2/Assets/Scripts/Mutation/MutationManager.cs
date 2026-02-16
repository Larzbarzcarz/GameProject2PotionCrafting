using System.Collections.Generic;
using UnityEngine;

public class MutationManager : MonoBehaviour
{
    public static MutationManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Monster monster;
    public Monster Monster => monster;

    private List<MutationEffect> activeEffects = new();
    private List<Item> consumedPotionsThisBattle = new();
    private bool effectsInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeEffects();
    }

    public void InitializeEffects()
    {
        activeEffects.Clear();

        int mineralTier = MutationData.GetTier(BaseKeyword.Mineral);
        if (mineralTier >= 1)
        {
            activeEffects.Add(new CarapaceEffect(mineralTier));
        }

        int animalTier = MutationData.GetTier(BaseKeyword.Animal);
        if (animalTier >= 1)
        {
            activeEffects.Add(new BoneySpikesEffect(animalTier));
        }

        int cursedTier = MutationData.GetTier(BaseKeyword.Cursed);
        if (cursedTier >= 1)
        {
            activeEffects.Add(new TwinnedHeadEffect(cursedTier));
        }

        int fungusTier = MutationData.GetTier(BaseKeyword.Fungus);
        if (fungusTier >= 1)
        {
            activeEffects.Add(new HelpfulInfectionEffect(fungusTier));
        }

        effectsInitialized = true;
    }

    public void OnBattleStart()
    {
        if (!effectsInitialized) InitializeEffects();

        consumedPotionsThisBattle.Clear();

        foreach (var effect in activeEffects)
        {
            effect.OnBattleStart();
        }
    }

    public void OnBattleEnd(bool victory, BattleSystem battle)
    {
        foreach (var effect in activeEffects)
        {
            effect.OnBattleEnd(victory, battle);
        }
    }

    public float ProcessIncomingDamage(float damage, Combatant source)
    {
        foreach (var effect in activeEffects)
        {
            damage = effect.ModifyIncomingDamage(damage, source);
        }
        return damage;
    }

    public float ProcessOutgoingDamage(float damage, Combatant target)
    {
        foreach (var effect in activeEffects)
        {
            damage = effect.ModifyOutgoingDamage(damage, target);
        }
        return damage;
    }

    public void OnAfterMonsterAttack(BattleSystem battle, Combatant target, float damageDealt)
    {
        foreach (var effect in activeEffects)
        {
            effect.OnAfterMonsterAttack(battle, target, damageDealt);
        }
    }

    public void RecordPotionConsumption(Item potion)
    {
        consumedPotionsThisBattle.Add(potion);
    }

    public Item GetRandomConsumedPotion()
    {
        if (consumedPotionsThisBattle.Count == 0) return null;
        return consumedPotionsThisBattle[Random.Range(0, consumedPotionsThisBattle.Count)];
    }

    public bool ShouldMonsterSkipTurn()
    {
        foreach (var effect in activeEffects)
        {
            if (effect.ShouldSkipTurn())
                return true;
        }
        return false;
    }

    public void RefreshEffects()
    {
        InitializeEffects();
    }
}
