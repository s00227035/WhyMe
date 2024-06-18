using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    float horizontal, vertical;//input horizontal and vertical
    Vector3 mouseWorldPosition;

    public override void Start()
    {
        

        base.Start();//call the Start method of Character
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        transform.up = mouseWorldPosition - transform.position;
    }

    private void FixedUpdate()
    {
        body.MovePosition(transform.position + new Vector3(horizontal, vertical, 0) * movementSpeed * Time.deltaTime);
    }
}
