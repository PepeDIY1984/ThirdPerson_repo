using UnityEngine;
using TMPro;  // 👈 Necesario para usar TextMeshProUGUI

public class TriggerAreaMessageTMP : MonoBehaviour
{
    [Header("Referencias del Canvas")]
    public GameObject messagePanel;          // Panel del Canvas que contiene el texto
    public TextMeshProUGUI messageText;      // Texto con TMP

    [Header("Mensajes")]
    [TextArea]
    public string messageNoKey = "🔒 Necesitas una llave para continuar.";
    [TextArea]
    public string messageHasKey = "✅ Tienes la llave. Puedes pasar a la siguiente fase.";

    [Header("Jugador")]
    public PlayerInventory playerInventory;  // Referencia al script del jugador que guarda la llave

    private void Start()
    {
        // Asegura que el panel de mensaje está oculto al inicio
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (messagePanel == null || messageText == null || playerInventory == null)
            {
                Debug.LogWarning("[TriggerAreaMessageTMP] Faltan referencias en el inspector.");
                return;
            }

            messagePanel.SetActive(true);

            // Comprueba si el jugador tiene la llave
            if (playerInventory.hasKey)
                messageText.text = messageHasKey;
            else
                messageText.text = messageNoKey;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (messagePanel != null)
                messagePanel.SetActive(false);
        }
    }
}
