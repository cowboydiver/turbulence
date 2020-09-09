using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

    public bool rotate = true;
    public float minSpeed = 0f;
    public float maxSpeed = 1f;

    float speed = 1f;

    Vector3 rotateDirection;

    public void Start() {
        rotateDirection = Random.insideUnitSphere;
        Mathf.Clamp(minSpeed, Mathf.NegativeInfinity, maxSpeed);
        Mathf.Clamp(maxSpeed, minSpeed, Mathf.Infinity);
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update () {
        if (rotate)
            transform.Rotate(rotateDirection * speed  * Time.deltaTime, Space.Self);
	}
}
