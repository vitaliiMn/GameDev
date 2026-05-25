using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("🏃 Движение")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;

    [Header("🦘 Прыжок")]
    public float jumpForce = 10f;
    public float gravity = -40f;

    [Header("📷 Камера")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    
    // 🔑 КЛЮЧЕВОЙ ФЛАГ: Разрешает прыжок только на земле
    private bool canJump = true; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
	    // ✅ ПРОВЕРКА: Если игрок упал ниже 0 - игра окончена
    if (transform.position.y < -10f) 
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver(false); // false = поражение
        }
        return; // Останавливаем выполнение, чтобы игрок не двигался после смерти
    }

        if (GameManager.Instance != null && !GameManager.Instance.isGameActive) return;

        // 1. Проверка земли (используем встроенный + Raycast для надежности)
        bool isGrounded = controller.isGrounded;
        
        // Если Raycast вниз видит землю в радиусе 0.2м - считаем что стоим
        if (!isGrounded)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, ~LayerMask.GetMask("Ignore Raycast"));
        }

        // 2. Если стоим на земле и падаем вниз -> обнуляем скорость и РАЗРЕШАЕМ прыжок
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Прижимаем к полу
            canJump = true;   // 🔑 ВОЗВРАЩАЕМ РАЗРЕШЕНИЕ НА ПРЫЖОК
        }

        // 3. Обработка прыжка
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            canJump = false; // 🔑 БЛОКИРУЕМ повторный прыжок в воздухе
            Debug.Log("🦘 Прыжок! canJump = false");
        }

        // 4. Движение и камера
        HandleMovement();
        HandleCamera();

        // 5. Гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    if (GameManager.Instance != null)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.ToggleShop();
        }
    }



    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        if (GameManager.Instance != null)
            currentSpeed *= GameManager.Instance.speedMultiplier;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 moveDirection = (forward * z + right * x).normalized;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
}