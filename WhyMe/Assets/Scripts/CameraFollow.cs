using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    Vector3 nextPosition;

    void Start()
    {
        nextPosition.z = transform.position.z;
    }

    void Update()
    {
        if (Target)
        {
            nextPosition.x = Target.position.x;
            nextPosition.y = Target.position.y;

            transform.position = nextPosition;
        }
    }
}
