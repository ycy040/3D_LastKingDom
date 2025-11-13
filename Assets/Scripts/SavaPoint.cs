using UnityEngine;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{
    [Header("시각적 설정")]
    public Color gizmoColor = Color.cyan;
    public float gizmoRadius = 1.5f;

    [Header("이펙트 설정")]
    public GameObject savePointEffect; // 세이브 포인트 이펙트 (선택사항)
    public ParticleSystem ambientParticle; // 주변 파티클 (선택사항)

    [Header("회복 설정")]
    public bool healOnSpawn = true; // 스폰 시 체력 회복 여부

    [Header("리스폰 UI 설정 (선택)")]
    public Text respawnCountText; // 리스폰 기회 표시 UI

    void Start()
    {
        // GameManager에 이 SavePoint의 위치를 등록
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateSavePoint(transform.position);
            Debug.Log($"SavePoint 등록 완료: {transform.position}");
            UpdateRespawnUI();
        }
        else
        {
            Debug.LogWarning("GameManager를 찾을 수 없습니다!");
        }

        // 이펙트가 있다면 활성화
        if (savePointEffect != null)
        {
            savePointEffect.SetActive(true);
        }

        if (ambientParticle != null)
        {
            ambientParticle.Play();
        }
    }

    void Update()
    {
        // 리스폰 기회 UI 업데이트
        UpdateRespawnUI();
    }

    void UpdateRespawnUI()
    {
        if (respawnCountText != null && GameManager.Instance != null)
        {
            int current = GameManager.Instance.GetRespawnCount();
            int max = GameManager.Instance.maxRespawnCount;
            respawnCountText.text = $"남은 기회: {current}/{max}";
        }
    }

    // 에디터에서 SavePoint 위치를 시각적으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);

        // 십자가 모양으로 표시
        Gizmos.DrawLine(transform.position + Vector3.left * gizmoRadius,
                       transform.position + Vector3.right * gizmoRadius);
        Gizmos.DrawLine(transform.position + Vector3.forward * gizmoRadius,
                       transform.position + Vector3.back * gizmoRadius);

        // "S" 표시
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position + Vector3.up * 2f, Vector3.one * 0.3f);
    }

    void OnDrawGizmosSelected()
    {
        // 선택되었을 때 더 큰 범위와 지면 표시
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, gizmoRadius);

        // 지면에 원형 표시
        Gizmos.color = gizmoColor;
        DrawCircle(transform.position, gizmoRadius * 1.5f, 32);
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

    // 옵션: 플레이어가 SavePoint 근처에 오면 저장 알림
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"SavePoint 활성화! 이 위치에서 리스폰됩니다. (남은 기회: {GameManager.Instance.GetRespawnCount()})");
            // UI로 "체크포인트 도달!" 메시지 표시 가능
        }
    }
}