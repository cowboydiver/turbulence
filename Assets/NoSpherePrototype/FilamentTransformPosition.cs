using UnityEngine;

namespace NoSpherePrototype
{
    public class FilamentTransformPosition : MonoBehaviour
    {
        private float rotationSpeed = 3f;

        Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.position;
        }

        public void Rotate(Vector3 initialMousePosition)
        {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");
            transform.Rotate(new Vector3(v, -h, 0), Space.World);
        }
    }
}