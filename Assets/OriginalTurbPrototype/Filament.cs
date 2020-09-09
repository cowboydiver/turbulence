using UnityEngine;

public class Filament : MonoBehaviour
{
    public Vector3 Position
    {
        get; private set;
    }

    public void SetFilamentValues(Vector3 OffSetPosition)
    {
        Position = OffSetPosition;
    }
}