using UnityEngine;
using Cinemachine;

public class CameraTriggerSwitch : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineVirtualCamera nextCamera;
    public CinemachineVirtualCamera previousCamera;

    [Header("Opcional")]
    [Tooltip("Duración del blend entre cámaras (en segundos).")]
    public float blendTime = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Cambiar prioridades para que Cinemachine haga el blend automático
        nextCamera.Priority = 10;
        previousCamera.Priority *=-1;
    }
}
