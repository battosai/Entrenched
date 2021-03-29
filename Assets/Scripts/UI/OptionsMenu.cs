using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //user
    public Slider musicVolume;
    public Slider soundFXVolume;

    //utils
    private MainMenu mainMenu;
    private AudioSource musicSource;
    public AudioSource soundFXSource {get; private set;}
    private Text musicText;
    private Text soundFXText;

    private void Awake()
    {
        mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
        musicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        soundFXSource = GameObject.Find("SoundFXSource").GetComponent<AudioSource>();
        musicText = transform.Find(">MusicText/MusicText").GetComponent<Text>();
        soundFXText = transform.Find(">SoundFXText/SoundFXText").GetComponent<Text>();
    }

    private void Start()
    {
        //adjust text for custom font
        Utils.AdjustTextScaleForCustomFont(musicText);
        Utils.AdjustTextScaleForCustomFont(soundFXText);

        //adjust volume slider knobs size
        // StartCoroutine(Utils.FitKnobToSlider(musicVolume));
        // StartCoroutine(Utils.FitKnobToSlider(soundFXVolume));

        musicVolume.onValueChanged.AddListener(
            delegate
            {
                AudioManager.musicVolume = musicVolume.value;
                musicSource.volume = AudioManager.musicVolume;
                PlayerPrefs.SetFloat("MusicVolume", AudioManager.musicVolume);
            });
        musicVolume.value = AudioManager.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        soundFXVolume.onValueChanged.AddListener(
            delegate
            {
                AudioManager.soundFXVolume = soundFXVolume.value;
                soundFXSource.volume = AudioManager.soundFXVolume;
                PlayerPrefs.SetFloat("SoundFXVolume", AudioManager.soundFXVolume);
            });
        soundFXVolume.value = AudioManager.soundFXVolume = PlayerPrefs.GetFloat("SoundFXVolume", 1f);

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// OnClick listener for Back button.
    /// </summary>
    public void Back()
    {
        mainMenu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}