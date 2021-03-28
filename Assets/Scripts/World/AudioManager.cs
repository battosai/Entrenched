using System;
using UnityEngine;
using UnityEngine.UI;

using Random=UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    //TODO:
    //get more sounds for enemies, ui, etc.
    //NEEDS: do it

    [Header("Sounds")]
    public AudioClip[] music;
    public AudioClip[] ambience;
    public AudioClip[] ammoDrops;
    public AudioClip[] footsteps;

    public AudioSource musicSource {get; private set;}
    public AudioSource ambienceSource {get; private set;}

    //util
    public static AudioManager instance;
    public static float musicVolume;
    public static float soundFXVolume;
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

        musicSource.volume = musicVolume;
        ambienceSource.volume = soundFXVolume;

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
    /// Load audio source with clip and play.
    /// </summary>
    public static void Play(
        AudioSource audioSource,
        AudioClip clip,
        bool loop=false,
        float startTime=-1f)
    {
        if(clip == null)
            return;

        audioSource.volume = soundFXVolume;
        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.time = (startTime > 0) ? startTime : 0f;

        audioSource.Play();
    }

    /// <summary>
    /// Play a clip once.
    /// </summary>
    public static void PlayClip(
        AudioSource audioSource,
        AudioClip clip)
    {
        if(clip == null)
            return;

        audioSource.volume = soundFXVolume;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays a random clip in the provided array from the provided source.
    /// </summary>
    public static void PlayOneClip(
        AudioSource audioSource,
        AudioClip[] clips)
    {
        if(clips == null || clips.Length == 0)
            return;

        int roll = Random.Range(0, clips.Length);
        audioSource.volume = soundFXVolume;
        audioSource.PlayOneShot(clips[roll]);
    }
}