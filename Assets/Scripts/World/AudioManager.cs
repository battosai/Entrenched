using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip[] music;
    public AudioClip[] ambience;

    public static AudioManager instance;
    public AudioSource source {get; private set;}

    private Krieger krieger;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        krieger = Krieger.instance;
    }
}