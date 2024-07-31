using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public GameObject exitUIPanel; //Reference to the UI panel

    private void Start()
    {
        //UI panel is hidden at the start
        if (exitUIPanel != null)
        {
            exitUIPanel.SetActive(false);
        }
    }

    //Detect when the player enters the trigger area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (exitUIPanel != null)
            {
                exitUIPanel.SetActive(true); //Show the UI panel
            }
        }
    }
}
