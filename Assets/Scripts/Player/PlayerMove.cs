using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private Transform cameraTransform; // 카메라 연결 필수
    [SerializeField] private Transform cameraHolder;    // Head Bob을 위한 카메라 부모
    [SerializeField] private float eyeHeight = 1.65f;

    [Header("Head Bob Settings")]
    [SerializeField] private float walkBobAmount = 0.004f;
    [SerializeField] private float walkBobSpeed = 5f;
    [SerializeField] private float runBobAmount = 0.004f;
    [SerializeField] private float runBobSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveDir;
    private bool isGrounded;
    private float bobTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (cameraTransform == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
                cameraTransform = cam.transform;
            else
                Debug.LogError("Camera Transform이 연결되지 않았습니다!");
        }

        if (cameraHolder == null)
            cameraHolder = cameraTransform.parent;

        cameraHolder.localPosition = new Vector3(0f, eyeHeight, 0f);
    }

    void Update()
    {
        // AWSD 입력
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 카메라 기준 이동
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        moveDir = (camForward * v + camRight * h);
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        // 점프 입력
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // 지면 체크
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.1f);

        // 중력 적용
        if (!isGrounded)
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        // 이동 속도 설정 (달리기: Left Shift)
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 이동
        Vector3 velocity = moveDir * speed;
        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(velocity.x, currentVelocity.y, velocity.z);

        // 플레이어 회전 (카메라 바라보는 방향)
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Vector3 lookDir = cameraTransform.forward;
            lookDir.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            float rotationSpeed = 6f; // 회전 부드럽기 조절
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Head Bob 처리
        HandleHeadBob(speed);
    }

    void HandleHeadBob(float speed)
    {
        if (moveDir.magnitude > 0.1f && isGrounded)
        {
            float bobAmount = Input.GetKey(KeyCode.LeftShift) ? runBobAmount : walkBobAmount;
            float bobSpeed = Input.GetKey(KeyCode.LeftShift) ? runBobSpeed : walkBobSpeed;

            bobTimer += Time.fixedDeltaTime * bobSpeed;
            Vector3 localPos = cameraHolder.localPosition;
            localPos.y = eyeHeight + Mathf.Sin(bobTimer) * bobAmount;
            cameraHolder.localPosition = localPos;
        }
        else
        {
            bobTimer = 0f;
            Vector3 localPos = cameraHolder.localPosition;
            localPos.y = eyeHeight;
            cameraHolder.localPosition = localPos;
        }
    }
}
