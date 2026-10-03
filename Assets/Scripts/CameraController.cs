using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo y Distancia")]
    public Transform target;
    // TERCERA PERSONA ACERCADA
    // X=0 (centrado), Y=2 (altura de la cabeza), Z=-2.5 (más cerca de la espalda)
    public Vector3 offset = new Vector3(0f, 2.0f, -2.5f);

    [Header("Velocidad de Seguimiento")]
    // Sincronización instantánea (valor muy alto)
    public float smoothSpeed = 30f;

    void Update()
    {
        if (target == null) return;

        // Calcula la posición detrás del personaje según su rotación
        Vector3 desiredPosition = target.position + (target.rotation * offset);

        // Movimiento de la cámara instantáneo o muy rápido
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Enfoque: Mira hacia la cabeza/espalda alta
        transform.LookAt(target.position + Vector3.up * 1.6f);
    }
}