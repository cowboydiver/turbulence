using System.Collections.Generic;
using UnityEngine;

public class OrginalTurbGameManager : MonoBehaviour
{
    private FilamentSetup FilamentSetup;
    public Transform filamentParent;
    public TargetSphere targetSphere;
    public List<GameObject> filamentObjects = new List<GameObject>();
    public CameraMovement CameraMovement;
    private GameObject filamentObject;

    private void Awake()
    {
        FilamentSetup = GetComponent<FilamentSetup>();
    }

    // Use this for initialization
    private void Start()
    {
        InitializeGame();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void InitializeGame()
    {
        NewFilament();
    }

    public void NewFilament()
    {
        if (filamentObject != null)
        {
            targetSphere.active = false;
            foreach (Transform child in filamentParent)
            {
                Destroy(child.gameObject);
            }
        }
        filamentObject = FilamentSetup.InitializeFilament(filamentObjects[Random.Range(0, filamentObjects.Count)].transform.GetChild(0).gameObject);
        CameraMovement.filamentObject = filamentObject;
        targetSphere.SetupTarget(filamentObject);

        // Call new filament
    }
}