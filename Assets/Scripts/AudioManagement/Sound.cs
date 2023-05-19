using UnityEngine.Audio;
using UnityEngine;

// Audio System altered from watching Brackeys "Introduction to AUDIO in Unity" YouTube Video, Published on May 31st, 2017

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(0.1f,3f)]
    public float pitch;

    public bool loop;

    [Range (0f,1f)]
    public float spatialBlend;

    [Range (0f,1000f)]
    public float minDistance;
    [Range(0f, 1000f)]
    public float maxDistance;

    [Range(0f,5f)]
    public float dopplerLevel;
}
