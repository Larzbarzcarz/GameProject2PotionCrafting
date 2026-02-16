using _Project._Scripts.Sound_and_Music;
using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public void PlayClickSound()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
    }
}
