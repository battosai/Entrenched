using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    public Image fader;
    public OptionsMenu options;
    public HelpMenu help;

    [Header("Sounds")]
    public AudioClip[] clicks;

    private void Start()
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach(Button b in buttons)
        {
            b.onClick.AddListener(() => AudioManager.PlayOneClip(options.soundFXSource, clicks));
        }
    }

    /// <summary>
    /// OnClick Listener for Play button.
    /// </summary>
    public void PlayWrapper(){StartCoroutine(Play());}
    private IEnumerator Play()
    {
        yield return StartCoroutine(
            Utils.Fade(
                fader,
                Color.clear,
                Color.black,
                1f));
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// OnClick Listener for Exit button.
    /// </summary>
    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    /// <summary>
    /// OnClick Listener for Options button.
    /// </summary>
    public void Options()
    {
        options.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// OnClick Listener for Help button.
    /// </summary>
    public void Help()
    {
        help.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}