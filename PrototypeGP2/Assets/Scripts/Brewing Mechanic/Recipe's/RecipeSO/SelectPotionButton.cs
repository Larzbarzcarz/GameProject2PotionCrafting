using UnityEngine;

public class SelectPotionButton : MonoBehaviour
{
    [SerializeField] private PotionThrowController throwController;
    [SerializeField] private PotionBaseSO potionToSelect;

    public void Select()
    {
        if (throwController == null || potionToSelect == null)
        {
            Debug.LogError("[UI] Missing throwController or potionToSelect.");
            return;
        }

        throwController.SelectPotion(potionToSelect);
    }
}
