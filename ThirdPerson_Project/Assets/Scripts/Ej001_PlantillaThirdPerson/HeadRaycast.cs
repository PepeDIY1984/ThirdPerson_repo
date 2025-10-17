using UnityEngine;
using System.Collections;

public class HeadRaycast : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    public Transform head;              // Transform de la cabeza del personaje
    public float rayDistance = 10f;     // Distancia máxima del rayo
    public Color rayColor = Color.red;  // Color de la línea del rayo

    [Header("Marcador de impacto")]
    public float hitMarkerSize = 0.2f;   // Tamaño de la esfera
    public float hitMarkerDuration = 2f; // Duración en segundos
    public Color hitMarkerColor = Color.yellow;

    void Update()
    {
        if (head == null)
        {
            Debug.LogWarning("No se ha asignado la cabeza del personaje al script HeadRaycast.");
            return;
        }

        Vector3 origin = head.position;
        Vector3 direction = head.forward;

        // Dibuja el rayo en la vista del editor
        Debug.DrawLine(origin, origin + direction * rayDistance, rayColor);

        // Lanza el raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            Debug.Log("Raycast ha chocado con: " + hit.collider.name);
            StartCoroutine(ShowHitMarker(hit.point));
        }
    }

    IEnumerator ShowHitMarker(Vector3 position)
    {
        // Crea una esfera temporal en el punto de impacto
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * hitMarkerSize;

        // Color de la esfera
        Renderer r = sphere.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Standard"));
        r.material.color = hitMarkerColor;

        // Quita el collider para evitar interferencias físicas
        Destroy(sphere.GetComponent<Collider>());

        // Espera el tiempo indicado y destruye la esfera
        yield return new WaitForSeconds(hitMarkerDuration);
        Destroy(sphere);
    }
}
