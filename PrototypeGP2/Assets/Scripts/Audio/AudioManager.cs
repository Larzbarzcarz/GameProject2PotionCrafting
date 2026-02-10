using _Project._Scripts.Interfaces;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace _Project._Scripts.Sound_and_Music
{
    public class AudioManager : MonoBehaviour, IAudioService
    {
        public static AudioManager Instance { get; private set; }

        [Header("FMOD Bus Paths")]
        [Tooltip("FMOD Master bus path (usually 'bus:/')")]
        [SerializeField] private string masterBusPath = "bus:/";
        [Tooltip("FMOD Music bus path")]
        [SerializeField] private string musicBusPath = "bus:/Music";
        [Tooltip("FMOD SFX bus path")]
        [SerializeField] private string sfxBusPath = "bus:/SFX";

        private Bus _masterBus;
        private Bus _musicBus;
        private Bus _sfxBus;
        private bool _hasMasterBus;
        private bool _hasMusicBus;
        private bool _hasSfxBus;

        private EventInstance _currentMusicInstance;
        private bool _isMusicPlaying = false;

        // --- Huldra-specific fields ---
        private EventInstance _huldraInstance;
        private bool _huldraPaused = false;
        private const string HuldraPath = "event:/Music/Huldra";
        private float Intecity;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[SoundManagerFmod] Duplicate instance detected. Destroying.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cache FMOD buses (safe — won't crash if buses don't exist yet)
            InitializeBuses();

            Debug.Log("[SoundManagerFmod] Registered as IAudioService and persistent.");
        }

        private void InitializeBuses()
        {
            try { _masterBus = RuntimeManager.GetBus(masterBusPath); _hasMasterBus = true; }
            catch { Debug.LogWarning($"[SoundManagerFmod] Master bus '{masterBusPath}' not found. Master volume control disabled."); }

            try { _musicBus = RuntimeManager.GetBus(musicBusPath); _hasMusicBus = true; }
            catch { Debug.LogWarning($"[SoundManagerFmod] Music bus '{musicBusPath}' not found. Music volume control disabled."); }

            try { _sfxBus = RuntimeManager.GetBus(sfxBusPath); _hasSfxBus = true; }
            catch { Debug.LogWarning($"[SoundManagerFmod] SFX bus '{sfxBusPath}' not found. SFX volume control disabled."); }
        }

        private void Start()
        {
            // Subscribe to volume changes from SettingsManager
            //if (SettingsManager.Instance != null)
            //{
            //    SettingsManager.Instance.OnMasterVolumeChanged += SetMasterVolume;
            //    SettingsManager.Instance.OnMusicVolumeChanged += SetMusicVolume;
            //    SettingsManager.Instance.OnSFXVolumeChanged += SetSFXVolume;

            //    // Apply saved volumes immediately
            //    var settings = SettingsManager.Instance.CurrentSettings;
            //    SetMasterVolume(settings.masterVolume);
            //    SetMusicVolume(settings.musicVolume);
            //    SetSFXVolume(settings.sfxVolume);
            //}
            //else
            //{
            //    Debug.LogWarning("[SoundManagerFmod] SettingsManager not found. Using default volumes.");
            //}
        }
        private void OnDestroy()
        {
            //if (Instance == this)
            //{
            //    // Unsubscribe from events
            //    if (SettingsManager.Instance != null)
            //    {
            //        SettingsManager.Instance.OnMasterVolumeChanged -= SetMasterVolume;
            //        SettingsManager.Instance.OnMusicVolumeChanged -= SetMusicVolume;
            //        SettingsManager.Instance.OnSFXVolumeChanged -= SetSFXVolume;
            //    }

            //    StopMusic();
            //    ServiceLocator.UnregisterService<IAudioService>(this);
            //    Instance = null;
            //}
        }


      // ── Volume Control ────────────────────────────────────────────

        private void SetMasterVolume(float volume)
        {
            if (_hasMasterBus) _masterBus.setVolume(volume);
        }

        private void SetMusicVolume(float volume)
        {
            if (_hasMusicBus) _musicBus.setVolume(volume);
        }

        private void SetSFXVolume(float volume)
        {
            if (_hasSfxBus) _sfxBus.setVolume(volume);
        }

        // ── IAudioService Implementation ──────────────────────────────

        public void PlayOneShot(EventReference sound)
        {
            if (sound.IsNull) return;
            RuntimeManager.PlayOneShot(sound);
        }

        public void PlayOneShotAtPosition(EventReference sound, Vector3 position)
        {
            if (sound.IsNull) return;
            RuntimeManager.PlayOneShot(sound, position);
        }

        public void PlayMusic(EventReference music)
        {
            if (music.IsNull) return;

            // If switching to Huldra, stop any non-Huldra music
            if (music.Path == HuldraPath)
            {
                // Stop currently playing non-Huldra music
                if (_currentMusicInstance.isValid())
                {
                    _currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    _currentMusicInstance.release();
                }

                // If Huldra is already playing and paused, resume it
                if (_huldraInstance.isValid())
                {
                    _huldraInstance.setPaused(false);
                }
                else
                {
                    // Create and start Huldra if not already created
                    _huldraInstance = RuntimeManager.CreateInstance(music);
                    _huldraInstance.start();
                }
                _huldraPaused = false;
                _isMusicPlaying = true;
                return;
            }
            else
            {
                // If Huldra is playing, pause it
                if (_huldraInstance.isValid() && !_huldraPaused)
                {
                    _huldraInstance.setPaused(true);
                    _huldraPaused = true;
                }
            }

            // Stop any currently playing non-Huldra music
            if (_currentMusicInstance.isValid())
            {
                _currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentMusicInstance.release();
            }

            // Play the requested non-Huldra music
            _currentMusicInstance = RuntimeManager.CreateInstance(music);
            _currentMusicInstance.start();
            _isMusicPlaying = true;
        }

        public void StopMusic()
        {
            if (_isMusicPlaying)
            {
                if (_currentMusicInstance.isValid())
                {
                    _currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    _currentMusicInstance.release();
                }
                _isMusicPlaying = false;
            }
        }

        public void HuldraIntensity(float intecityInc)
        {
            Intecity += intecityInc;
            _huldraInstance.setParameterByName("Intensity", intecityInc);
        }
    }
}




