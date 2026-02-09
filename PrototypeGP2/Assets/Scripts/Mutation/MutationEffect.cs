using UnityEngine;

public abstract class MutationEffect
{
    public BaseKeyword Keyword { get; protected set; }
    public int Tier { get; protected set; }
    public bool IsActive { get; set; } = true;

    public virtual void OnBattleStart() { }
    public virtual void OnBattleEnd(bool victory, BattleSystem battle) { }

    public virtual float ModifyIncomingDamage(float damage, Combatant source) => damage;
    public virtual float ModifyOutgoingDamage(float damage, Combatant target) => damage;

    public virtual void OnAfterMonsterAttack(BattleSystem battle, Combatant target, float damageDealt) { }

    public virtual bool ShouldSkipTurn() => false;
}

public class CarapaceEffect : MutationEffect
{
    private bool isBroken = false;
    private bool isStunned = false;

    public CarapaceEffect(int tier)
    {
        Keyword = BaseKeyword.Mineral;
        Tier = tier;
    }

    public override void OnBattleStart()
    {
        isBroken = false;
        isStunned = false;
    }

    public override float ModifyIncomingDamage(float damage, Combatant source)
    {
        if (!IsActive || isBroken)
            return damage;

        if (damage > 25f)
        {
            isBroken = true;
            isStunned = true;
            return damage;
        }

        if (damage < 15f)
        {
            float reduced = Mathf.Ceil(damage / 2f);
            return reduced;
        }

        return damage;
    }

    public override bool ShouldSkipTurn()
    {
        if (isStunned)
        {
            isStunned = false;
            return true;
        }
        return false;
    }
}

public class BoneySpikesEffect : MutationEffect
{
    public BoneySpikesEffect(int tier)
    {
        Keyword = BaseKeyword.Animal;
        Tier = tier;
    }

    public override float ModifyOutgoingDamage(float damage, Combatant target)
    {
        if (!IsActive || target == null)
            return damage;

        float targetHP = target.CurrentHealth;

        if (targetHP < 30f)
        {
            return damage + 3f;
        }

        if (targetHP > 90f)
        {
            return Mathf.Max(0, damage - 2f);
        }

        return damage;
    }
}

public class TwinnedHeadEffect : MutationEffect
{
    public TwinnedHeadEffect(int tier)
    {
        Keyword = BaseKeyword.Cursed;
        Tier = tier;
    }

    public override void OnAfterMonsterAttack(BattleSystem battle, Combatant target, float damageDealt)
    {
        if (!IsActive || target == null || target.isDead)
            return;

        if (Random.value < 0.5f)
        {
            float secondDamage = damageDealt * 0.75f;
            
            target.TakeDamage(secondDamage);

            if (battle.monster != null)
            {
                float hpPercent = battle.monster.CurrentHealth / battle.monster.MaxHealth;
                if (hpPercent > 0.5f)
                {
                    battle.monster.TakeDamage(5f);
                }
            }
        }
    }
}

public class HelpfulInfectionEffect : MutationEffect
{
    public HelpfulInfectionEffect(int tier)
    {
        Keyword = BaseKeyword.Fungus;
        Tier = tier;
    }

    public override void OnBattleEnd(bool victory, BattleSystem battle)
    {
        if (!IsActive || !victory || MutationManager.Instance == null || battle == null)
            return;

        Item randomPotion = MutationManager.Instance.GetRandomConsumedPotion();
        if (randomPotion == null) return;

        string[] keywords = randomPotion.VariantKey.Split('_');
        if (keywords.Length < 2) return;

        string targetMain = keywords[0];
        string targetBase = keywords[1];

        bool giveMain = Random.value < 0.5f;
        string searchKeyword = giveMain ? targetMain : targetBase;

        InventoryObject inv = battle.PlayerInventory;
        if (inv == null || inv.database == null) return;

        foreach (var itemSO in inv.database.Items)
        {
            if (itemSO is IngredientObject ingredient)
            {
                bool match = giveMain ? 
                    ingredient.mainKeyword.ToString() == searchKeyword : 
                    ingredient.baseKeyword.ToString() == searchKeyword;

                if (match)
                {
                    Debug.Log($"[Helpful Infection] Gained ingredient reward: {ingredient.ItemName}");
                    inv.AddItem(new Item(ingredient), 1, "");
                    return;
                }
            }
        }
    }
}
