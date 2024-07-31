using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitUIManager : MonoBehaviour
{
    public GameObject exitUIPanel; //Reference to the UI panel
    private Player player; //Reference to the player script

    private void Awake()
    {
        if (exitUIPanel != null)
        {
            exitUIPanel.SetActive(false); //Hide the panel on start
        }
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
        SceneManager.LoadScene("MainMenu");
    }

    //handle restart button
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Reload the current scene
    }

    //handle quit button
    public void OnQuitButtonClick()
    {
        Application.Quit(); //Quit the app
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Stop play mode in Unity Editor
        #endif
    }
}
