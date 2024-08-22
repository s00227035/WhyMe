using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //AudioSource is initialized
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing on MainMenuManager GameObject!");
        }

        //Play On Awake is disabled
        audioSource.playOnAwake = false;
    }

    public void OnPlayButtonClicked()
    {
        //Debug.Log("Play button clicked");
        PlayClickSound();
        SceneManager.LoadScene("TestMap"); //Load the game map
    }

    public void OnQuitButtonClicked()
    {
        //Debug.Log("Quit button clicked");
        PlayClickSound();
        Application.Quit(); //Quit the app
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Stop the play mode in Unity Editor
        #endif
    }

    public void OnButtonHover()
    {
        //Debug.Log("Button hovered");
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            //Debug.Log("Playing hover sound");
            audioSource.PlayOneShot(hoverSound);
        }
        else
        {
            Debug.LogWarning("Hover sound or AudioSource is missing");
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            //Debug.Log("Playing click sound");
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("Click sound or AudioSource is missing");
        }
    }
}
