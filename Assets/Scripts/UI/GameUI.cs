using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("General")]
    public Image fader;

    [Header("Weapon Selection")]
    public GameObject weaponSelection;
    //TODO:
    //fix Ready button clickable area is much larger than appears bc image is preserving aspect
    //NEEDS: do it

    //TODO:
    //Make selected weapons remain selectedSprite etc.
    //NEEDS: do it
    private Button[] weaponButtons;

    [Header("End Game")]
    public Text endGameText;
    public Image[] endGameButtonImages;

    // Custom font needs to scale via transform
    // Ratio established w/ iPhone 5/5S/5C/SE
    // (scale of 7)/(viewport width AKA Camera.pixelWidth of 1136)
    private readonly float textWidthRatio = 7f/1136f;

    private void Awake()
    {
        Debug.Assert(fader != null);
        weaponButtons = weaponSelection.transform.Find("Names").GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        foreach(Button b in weaponButtons)
        {
            b.onClick.AddListener(() => SelectWeapon(b.gameObject.name));
        }

        endGameText.transform.localScale = Vector3.one * Camera.main.pixelWidth * textWidthRatio;
        Krieger.instance.OnDeath += EndScreenSequenceWrapper;
    }

    /// <summary>
    /// OnClick Listener for weapon selection buttons.
    /// </summary>
    private void SelectWeapon(string buttonName)
    {
        string realName = Utils.antiLawsuit[buttonName];
        if(realName.Contains("gun"))
        {
            if(Krieger.instance.startingRangedWeapon != realName)
            {
                Krieger.instance.startingRangedWeapon = realName;
            }
        }
        else
        {
            if(Krieger.instance.startingMeleeWeapon != realName)
            {
                Krieger.instance.startingMeleeWeapon = realName;
            }
        }
    }

    /// <summary>
    /// OnClick Listener for Ready button (WeaponSelection Menu).
    /// </summary>
    public void Ready()
    {
        weaponSelection.SetActive(false);
        GameState.instance.isReady = true;
    }

    /// <summary>
    /// OnClick Listener for Redeploy button.
    /// </summary>
    public void RedeployWrapper(){StartCoroutine(Redeploy());}
    private IEnumerator Redeploy()
    {
        //fade out text and buttons before loading main menu
        foreach(Image img in endGameButtonImages)
        {
            StartCoroutine(
                Utils.Fade(
                    img,
                    Color.white,
                    Color.clear,
                    1f));
            img.GetComponent<Button>().interactable = false;
        }

        yield return StartCoroutine(
            Utils.Fade(
                endGameText,
                Color.white,
                Color.clear,
                1f));

        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// OnClick Listener for Back button.
    /// </summary>
    public void BackWrapper(){StartCoroutine(Back());}
    private IEnumerator Back()
    {
        //fade out text and buttons before loading main menu
        foreach(Image img in endGameButtonImages)
        {
            StartCoroutine(
                Utils.Fade(
                    img,
                    Color.white,
                    Color.clear,
                    1f));
            img.GetComponent<Button>().interactable = false;
        }

        yield return StartCoroutine(
            Utils.Fade(
                endGameText,
                Color.white,
                Color.clear,
                1f));

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Subscriber to Krieger.OnDeath Event. Starts the end screen sequence.
    /// </summary>
    private void EndScreenSequenceWrapper() {StartCoroutine(EndScreenSequence());}
    private IEnumerator EndScreenSequence() 
    {
        GameObject endGameElements = endGameText.transform.parent.parent.gameObject;
        endGameElements.SetActive(true);

        //fader screen comes in
        yield return StartCoroutine(
            Utils.Fade(
                fader,
                Color.clear,
                Color.black,
                1f));

        //text and buttons fade in together
        foreach(Image img in endGameButtonImages)
        {
            StartCoroutine(
                Utils.Fade(
                    img,
                    Color.clear,
                    Color.white,
                    1f));
        }
        
        endGameText.text = 
            $"ENEMIES KILLED: {GameState.instance.enemiesDefeated}\n" +
            $"DISTANCE WALKED: {GameState.instance.feetTraversed} FT";
        yield return StartCoroutine(
            Utils.Fade(
                endGameText,
                Color.clear,
                Color.white,
                1f));

        //buttons made interactable after
        foreach(Image img in endGameButtonImages)
        {
            img.GetComponent<Button>().interactable = true;
        }
    }
}