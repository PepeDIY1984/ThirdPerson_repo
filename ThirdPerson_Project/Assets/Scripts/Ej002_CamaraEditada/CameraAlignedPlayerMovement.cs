using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraAlignedPlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;
    public bool canJump = true;
    private bool canMove = true;

    [Header("C�mara")]
    [Tooltip("Arrastra aqu� la c�mara principal o CinemachineBrain.")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 inputDir;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // --- Captura de input ---
        float h = Input.GetAxis("Horizontal"); // A / D
        float v = Input.GetAxis("Vertical");   // W / S
        inputDir = new Vector3(h, 0f, v).normalized;

        // --- Salto ---
        if (canJump && Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // --- Detecci�n de suelo ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    public void EnableMovement(bool value)
    {
        canMove = value;
        if (!canMove)
            rb.linearVelocity = Vector3.zero; // Detiene el movimiento inmediatamente
    }

    void FixedUpdate()
    {
        if (!canMove)
            return;

        if (inputDir.sqrMagnitude < 0.01f)
            return;

        // --- Dirección relativa a la cámara ---
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

        Vector3 targetVel = moveDir * moveSpeed;
        rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
