using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitUIManager : MonoBehaviour
{
    public GameObject exitUIPanel; //Reference to the UI panel
    private Player player; //Reference to the player script
    private AudioSource audioSource;

    public AudioClip hoverSound; //Hover sound
    public AudioClip clickSound; //Click sound

    private void Awake()
    {
        if (exitUIPanel != null)
        {
            exitUIPanel.SetActive(false); //Hide the panel on start
        }

        //AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Find and assign the Player instance
        player = FindObjectOfType<Player>();
    }

    //Method to show the exit UI
    public void ShowExitUI()
    {
        if (exitUIPanel != null)
        {
            exitUIPanel.SetActive(true); //Show the UI panel
            if (player != null)
            {
                player.SetInputEnabled(false); //Disable player input
            }
        }
    }

    //Method to handle Main menu button
    public void OnMainMenuButtonClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene("MainMenu");
    }

    //handle restart button
    public void OnRestartButtonClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Reload the current scene
    }

    //handle quit button
    public void OnQuitButtonClick()
    {
        PlayClickSound();
        Application.Quit(); //Quit the app
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Stop play mode in Unity Editor
        #endif
    }

    public void OnButtonHover()
    {
        PlayHoverSound();
    }


    //Hover sound
    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound); // Play hover sound
        }
    }

    //Click sound
    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound); // Play click sound
        }
    }
}

