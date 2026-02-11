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
    [field: SerializeField] public EventReference cookMusic { get; private set; }
    [field: SerializeField] public EventReference combatMusic { get; private set; }
    [field: SerializeField] public EventReference victoryMusic { get; private set; }

    [field: Header("Menu SFX")]
    [field: SerializeField] public EventReference buttonPress { get; private set; }
    [field: SerializeField] public EventReference Cooking { get; private set; }
    [field: SerializeField] public EventReference expeditionStart { get; private set; }
    [field: SerializeField] public EventReference victory { get; private set; }
    [field: SerializeField] public EventReference loss { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerAttack1 { get; private set; }
    [field: SerializeField] public EventReference playerAttack2 { get; private set; }
    [field: SerializeField] public EventReference playerAttack3 { get; private set; }
    [field: SerializeField] public EventReference playerHurt1 { get; private set; }
    [field: SerializeField] public EventReference playerHurt2 { get; private set; }
    [field: SerializeField] public EventReference playerHurt3 { get; private set; }
    [field: SerializeField] public EventReference playerDead { get; private set; }
    [field: SerializeField] public EventReference playerIdle { get; private set; }

    [field: Header("BlobE SFX")]
    [field: SerializeField] public EventReference blobAttack1 { get; private set; }
    [field: SerializeField] public EventReference blobAttack2 { get; private set; }
    [field: SerializeField] public EventReference blobHurt { get; private set; }
    [field: SerializeField] public EventReference blobDead { get; private set; }
    [field: SerializeField] public EventReference blobSpecial { get; private set; }

    [field: Header("TreeE SFX")]
    [field: SerializeField] public EventReference treeAttack { get; private set; }
    [field: SerializeField] public EventReference treeImpact { get; private set; }
    [field: SerializeField] public EventReference treeDead { get; private set; }
    [field: SerializeField] public EventReference treeHurt { get; private set; }

    [field: Header("BatE SFX")]
    [field: SerializeField] public EventReference batAttack { get; private set; }
    [field: SerializeField] public EventReference batIdle { get; private set; }
    [field: SerializeField] public EventReference batDead { get; private set; }
    [field: SerializeField] public EventReference batHurt { get; private set; }
    [field: SerializeField] public EventReference batNoise { get; private set; }




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