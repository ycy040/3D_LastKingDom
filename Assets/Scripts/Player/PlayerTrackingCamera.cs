using UnityEngine;

public class PlayerTrackingCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform;                  // Main Camera Transform
    public Transform playerTransform;                  // 플레이어 Transform
    public float eyeHeight = 1.65f;                     // 플레이어 눈 높이
    public float pitchMin = -35f;
    public float pitchMax = 60f;
    public float mouseSensitivity = 5f;
    public float cameraSmoothTime = 0.15f;
    //public float zoomSpeed = 2f;
    //public float minZoom = 0.1f; // 1인칭에 맞춤
    //public float maxZoom = 5f;
    public float fixZoom = 0.1f;

    private float yaw = 0f;
    private float pitch = 0f;
    private float currentZoom;
    private Vector3 currentVelocity = Vector3.zero;

    [HideInInspector] public Vector3 moveDir;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (playerTransform == null)
            Debug.LogError("Player Transform이 연결되지 않았습니다!");

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // currentZoom = minZoom; // 초기 카메라 거리

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraRotation();
        //HandleZoom();
        HandleMovementInput();
    }

    void LateUpdate()
    {
        HandleCameraPosition();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    /*void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }*/

    void HandleCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 eyePosition = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 offsetDir = rotation * Vector3.forward * -fixZoom;
        Vector3 desiredPosition = eyePosition + offsetDir;

        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position,
            desiredPosition,
            ref currentVelocity,
            cameraSmoothTime
        );

        cameraTransform.LookAt(eyePosition);
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        moveDir = (camForward * vertical + camRight * horizontal);

        if (moveDir.magnitude > 0.1f)
            moveDir.Normalize();
    }
}
