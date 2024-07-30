using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public BoxTargetArea[] targetAreas; //Array of target areas

    private void Update()
    {
        if (AreAllBoxesPlaced())
        {
            OpenDoor();
        }
    }

    //Check if all boxes are placed in their respective areas
    private bool AreAllBoxesPlaced()
    {
        foreach (BoxTargetArea area in targetAreas)
        {
            if (!area.isOccupied)
            {
                return false; //Return not true if any area is not occupied
            }
        }
        return true; //Return true if areas are occuped
    }

    //Open the door
    private void OpenDoor()
    {
        Debug.Log("DOOR OPENED!");
        Destroy(gameObject); //Destroy the door
    }
}
