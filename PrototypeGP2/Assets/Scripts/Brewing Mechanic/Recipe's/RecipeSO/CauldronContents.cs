using UnityEngine;
using System.Collections.Generic;

public class CauldronContents : MonoBehaviour
{
    private List<string> sequence = new();

    public IReadOnlyList<string> Sequence => sequence;

    public void AddIngredients(string stableId) => sequence.Add(stableId);

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
