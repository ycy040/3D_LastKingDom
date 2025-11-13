using UnityEngine;

public class HealingStone : MonoBehaviour
{
    [Header("회복 설정")]
    public float healAmount = 20f; // 회복량
    public float healRadius = 3f; // 회복 범위
    public float healInterval = 30f; // 회복 간격 (초) - 30초마다

    [Header("시각 효과")]
    public Color gizmoColor = Color.green;
    public GameObject healEffect; // 회복 이펙트 (선택사항)
    public ParticleSystem healParticle; // 회복 파티클 (선택사항)
    public AudioClip healSound; // 회복 사운드 (선택사항)

    [Header("회전 효과")]
    public bool rotateStone = true; // 돌 회전 여부
    public float rotationSpeed = 50f; // 회전 속도

    private AudioSource audioSource;
    private float lastHealTime = 0f;
    private GameObject currentPlayer; // 범위 안에 있는 플레이어

    void Start()
    {
        // AudioSource 컴포넌트 가져오기 (없으면 추가)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && healSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 이펙트가 있다면 활성화
        if (healEffect != null)
        {
            healEffect.SetActive(true);
        }

        if (healParticle != null)
        {
            healParticle.Play();
        }

        // 시작 시간 초기화 (게임 시작 후 바로 회복 가능하도록)
        lastHealTime = Time.time - healInterval;
    }

    void Update()
    {
        // 돌 회전 효과
        if (rotateStone)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // 범위 안에 플레이어가 있으면 30초마다 회복
        if (currentPlayer != null)
        {
            if (Time.time >= lastHealTime + healInterval)
            {
                HealPlayer();
                lastHealTime = Time.time;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 범위 안에 들어옴
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.gameObject;
            Debug.Log("회복 돌 범위 안에 들어왔습니다!");
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 플레이어가 범위 안에 머물러 있음
        if (other.CompareTag("Player") && currentPlayer == null)
        {
            currentPlayer = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 플레이어가 범위 밖으로 나감
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;
            Debug.Log("회복 돌 범위를 벗어났습니다.");
        }
    }

    void HealPlayer()
    {
        if (GameManager.Instance != null)
        {
            // 현재 HP가 최대 HP보다 낮을 때만 회복
            if (GameManager.Instance.GetCurrentHP() < GameManager.Instance.GetMaxHP())
            {
                GameManager.Instance.Heal(healAmount);
                Debug.Log($"HP 회복! +{healAmount}");

                // 회복 사운드 재생
                if (audioSource != null && healSound != null)
                {
                    audioSource.PlayOneShot(healSound);
                }

                // 회복 파티클 재생
                if (healParticle != null)
                {
                    healParticle.Play();
                }
            }
            else
            {
                Debug.Log("HP가 이미 가득 차 있습니다.");
            }
        }
    }

    // 에디터에서 회복 범위를 시각적으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }

    void OnDrawGizmosSelected()
    {
        // 선택되었을 때 더 진한 색으로 표시
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, healRadius);

        // 회복 범위 표시
        Gizmos.color = gizmoColor;
        DrawCircle(transform.position, healRadius, 32);
    }

    // 원을 그리는 헬퍼 함수
    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(radius, 0, 0);

        for (int i = 0; i <= segments; i++)
        {
            angle = (i / (float)segments) * Mathf.PI * 2;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );

            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}