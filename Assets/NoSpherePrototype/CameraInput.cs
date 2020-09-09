using System.Collections.Generic;
using UnityEngine;

namespace NoSpherePrototype
{
    public class CameraInput : MonoBehaviour
    {
        #region Variables

        private Vector3 initialMousePosition = Vector3.zero;
        private new Camera camera;

        public List<FilamentTransformPosition> activeFilamentObjects = new List<FilamentTransformPosition>();
        public FilamentManager filamentManager;

        #endregion Variables

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                RotateFilament();
            }

            if (Input.GetMouseButtonDown(0))
            {
                CastRay();
            }
        }

        private void RotateFilament()
        {
            if (Input.GetMouseButtonDown(1))
            {
                initialMousePosition = Input.mousePosition;
            }

            if (initialMousePosition.x < Screen.width / 3)
            {
                activeFilamentObjects[0].Rotate(initialMousePosition);
            }
            else if (initialMousePosition.x > Screen.width / 3 && initialMousePosition.x < Screen.width - Screen.width / 3)
            {
                activeFilamentObjects[1].Rotate(initialMousePosition);
            }
            else
            {
                activeFilamentObjects[2].Rotate(initialMousePosition);
            }
        }

        private void CastRay()
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                filamentManager.SelectFilament(hit.transform);
            }
        }
    }
}