using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    //[field: Header("Ambience")]
    ////Not used at the moment, but could be used for things like spooky ambience.
    //[field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference baseMusic { get; private set; }
    [field: SerializeField] public EventReference CookMusic { get; private set; }
    [field: SerializeField] public EventReference CombatMusic { get; private set; }
    [field: SerializeField] public EventReference mainMenu { get; private set; }

    [field: Header("Menu SFX")]
    [field: SerializeField] public EventReference buttonPress { get; private set; }
    [field: SerializeField] public EventReference Cooking { get; private set; }
    [field: SerializeField] public EventReference expeditionStart { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerAttack1 { get; private set; }
    [field: SerializeField] public EventReference playerAttack2 { get; private set; }
    [field: SerializeField] public EventReference playerHurt { get; private set; }
    [field: SerializeField] public EventReference playerDead { get; private set; }
    [field: SerializeField] public EventReference playerSpecial { get; private set; }

    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference enemyAttack1 { get; private set; }
    [field: SerializeField] public EventReference enemyAttack2 { get; private set; }
    [field: SerializeField] public EventReference enemyHurt { get; private set; }
    [field: SerializeField] public EventReference enemyDead { get; private set; }
    [field: SerializeField] public EventReference enemySpecial { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}