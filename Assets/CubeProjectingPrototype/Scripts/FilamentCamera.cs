using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentCamera : MonoBehaviour {

    public GameObject projectionCubeFace;
    ProjectionFill projectionFill;
    RenderTexture targetTexture;

	// Use this for initialization
	void Start () {
        projectionFill = projectionCubeFace.GetComponent<ProjectionFill>();
        targetTexture = GetComponent<Camera>().targetTexture;
        projectionFill.InitialiseTextures(targetTexture, GetComponent<Camera>());
	}
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(projectionFill.ReadProjectionPixels(targetTexture));
    }
}
