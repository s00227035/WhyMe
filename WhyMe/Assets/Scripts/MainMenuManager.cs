using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("TestMap"); //Load the game map
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit(); //Quit the app
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Stop the play mode in Unity Editor
        #endif
    }
}
