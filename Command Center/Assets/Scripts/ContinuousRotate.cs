using UnityEngine;

public class ContinuousRotate : MonoBehaviour
{
    public Vector3 direction;

    private void Update()
    {
        transform.Rotate(direction);
    }
}
