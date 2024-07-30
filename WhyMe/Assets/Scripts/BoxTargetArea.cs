using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTargetArea : MonoBehaviour
{
    public bool isOccupied { get; private set; } = false; //Whether the area is occupied by a box
    public string requredBoxTag = "Box"; //Tag to identify boxes

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(requredBoxTag))
        {
            //Mark this area as occupied if the correct box enters
            isOccupied = true;
            Debug.Log("BOX PLACED CORRECTLY!");
            //Lock the box in the area
            DraggableBox box = collision.GetComponent<DraggableBox>();
            if (box != null)
            {
                box.LockPosition(transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(requredBoxTag))
        {
            //Mark this area as unoccupied if the box leaves
            isOccupied = false;
            Debug.Log("BOX REMOVED FROM TARGET AREA");
        }
    }
}
