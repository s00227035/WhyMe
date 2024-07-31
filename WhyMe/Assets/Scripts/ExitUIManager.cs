using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitUIManager : MonoBehaviour
{
    public GameObject exitUIPanel; //Reference to the UI panel

    private void Awake()
    {
        if (exitUIPanel != null)
        {
            exitUIPanel.SetActive(false); //Hide the panel on start
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
