using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraRaycast : MonoBehaviour
{
    [Header("Raycast")]
    public Camera playerCamera;                 // Cámara desde la que sale el rayo
    public float rayDistance = 20f;             // Distancia máxima
    public LayerMask hittableLayers = ~0;       // Capas a considerar (por defecto, todas)
    public QueryTriggerInteraction triggerMode = QueryTriggerInteraction.Ignore;

    [Tooltip("Empuja el origen del rayo por delante del nearClip para evitar empezar dentro del collider del player.")]
    public float originPushEpsilon = 0.02f;     // Empuje extra desde la cámara

    [Header("Debug")]
    public Color rayColor = Color.red;
    public bool verboseDebug = false;           // Log detallado de impactos

    [Header("Marcador de impacto")]
    public float hitMarkerSize = 0.2f;
    public float hitMarkerDuration = 2f;
    public Color hitMarkerColor = Color.yellow;

    // Conjunto de colliders del player para ignorarlos rápido
    private HashSet<Collider> playerColliders;

    void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        // Reúne TODOS los colliders del jugador (este GO y sus hijos)
        playerColliders = new HashSet<Collider>(GetComponentsInChildren<Collider>());
    }

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("[CameraRaycast] No se ha asignado playerCamera.");
            return;
        }

        // Origen del rayo: un poco por delante del near clip, evita empezar dentro del capsule del player
        Vector3 origin = playerCamera.transform.position
                         + playerCamera.transform.forward * (playerCamera.nearClipPlane + originPushEpsilon);
        Vector3 direction = playerCamera.transform.forward;

        // Dibuja el rayo en Scene View
        Debug.DrawRay(origin, direction * rayDistance, rayColor);

        // Lanza RaycastAll para poder filtrar impactos propios
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, rayDistance, hittableLayers, triggerMode);

        if (hits.Length == 0)
        {
            if (verboseDebug) Debug.Log("[CameraRaycast] Sin impactos.");
            return;
        }

        // Ordena por distancia y elige el primer impacto que NO sea del player
        var ordered = hits.OrderBy(h => h.distance);
        RaycastHit? firstValid = null;

        foreach (var h in ordered)
        {
            if (playerColliders.Contains(h.collider))
            {
                if (verboseDebug) Debug.Log($"[CameraRaycast] Ignorado (player): {h.collider.name} a {h.distance:F3}m");
                continue;
            }
            firstValid = h;
            break;
        }

        if (verboseDebug)
        {
            string list = string.Join(", ", ordered.Select(h => $"{h.collider.name}@{h.distance:F2}m"));
            Debug.Log($"[CameraRaycast] Hits: {list}");
        }

        if (firstValid.HasValue)
        {
            var hit = firstValid.Value;
            Debug.Log($"[CameraRaycast] Impacto: {hit.collider.name} @ {hit.distance:F2}m");
            StartCoroutine(ShowHitMarker(hit.point));
        }
        // Si todos los impactos eran del player, no hacemos nada
    }

    IEnumerator ShowHitMarker(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * hitMarkerSize;

        var r = sphere.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Standard"));
        r.material.color = hitMarkerColor;

        Destroy(sphere.GetComponent<Collider>());

        yield return new WaitForSeconds(hitMarkerDuration);
        if (sphere) Destroy(sphere);
    }
}