//using FMODUnity;
//using System.Collections;
//using Unity.VisualScripting;
//using Unity.VisualScripting.Antlr3.Runtime;
//using UnityEngine;

//public class AudioManager : MonoBehaviour
//{
//    [SerializeField] private EventReference[] MusicLibrary;
//    [SerializeField] private EventReference[] SFXLibary;
//    [SerializeField] private GameObject BreakingBad;
//    FMOD.Studio.EventInstance BaseMusic;
//    private float currentIntensity = 0;
//    public float newIntensity;
//    private Coroutine fadeCoroutine;


//    private void Start()
//    {
//        if (BreakingBad != null)
//        {
//            BreakingBad.SetActive(false);
//        }

//        if (MusicLibrary.Length > 0) {

//            currentIntensity = newIntensity;
//            BaseMusic = FMODUnity.RuntimeManager.CreateInstance(MusicLibrary[0]);
//            Intensity();
//            BaseMusic.start();
//        }


//        //MusicLibrary[0] = EventReference.Find("event:/Music/Huldra");

//        //SFXLibary[0] = EventReference.Find("event:/SFX/Button_press_1_Confirm_Back");
//        //SFXLibary[1] = EventReference.Find("event:/SFX/potion-brew");
//        //SFXLibary[2] = EventReference.Find("event:/SFX/Close_menu");
//        //SFXLibary[3] = EventReference.Find("event:/SFX/Area_unlocked_omnious");


//    }

//    public void PlaySound(int index)
//    {
//        RuntimeManager.PlayOneShot(SFXLibary[index], transform.position);
//    }

//    public void StopMusic()
//    {
//        if (MusicLibrary.Length > 0)
//        {
//        BaseMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
//        BaseMusic.release();
//        }
//    }

//    public void ChangeMusic(int index)
//    {
//        if (fadeCoroutine != null)
//        {
//            StopCoroutine(fadeCoroutine);
//            fadeCoroutine = null;
//        }

//        if (index == 0)
//        {
//            if (BreakingBad != null)
//            {
//                if (BreakingBad.activeSelf)
//                {
//                    BreakingBad.SetActive(false);
//                    BaseMusic.setPaused(true);

//                }
//            }
//        }
//        else if (index == 1)
//        {
//            BreakingBad.SetActive(true);
//            if (MusicLibrary.Length > 0)
//            {
//                Intensity();
//                BaseMusic.setPaused(true);

//            }

//        }
//    }

//    #region Intensity

//    public void IntensityChange(int Inte)
//    {
//        newIntensity += Inte * 0.5f;
//    }

//    private void Intensity()
//    {
//            BaseMusic.setParameterByName("Intensity", currentIntensity);
//    }

//    #endregion Intensity
//}
