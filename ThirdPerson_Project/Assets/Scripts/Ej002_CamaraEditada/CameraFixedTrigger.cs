using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraFixedTrigger : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineVirtualCamera fixedCamera;
    public CinemachineVirtualCamera previousCamera;

    [Header("Player")]
    public CameraAlignedPlayerMovement playerMovement;

    [Header("Controles")]
    [Tooltip("Tecla para salir manualmente de la cámara fija.")]
    public KeyCode exitKey = KeyCode.Escape;

    private bool isActive = false; // Indica si la cámara fija está activa
    public Button botonSalir;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        botonSalir.gameObject.SetActive(true);

        // Activar cámara fija
        fixedCamera.Priority = 30;
        previousCamera.Priority = 10;

        // Desactivar movimiento
        if (playerMovement != null)
            playerMovement.EnableMovement(false);

        // Marcar como activa
        isActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        RestorePreviousState();
    }

    private void Update()
    {
        // Si está activa la cámara fija y se presiona la tecla de salir
        if (isActive && Input.GetKeyDown(exitKey))
        {
            RestorePreviousState();
        }
    }

    public void RestorePreviousState()
    {
        botonSalir.gameObject.SetActive(false);

        // Volver a la cámara anterior
        fixedCamera.Priority = 10;
        previousCamera.Priority = 20;

        // Reactivar movimiento del jugador
        if (playerMovement != null)
            playerMovement.EnableMovement(true);

        // Marcar como inactiva
        isActive = false;
    }
}
