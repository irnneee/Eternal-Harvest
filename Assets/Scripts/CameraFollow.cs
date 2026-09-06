using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [Header("Hitbox que delimita el mapa")]
    public BoxCollider2D mapBounds; 

    private Camera cam;
    private float camHeight, camWidth;

    private void Start()
    {
        // Obtenemos la cámara y calculamos su alto y ancho en unidades de Unity
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    private void LateUpdate()
    {
        if (target != null && mapBounds != null)
        {
            Vector3 newPosition = target.position;

            // mapBounds.bounds nos da los bordes reales del collider
            // Sumamos/restamos el tamaño de la cámara para que el borde de la pantalla no se salga
            float minX = mapBounds.bounds.min.x + camWidth;
            float maxX = mapBounds.bounds.max.x - camWidth;
            float minY = mapBounds.bounds.min.y + camHeight;
            float maxY = mapBounds.bounds.max.y - camHeight;

            // Aplicamos los límites matemáticos
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
    }
}