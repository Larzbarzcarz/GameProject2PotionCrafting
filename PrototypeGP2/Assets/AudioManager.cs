using FMODUnity;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public EventReference[] MusicLibrary;
    public EventReference[] SFXLibary;
    private Coroutine fadeCoroutine;
    public GameObject BreakingBad;

    FMOD.Studio.EventInstance BaseMusic;

    private float currentIntensity = 0;
    public float newIntensity;
    


    private void Start()
    {
        //-----------------------I am trying to automate the fmod sound reference, but it is not working, so I will just assign them in the inspector for now.-----------------------//
        //MusicLibrary[0] = EventReference.Find("event:/Music/Huldra");

        //SFXLibary[0] = EventReference.Find("event:/SFX/Button_press_1_Confirm_Back");
        //SFXLibary[1] = EventReference.Find("event:/SFX/potion-brew");
        //SFXLibary[2] = EventReference.Find("event:/SFX/Close_menu");
        //SFXLibary[3] = EventReference.Find("event:/SFX/Area_unlocked_omnious");


        BreakingBad = GameObject.Find("BrewMusic");
        BreakingBad.SetActive(false);
        BaseMusic = RuntimeManager.CreateInstance(MusicLibrary[0]);
        BaseMusic.start();
        currentIntensity = newIntensity;
        BaseMusic.setParameterByName("Intensity", currentIntensity);

    }

    public void PlaySound(int index)
    {
        RuntimeManager.PlayOneShot(SFXLibary[index], transform.position);
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
                BaseMusic.setPaused(false); ;
            }
        }
        else if (index == 1)
        {
            BreakingBad.SetActive(true);
            BaseMusic.setPaused(true);
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
}