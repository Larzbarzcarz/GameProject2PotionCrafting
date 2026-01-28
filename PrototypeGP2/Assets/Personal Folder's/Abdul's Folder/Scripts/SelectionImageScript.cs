using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Selectionimagescript : MonoBehaviour
{
    public Image selectionboxthing1;
    public Image selectionboxthing2;

    void Start()
    {
        InvokeRepeating(nameof(Switch), 0f, 0.5f);
    }

    void Switch()
    {
        selectionboxthing1.enabled = !selectionboxthing1.enabled;
        selectionboxthing2.enabled = !selectionboxthing2.enabled;
    }
}