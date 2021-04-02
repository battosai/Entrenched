using System;
using UnityEngine;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour
{
    public Sprite touchControls;
    public Sprite keyboardControls;

    private MainMenu mainMenu;
    private Image image;

    private void Awake()
    {
        mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
        image = transform.Find("Image").GetComponent<Image>();
    }

    private void Start()
    {
        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            image.sprite = touchControls;
        #else
            image.sprite = keyboardControls;
        #endif
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