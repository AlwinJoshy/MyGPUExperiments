using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] List<AudioClipContainer> soundLibrary = new List<AudioClipContainer>();
    [SerializeField] float checkRate = 5;


    List<AudioSource> freeAudioSources = new List<AudioSource>();
    List<AudioSource> usedAudioSources = new List<AudioSource>();

    public static SoundManager instance;

    float nextDequeCheck;
    AudioSource selectedSrc;
    AudioClip soundClip;

    void Start()
    {

        instance = this;

        for (int i = 0; i < soundLibrary.Count; i++)
        {
            soundLibrary[i].HashCode = soundLibrary[i].name.GetHashCode();
        }
    }

    void Update()
    {
        if (Time.time > nextDequeCheck)
        {

            for (int i = 0; i < usedAudioSources.Count; i++)
            {
                if (!usedAudioSources[i].isPlaying)
                {
                    freeAudioSources.Add(usedAudioSources[i]);
                    usedAudioSources.Remove(usedAudioSources[i]);
                }
            }

            nextDequeCheck = Time.time + 1 / checkRate;
        }
    }

    public void PlaySound(string soundName, float volume, float pitchShift)
    {
        if (this != null)
        {
            int soundHash = soundName.GetHashCode();

            for (int i = 0; i < soundLibrary.Count; i++)
            {
                if (soundLibrary[i].HashCode == soundHash)
                {
                    soundClip = soundLibrary[i].audioClip;
                    break;
                }
            }

            // there is free audio source
            if (freeAudioSources.Count > 0)
            {
                selectedSrc = freeAudioSources[freeAudioSources.Count - 1];
                freeAudioSources.Remove(selectedSrc);
            }

            else
            {
                selectedSrc = this.gameObject.AddComponent<AudioSource>();
            }

            usedAudioSources.Add(selectedSrc);
            selectedSrc.clip = soundClip;
            selectedSrc.volume = volume;
            selectedSrc.pitch = pitchShift;
            selectedSrc.Play();
            selectedSrc = null;
        }
    }

    [System.Serializable]
    public class AudioClipContainer
    {
        public string name;
        public AudioClip audioClip;
        int hashCode;

        public int HashCode
        {
            get { return hashCode; }
            set { hashCode = value; }
        }
    }

}
