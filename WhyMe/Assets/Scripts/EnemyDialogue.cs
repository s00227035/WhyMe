using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDialogue : MonoBehaviour
{
    public Button dialogueButton; //Button componenet
    public Text dialogueText; //Text componenet inside the Button
    public float dialogueDuration = 2f; //Time for visibilty
    public Vector3 offset = new Vector3(0, 1, 0); //Position above the enemy

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        //Initially inactive
        if (dialogueButton != null)
        {
            dialogueButton.gameObject.SetActive(false);
        }

        //Reference for Text inside the button
        if (dialogueButton != null && dialogueText == null)
        {
            dialogueText = dialogueButton.GetComponentInChildren<Text>();
        }
    }

    public void ShowDialogue(string message)
    {
        if (dialogueButton != null && dialogueText != null)
        {
            dialogueText.text = message; //Text of the button
            dialogueButton.gameObject.SetActive (true); //show the button

            //Coroutine for hiding the pop up
            StartCoroutine(HideDialogueAfterDelay(dialogueDuration));
        }
    }

    private IEnumerator HideDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialogueButton != null)
        {
            dialogueButton.gameObject.SetActive (false); //Hide the button
        }
    }

    private void Update()
    {
        if (dialogueButton != null && dialogueButton.gameObject.activeSelf)
        {
            //Position the pop up above the neenmy
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + offset);
            dialogueButton.transform.position = screenPosition;
        }
    }
}
