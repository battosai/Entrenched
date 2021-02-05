using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //TODO:
    //Add options for options button...
    //NEED: needs music, sounds, etc.

    public Image fader;

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
        Debug.Log($"There are no options yet c:");
    }
}