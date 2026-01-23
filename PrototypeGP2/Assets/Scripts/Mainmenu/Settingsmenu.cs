using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

using System.Collections.Generic;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Serialization;


public class Settingsmenu : MonoBehaviour
{
    public TMPro.TMP_Dropdown resolutionDropdown;

    public AudioMixer  MasteraudioMixer;
    public AudioMixerGroup MusicGroup;
    public AudioMixerGroup SfxGroup;
    public Toggle vsyncTog;
    Resolution[] _resolution;
    public int userQualityLevel;
    public bool Isfullscreen;
    public int qualityLevel;
    public int vsyncCount;
    public Toggle vsync_Tog;
    int user_vsync_count = 0;
    int User_Quality_Level = 0;
    public float musicVolume;
    public float sfxvolume;
    public float MasterVolume;
    private bool ismuted;
    private const string MUSIC_PARAM = "musicvolume";
    private bool musicMuted = false;
    
    

    public string settingsFilePath => Application.persistentDataPath + "/settings.json";


    public class GameSettings
    {
        public bool vsyncEnabled;
        public float sfxvolume;
        public bool isFullscreen;
        public float musicvolume;
        public int Resolutionindex;
        public int qualityLevel;
        public int vsyncCount;
        public float Mastervolume;
        

        Resolution[] _resolutions;
        
      


    }



    void Start()    
    {       
        
     
        LoadSettings();
        _resolution = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolution.Length; i++)
        {
            string option = _resolution[i].width + " x " + _resolution[i].height;
            options.Add(option);

            if (_resolution[i].width == Screen.currentResolution.width &&
                _resolution[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void Set_Resolution(int resolutionIndex)
    {
        Resolution resolution = _resolution[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

    }

   public void Set_Quality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Quality level set to: " + qualityIndex);
    }
    


    public void SetFullscreen(bool isfullscreen)
    {
        Screen.fullScreen = isfullscreen;
        Debug.Log("Screen is full screen: " + Screen.fullScreen);

    }

    public void OnsyncToggleChanged(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.vSyncCount = 1;
            user_vsync_count = 1;
            Debug.Log("VSync enabled");
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            user_vsync_count = 0;
            Debug.Log("VSync disabled");

        }
    }

    public void Set_Music_Volume(float volume)
    {
        string ParameterName = "musicvolume";
        MasteraudioMixer.SetFloat(ParameterName, volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
         PlayerPrefs.SetFloat("musicvolume", musicVolume);
    }

    public void Set_Master_Volume(float volume)
    {
        string ParameterName = "mastervolume";
        MasteraudioMixer.SetFloat(ParameterName, volume);
        
            PlayerPrefs.SetFloat("Mastervolume", volume);
            PlayerPrefs.SetFloat("mastervolume", MasterVolume);
    
        
    }

    public void Set_SFX_Volume(float volume)
    {
        string ParameterName = "sfxvolume";
       MasteraudioMixer.SetFloat(ParameterName, volume);
        PlayerPrefs.SetFloat("SFXvolume", volume);
        PlayerPrefs.SetFloat("sfxvolume", sfxvolume);
    }
    

    public void SaveSettings()
    {
        GameSettings settings = new GameSettings
        {
            musicvolume = PlayerPrefs.GetFloat("musicvolume"),
            sfxvolume = PlayerPrefs.GetFloat("sfxvolume"),
            Mastervolume = PlayerPrefs.GetFloat("mastervolume"),
            isFullscreen = Screen.fullScreen,
            //vsyncEnabled = vsync_Tog.isOn,
            qualityLevel = userQualityLevel,
            vsyncCount = user_vsync_count,
            Resolutionindex = resolutionDropdown.value





        };
    }

    public void Vsync_ON()
    {
        
        
    }

    public void ToggleMusic(bool musicmuted)
    {
        musicMuted = !musicMuted;
        if (musicMuted)
        {
            Debug.Log("Music is off");
          
            MasteraudioMixer.SetFloat(MUSIC_PARAM, -80);
        }
        else
        {
            MasteraudioMixer.SetFloat(MUSIC_PARAM, 0);
            Debug.Log("Music is on");
            
        }
        PlayerPrefs.SetInt("musicMuted", musicMuted ? 1 : 0);
    }
    
public void ToggleSound()
{
    ismuted =!ismuted;

    if (ismuted)
    {
        Debug.Log("audio is off");
        AudioListener.volume = 0;
    }
    else
    {   Debug.Log("audio is on");
        AudioListener.volume = 1;
    }

}
    
    

public void LoadSettings()
            {
                if (File.Exists(settingsFilePath))
                {
                    string json = File.ReadAllText(settingsFilePath);
                    GameSettings settings = JsonUtility.FromJson<GameSettings>(json);
                    musicMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
                    
                    musicVolume = settings.musicvolume;
                    
                    MasteraudioMixer.SetFloat("volume", settings.sfxvolume);
                    
                    QualitySettings.vSyncCount = settings.vsyncCount;

                    userQualityLevel = settings.qualityLevel;
                    
                    vsyncCount = settings.vsyncCount;
                    
                    //vsync_Tog.isOn = settings.vsyncEnabled;
                    
                    Screen.fullScreen = settings.isFullscreen;
                    
                    OnsyncToggleChanged(settings.vsyncEnabled);
                    
                    QualitySettings.vSyncCount = settings.qualityLevel;
                    
                    Set_Quality(settings.qualityLevel);
                    
                    Set_Quality(User_Quality_Level);
                    
                    resolutionDropdown.value = settings.Resolutionindex;
                    
                    Set_Resolution(settings.Resolutionindex);
                    if (musicMuted)
                        MasteraudioMixer.SetFloat(MUSIC_PARAM, -80f);
                    else
                        MasteraudioMixer.SetFloat(MUSIC_PARAM, 0f);
                    
                    Debug.Log("settings loaded from JSON");
                        
                }

                else 
                    {
                        Debug.Log("No settings loaded from JSON");
                }
            }
            
        }
        




