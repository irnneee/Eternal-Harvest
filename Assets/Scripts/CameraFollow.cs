using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.z = transform.position.z; // Maintain the camera's z position
            transform.position = newPosition;
        }
    }
}
