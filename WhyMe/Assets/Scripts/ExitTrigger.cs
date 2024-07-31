using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public ExitUIManager exitUIManager; //Reference to the UI manager

    private void Start()
    {
        //UI panel is hidden at the start
        if (exitUIManager != null)
        {
            exitUIManager.exitUIPanel.SetActive(false);
        }
    }

    //Detect when the player enters the trigger area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (exitUIManager != null)
            {
                exitUIManager.ShowExitUI(); //Show the exit UI
            }
        }
    }
}
