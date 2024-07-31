using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAreaColor : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; //Reference to the sprite renderer
    public Color occupiedColor = Color.green;
    public Color unoccupiedColor = Color.red;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Initialize with the unoccupied color
        spriteRenderer.color = unoccupiedColor;
    }

    //Change color when box's there
    public void SetOccupiedColor()
    {
        spriteRenderer.color = occupiedColor;
    }

    //Change color when box's not there
    public void SetUnoccupiedColor()
    {
        spriteRenderer.color = unoccupiedColor;
    }
}
