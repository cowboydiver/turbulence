using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentMovement : MonoBehaviour {

    public Vector3 moveDirection = new Vector3();
    public Vector3 filamentParentOriginalPosition;
    public Bounds boundingBox;

    private void Start()
    {
        filamentParentOriginalPosition = transform.parent.position;
    }

    public void Move(float movement)
    {
        //if(boundingBox.ma)
        transform.parent.transform.Translate(moveDirection * -movement * Time.deltaTime, Space.World);
    }
}
