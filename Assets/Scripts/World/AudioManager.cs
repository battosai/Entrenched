using System;
using UnityEngine;

using Random=UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip[] music;
    public AudioClip[] ambience;
    public AudioClip[] ammoDrops;

    private AudioSource musicSource;
    private AudioSource ambienceSource;

    //util
    public static AudioManager instance;
    private Krieger krieger;
    private int ambienceIndex;

    private void Awake()
    {
        instance = this;
        musicSource = transform.Find("MusicAudioSource").GetComponent<AudioSource>();
        ambienceSource = transform.Find("AmbienceAudioSource").GetComponent<AudioSource>();
    }

    private void Start()
    {
        krieger = Krieger.instance;
        ambienceIndex = Random.Range(0, ambience.Length);

        int musicSelection = Random.Range(0, music.Length);
        musicSource.PlayOneShot(music[musicSelection]);
    }

    private void Update()
    {
        if(!ambienceSource.isPlaying)
        {
            ambienceIndex = (ambienceIndex + 1) % ambience.Length;
            ambienceSource.PlayOneShot(ambience[ambienceIndex]);
        }
    }

    /// <summary>
    /// Play a clip once.
    /// </summary>
    public static void Play(
        AudioSource audioSource,
        AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays a random clip in the provided array from the provided source.
    /// </summary>
    public static void PlayOne(
        AudioSource audioSource,
        AudioClip[] clips)
    {
        if(clips.Length == 0)
            return;

        int roll = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[roll]);
    }
}