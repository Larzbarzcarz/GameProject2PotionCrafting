using _Project._Scripts.Sound_and_Music;
using FMODUnity;
using System.Threading;
using UnityEngine;

public class MonsterSound : MonoBehaviour
{
    [SerializeField] private int Enemy;

    public void PlayMonsterSoundHit()
    {
        Debug.Log("Playing monster hit sound for enemy type: " + Enemy);
        switch (Enemy)
        {
            case 0:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.treeImpact);
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.treeAttack);
                break;
            case 1:
                int randomBlobHurt = Random.Range(1, 2);
                if (randomBlobHurt == 1)
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.instance.blobAttack1);
                }
                else
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.instance.blobAttack2);
                }
                break;
            case 2:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.batAttack);
                break;
            default:
                Debug.LogWarning("Invalid enemy type for MonsterSound.");
                break;
        }
    }

    public void PlayMonsterSoundHurt()
    {
        Debug.Log("Playing monster hurt sound for enemy type: " + Enemy);
        switch (Enemy)
        {
            case 0:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.treeHurt);
                break;
            case 1:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.blobHurt);
                break;
            case 2:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.batHurt);
                break;
            default:
                Debug.LogWarning("Invalid enemy type for MonsterSound.");
                break;
        }
    }

    public void PlayMonsterSoundDead()
    {
        Debug.Log("Playing monster dead sound for enemy type: " + Enemy);
        switch (Enemy)
        {
            case 0:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.treeDead);
                break;
            case 1:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.blobDead);
                break;
            case 2:
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.batDead);
                break;
            default:
                Debug.LogWarning("Invalid enemy type for MonsterSound.");
                break;
        }
    }
}
