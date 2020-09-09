using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentParent : MonoBehaviour {

    public List<GameObject> filamentObjects = new List<GameObject>();
    GameObject activeFilament;

    // Use this for initialization
	void Start () {
        activeFilament = filamentObjects[0];
        activeFilament.SetActive(true);
	}

    public void NewFilament()
    {
        activeFilament.SetActive(false);
        activeFilament = filamentObjects[Random.Range(0, filamentObjects.Count)];
        activeFilament.SetActive(true);
    }
}
