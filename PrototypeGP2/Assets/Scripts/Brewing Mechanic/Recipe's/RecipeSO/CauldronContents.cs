using UnityEngine;
using System.Collections.Generic;

public class CauldronContents : MonoBehaviour
{
    private List<string> sequence = new();
    [SerializeField] private int maxIngredients = 2;

    public IReadOnlyList<string> Sequence => sequence;

    public bool AddIngredients(string stableId)
    {
        if (sequence.Count >= maxIngredients)
        {
            Debug.Log("[Cauldron] Full (2 ingredients already)!");
            return false;
        }

        sequence.Add(stableId);
        Debug.Log($"[Cauldron] Added: {stableId}.Count={sequence.Count}");
        return true;
    }

    public void Clear() => sequence.Clear();

    public bool TryPopLast(out string stableId)
    {
        stableId = null;
        if (sequence.Count == 0)
            return false;

        int last = sequence.Count - 1;
        stableId = sequence[last];
        sequence.RemoveAt(last);
        return true;
    }
}
