using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKey = false;

    // Ejemplo: recoger llave
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            Debug.Log("Has recogido una llave.");
            Destroy(other.gameObject);
        }
    }
}
