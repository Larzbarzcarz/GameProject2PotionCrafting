using FMODUnity;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private EventReference[] MusicLibrary;
    private Clickforcamera clickforcamera;

    FMOD.Studio.EventInstance currentMusic;

    private void Start()
    {
        clickforcamera = FindFirstObjectByType<Clickforcamera>();
        currentMusic = FMODUnity.RuntimeManager.CreateInstance(MusicLibrary[0]);
        currentMusic.start();
        
    }
    
    //public void ChangeMusic(int index)
    //{
    //    currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    //    currentMusic.release();
    //    currentMusic = FMODUnity.RuntimeManager.CreateInstance(MusicLibrary[index]);
    //    currentMusic.start();
    //}

    //private void OnDestroy()
    //{
    //    currentMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //    currentMusic.release();
    //}

}