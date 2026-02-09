using FMODUnity;
using UnityEngine;

namespace _Project._Scripts.Interfaces
{ 
public interface IAudioService
    {
        void PlayOneShot(EventReference sound);

        void PlayOneShotAtPosition(EventReference sound, Vector3 position);

        void PlayMusic(EventReference Music);

        void StopMusic();
    }
}
