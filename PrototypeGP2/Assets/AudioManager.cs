using FMODUnity;
using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private EventReference[] MusicLibrary;
    [SerializeField] private EventReference[] SFXLibary;
    [SerializeField] private GameObject BreakingBad;
    FMOD.Studio.EventInstance BaseMusic;
    //FMOD.Studio.EventInstance BrewingMusic;
    private float currentIntensity = 0;
    public float newIntensity;
    private Coroutine fadeCoroutine;


    private void Start()
    {
        BaseMusic = FMODUnity.RuntimeManager.CreateInstance(MusicLibrary[0]);
        BreakingBad.SetActive(false);
        BaseMusic.start();
        currentIntensity = newIntensity;


        //MusicLibrary[0] = EventReference.Find("event:/Music/Huldra");

        //SFXLibary[0] = EventReference.Find("event:/SFX/Button_press_1_Confirm_Back");
        //SFXLibary[1] = EventReference.Find("event:/SFX/potion-brew");
        //SFXLibary[2] = EventReference.Find("event:/SFX/Close_menu");
        //SFXLibary[3] = EventReference.Find("event:/SFX/Area_unlocked_omnious");

        Debug.Log("help " + MusicLibrary[0]);
        Debug.Log("helppppppppppppppppppppppppppppppppppppppppppppp ");
        
    }

    public void PlaySound(int index)
    {
        FMODUnity.RuntimeManager.PlayOneShot(SFXLibary[index], transform.position);
    }

    public void StopMusic()
    {
        BaseMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BaseMusic.release();
    }

    public void ChangeMusic(int index)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (index == 0)
        {
            IntensityChangeCoroutine();
            Debug.Log(BreakingBad.activeSelf);
            if (BreakingBad.activeSelf)
            {
                BreakingBad.SetActive(false);
                fadeCoroutine = StartCoroutine(InNFadeOutCoroutine(BaseMusic, 1f, 0));
            }
        }
        else if (index == 1)
        {
            BreakingBad.SetActive(true);
            fadeCoroutine = StartCoroutine(InNFadeOutCoroutine(BaseMusic, 1f, 1));
        }
    }

    #region Intensity

    public void IntensityChange(int Inte)
    {
        newIntensity += Inte * 0.5f;
    }

    private IEnumerator IntensityChangeCoroutine()
    {
        while (currentIntensity != newIntensity)
        {
            currentIntensity = Mathf.MoveTowards(currentIntensity, newIntensity, Time.deltaTime * 0.5f);
            BaseMusic.setParameterByName("Intensity", currentIntensity);
            yield return null;
        }
    }

    #endregion Intensity

    #region FadeInOut
    private IEnumerator InNFadeOutCoroutine(FMOD.Studio.EventInstance music, float duration, int InOut)
    {
        if (InOut == 1) // Fade Out
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newVolume = Mathf.Lerp(1f, 0f, elapsed / duration);
                music.setVolume(newVolume);
                yield return null;
            }

            music.setVolume(0f);
            music.setPaused(true);
        }
        else // Fade In
        {
            StartCoroutine(IntensityChangeCoroutine());
            music.setPaused(false);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newVolume = Mathf.Lerp(0f, 1f, elapsed / duration);
                music.setVolume(newVolume);
                yield return null;
            }
            
            music.setVolume(1f);
           
        }
        #endregion FadeInOut

    }
}