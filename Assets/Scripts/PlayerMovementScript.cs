using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;
    private Transform cameraTransform;
    
    private Rigidbody rb;
    public float MoveSpeed = 5f;
    public float moveHorizontal;
    public float moveForward;

    public float sprintMultiplier = 1.5f;
    public float slowMultiplier = 0.5f;
    private float currentSpeedMultiplier = 1f;

    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f; 
    public float ascendMultiplier = 2f; 
    private bool isGrounded = true;
    public LayerMask groundLayer;
    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float playerHeight;
    private float raycastDistance;

    [SerializeField] Material pixelate;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float defaultFadeDuration = 2.0f;

    [SerializeField] private bool awakeAnim = true;

    private void Awake() {
        pixelate.SetInt("_Pixelation", 3);

        if (awakeAnim) {
            GameEventManager.InputContext = InputContextEnum.LOCKED;
            StartCoroutine(awakaningAnimation());
        }
    }

    public IEnumerator awakaningAnimation() {
        pixelate.SetInt("_Pixelation", 100);
        fadeCanvasGroup.alpha = 1;

        float elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration));
            fadeCanvasGroup.alpha = newAlpha;

            int newPixel = (int)Mathf.Lerp(100f, 3f, (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration));
            pixelate.SetInt("_Pixelation", newPixel);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        pixelate.SetInt("_Pixelation", 3);
        GameEventManager.InputContext = InputContextEnum.DEFAULT;
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;

        playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if(GameEventManager.InputContext != InputContextEnum.LOCKED) {
            moveHorizontal = Input.GetAxisRaw("Horizontal");
            moveForward = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(KeyCode.LeftShift)) {
                currentSpeedMultiplier = sprintMultiplier;
                GameEventManager.WalkingContext = WalkingContextEnum.SPRINTING;
            } else if (Input.GetKey(KeyCode.LeftControl)) {
                currentSpeedMultiplier = slowMultiplier;
                GameEventManager.WalkingContext = WalkingContextEnum.SLOW;
            } else {
                currentSpeedMultiplier = 1f;
                GameEventManager.WalkingContext = WalkingContextEnum.WALKING;
            }

            RotateCamera();

            if (Input.GetButtonDown("Jump") && isGrounded) {
                Jump();
            }
        }

        if (!isGrounded && groundCheckTimer <= 0f) {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
        } else {
            groundCheckTimer -= Time.deltaTime;
        }

    }

    void FixedUpdate() {
        MovePlayer();
        ApplyJumpPhysics();
    }

    void MovePlayer() {
        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        Vector3 targetVelocity = movement * MoveSpeed * currentSpeedMultiplier;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;

        rb.linearVelocity = velocity;

        if (isGrounded && moveHorizontal == 0 && moveForward == 0) {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void RotateCamera() {
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, horizontalRotation, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void Jump() {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); 
    }

    void ApplyJumpPhysics() {
        if (rb.linearVelocity.y < 0) {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        } 
        else if (rb.linearVelocity.y > 0) {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendMultiplier  * Time.fixedDeltaTime;
        }
    }
}