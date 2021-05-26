using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMnager : MonoBehaviour
{
    public static SoundMnager instance;
    public SoundClip[] soundClips;

    List<AudioSource> dormentAudioSources = new List<AudioSource>();

    private void Start() {
        instance = this;
    }

    public void PlayClipName(string clipName, float volume, float pitchShift){
        for (int i = 0; i < dormentAudioSources.Count; i++)
        {
            if(!dormentAudioSources[i].isPlaying){
                dormentAudioSources[i].clip = FindClip(clipName);
                dormentAudioSources[i].volume = volume;
                dormentAudioSources[i].pitch = 1 + (float)(Random.Range(-100,100)/100.00f) * pitchShift;
                dormentAudioSources[i].Play();
                return;
            }
        }
        AudioSource audioSource = CreateAudioSource();
        audioSource.playOnAwake = false;
        dormentAudioSources.Add(audioSource);
        audioSource.clip = FindClip(clipName);
        audioSource.volume = volume;
        audioSource.Play();

    }

    AudioSource CreateAudioSource(){
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.dopplerLevel = 0;
        audioSource.loop = false;
        audioSource.spatialBlend = 0;
        return audioSource;
    }

    AudioClip FindClip(string clipName){
        for (int i = 0; i < soundClips.Length; i++)
        {
            if(soundClips[i].name == clipName)return soundClips[i].clip;
        }
        return null;
    }

    [System.Serializable]
    public class SoundClip{
        public string name;
        public AudioClip clip;
    }

}
