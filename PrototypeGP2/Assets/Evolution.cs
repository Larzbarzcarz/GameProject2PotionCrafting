using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject Spine;
    public GameObject Arms;
    public GameObject LeftShoulder;

    public GameObject Mushroomneck;
    public GameObject MushroomLeftShoulder;
    public GameObject MushroomRightShoulder;

    public GameObject SecoundHead;

    public void Awake()
    {
        HideAll();
        ApplyMutations();
    }

    private void OnEnable()
    {
        if (MutationRegistry.Instance != null)
            MutationRegistry.Instance.OnMutationUnlocked += OnMutationUnlocked;
    }

    private void OnDisable()
    {
        if (MutationRegistry.Instance != null)
            MutationRegistry.Instance.OnMutationUnlocked -= OnMutationUnlocked;
    }

    private void OnMutationUnlocked(BaseKeyword keyword, int tier)
    {
        if (tier >= 1)
            ActivateMutation(keyword);
    }

    public void ApplyMutations()
    {
        MutationData.Load();

        if (MutationData.GetTier(BaseKeyword.Animal) >= 1)
            ActivateMutation(BaseKeyword.Animal);

        if (MutationData.GetTier(BaseKeyword.Mineral) >= 1)
            ActivateMutation(BaseKeyword.Mineral);

        if (MutationData.GetTier(BaseKeyword.Fungus) >= 1)
            ActivateMutation(BaseKeyword.Fungus);

        if (MutationData.GetTier(BaseKeyword.Cursed) >= 1)
            ActivateMutation(BaseKeyword.Cursed);
    }

    public void ActivateMutation(BaseKeyword keyword)
    {
        switch (keyword)
        {
            case BaseKeyword.Animal:
                if (LeftShoulder != null) LeftShoulder.SetActive(true);
                if (Arms != null) Arms.SetActive(true);
                break;

            case BaseKeyword.Mineral:
                if (Spine != null) Spine.SetActive(true);
                break;

            case BaseKeyword.Fungus:
                if (Mushroomneck != null) Mushroomneck.SetActive(true);
                if (MushroomLeftShoulder != null) MushroomLeftShoulder.SetActive(true);
                if (MushroomRightShoulder != null) MushroomRightShoulder.SetActive(true);
                break;

            case BaseKeyword.Cursed:
                if (SecoundHead != null) SecoundHead.SetActive(true);
                break;
        }
    }

    public void DeactivateMutation(BaseKeyword keyword)
    {
        switch (keyword)
        {
            case BaseKeyword.Animal:
                if (LeftShoulder != null) LeftShoulder.SetActive(false);
                if (Arms != null) Arms.SetActive(false);
                break;

            case BaseKeyword.Mineral:
                if (Spine != null) Spine.SetActive(false);
                break;

            case BaseKeyword.Fungus:
                if (Mushroomneck != null) Mushroomneck.SetActive(false);
                if (MushroomLeftShoulder != null) MushroomLeftShoulder.SetActive(false);
                if (MushroomRightShoulder != null) MushroomRightShoulder.SetActive(false);
                break;

            case BaseKeyword.Cursed:
                if (SecoundHead != null) SecoundHead.SetActive(false);
                break;
        }
    }

    private void HideAll()
    {
        if (Spine != null) Spine.SetActive(false);
        if (Arms != null) Arms.SetActive(false);
        if (LeftShoulder != null) LeftShoulder.SetActive(false);
        if (Mushroomneck != null) Mushroomneck.SetActive(false);
        if (MushroomLeftShoulder != null) MushroomLeftShoulder.SetActive(false);
        if (MushroomRightShoulder != null) MushroomRightShoulder.SetActive(false);
        if (SecoundHead != null) SecoundHead.SetActive(false);
    }
}
