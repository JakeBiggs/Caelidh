using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections;

// Audio System altered from watching Brackeys "Introduction to AUDIO in Unity" YouTube Video, Published on May 31st, 2017

public class AudioManager : MonoBehaviour
{
    public GameObject tempAudGameObj;

    [NonReorderable]
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if(instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #region PlaySound
    //
    //Ask for advice on how to better manage that vvv
    //

    public AudioSource Play(string name)
    {
        bool wasTemp = true;
        GameObject focus;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return null;

        focus = Instantiate(tempAudGameObj);
        focus.transform.position = Vector3.zero;
        s.spatialBlend = 0;

        AudioSource audSource = focus.AddComponent<AudioSource>();
        AdjustAudioSource(audSource, s);

        audSource.Play();

        if (!audSource.loop) StartCoroutine(DestroyUsedAudio(wasTemp, focus, audSource));

        return audSource;
    }

    public AudioSource Play(string name, GameObject goOrigin)
    {
        bool wasTemp = false;
        GameObject focus;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return null;

        focus = goOrigin;

        AudioSource audSource = focus.AddComponent<AudioSource>();
        AdjustAudioSource(audSource, s);

        audSource.Play();

        if (!audSource.loop) StartCoroutine(DestroyUsedAudio(wasTemp, focus, audSource));

        return audSource;
    }

    public AudioSource Play(string name,Vector3 posOrigin)
    {
        bool wasTemp = true;
        GameObject focus;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return null;

        focus = Instantiate(tempAudGameObj);
        focus.transform.position = posOrigin;

        AudioSource audSource = focus.AddComponent<AudioSource>();
        AdjustAudioSource(audSource, s);

        audSource.Play();

        if(!audSource.loop) StartCoroutine(DestroyUsedAudio(wasTemp, focus, audSource));

        return audSource;
    }
    #endregion
    private IEnumerator DestroyUsedAudio(bool wasTemp,GameObject go, AudioSource audSource)
    {
        yield return new WaitForSeconds(audSource.clip.length);
        if(audSource != null) Destroy(audSource);
        if (wasTemp) Destroy(go);
    }

    public void StopAudio(AudioSource audSource)
    {
        if (audSource != null) audSource.Stop();
        if (audSource.transform.name.Contains("TempSoundSource")) Destroy(audSource.transform.gameObject);
        else Destroy(audSource);
    }

    //Will make any audio source fitted with a given Sound's Properties
    public void AdjustAudioSource(AudioSource source, Sound sound)
    {
        source.clip = sound.clip;

        source.volume = sound.volume;
        source.pitch = sound.pitch;

        source.loop = sound.loop;

        source.spatialBlend = sound.spatialBlend;

        source.minDistance = sound.minDistance;
        source.maxDistance = sound.maxDistance;

        source.dopplerLevel = sound.dopplerLevel;
    }
}
