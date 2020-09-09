using System.Collections.Generic;
using UnityEngine;

namespace NoSpherePrototype
{
    public class FilamentManager : MonoBehaviour
    {
        private FilamentSetup FilamentSetup;
        public List<Filament> filamentObjects = new List<Filament>();
        public List<FilamentTransformPosition> filamentTransformPositions = new List<FilamentTransformPosition>();
        public CameraInput CameraMovement;
        public List<Material> filamentMaterials = new List<Material>();

        public enum MatType
        {
            Regular,
            Green,
            Red,
            LightYellow
        }

        private float sphereSize = 0;

        private void Awake()
        {
            FilamentSetup = GetComponent<FilamentSetup>();
        }

        // Use this for initialization
        private void Start()
        {
            NewFilament();
        }

        public void NewFilament()
        {
            for (int i = 0; i < filamentTransformPositions.Count; i++)
            {
                foreach (Transform filament in filamentTransformPositions[i].transform)
                {
                    Destroy(filament.gameObject);
                }
            }

            for (int i = 0; i < filamentTransformPositions.Count; i++)
            {
                List<GameObject> completeFilamet = FilamentSetup.InitializeFilament(filamentObjects[Random.Range(0, filamentObjects.Count)]);
                for (int j = 0; j < completeFilamet.Count; j++)
                {
                    completeFilamet[j].transform.SetParent(filamentTransformPositions[i].transform);// = tempFilament.transform.position + new Vector3(i * 2, tempFilament.transform.position.y, tempFilament.transform.position.z);
                    completeFilamet[j].transform.position = new Vector3((filamentTransformPositions[i].transform.position.x + completeFilamet[j].transform.position.x), completeFilamet[j].transform.position.y, completeFilamet[j].transform.position.z);
                }
            }
            CameraMovement.activeFilamentObjects = filamentTransformPositions;
        }

        public void SelectFilament(Transform selectedFilament)
        {
            sphereSize = 0;

            for (int i = 0; i < filamentTransformPositions.Count; i++)
            {
                Filament[] filaments = filamentTransformPositions[i].GetComponentsInChildren<Filament>();

                for (int j = 0; j < filaments.Length; j++)
                {
                    filaments[j].GetComponent<Renderer>().material = filamentMaterials[(int)MatType.Regular];
                }

                float filamentSphereSize = filaments[0].GetBiggestPossibleSphereSize;
                sphereSize = sphereSize < filamentSphereSize ? filamentSphereSize : sphereSize;
            }

            foreach (Transform filamentCHild in selectedFilament.parent)
            {
                filamentCHild.GetComponent<Renderer>().material = filamentMaterials[(int)MatType.LightYellow];
            }
        }

        public void SubmitFilament()
        {
            for (int i = 0; i < filamentTransformPositions.Count; i++)
            {
                foreach (Transform filament in filamentTransformPositions[i].transform)
                {
                    print(filament.name);
                    filament.GetComponent<Renderer>().material = filament.GetComponent<Filament>().GetBiggestPossibleSphereSize >= sphereSize ?
                                                                 filamentMaterials[(int)MatType.Green] : filamentMaterials[(int)MatType.Red];
                }
            }
        }
    }
}