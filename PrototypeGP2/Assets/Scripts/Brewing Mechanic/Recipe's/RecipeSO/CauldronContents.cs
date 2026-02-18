using UnityEngine;
using System.Collections.Generic;

public class CauldronContents : MonoBehaviour
{
    private List<string> sequence = new();
    [SerializeField] private int maxIngredients = 2;

    public IReadOnlyList<string> Sequence => sequence;

    public bool AddIngredients(string StableId)
    {
        if (sequence.Count >= maxIngredients)
        {
            Debug.Log("[Cauldron] Full (2 ingredients already)!");
            return false;
        }

        sequence.Add(StableId);
        Debug.Log($"[Cauldron] Added: {StableId}.Count={sequence.Count}");
        return true;
    }

    public void Clear() => sequence.Clear();

    public bool TryPopLast(out string StableId)
    {
        StableId = null;
        if (sequence.Count == 0)
            return false;

        int last = sequence.Count - 1;
        StableId = sequence[last];
        sequence.RemoveAt(last);
        return true;
    }
	
	private void OnTriggerEnter(Collider other)
{
    var pickup = other.GetComponent<PickupItems>();
    if (pickup == null)
        return;

 Debug.Log($"[Cauldron] Item entered. item null? {pickup.item == null}");
Debug.Log($"[Cauldron] StableId: '{pickup.StableId}'");

if (AddIngredients(pickup.StableId))
{
    Destroy(other.gameObject);
}

}

}
