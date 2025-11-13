using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("시각적 설정")]
    public Color gizmoColor = Color.yellow;
    public float gizmoRadius = 1f;

    [Header("트리거 설정")]
    public bool requiresAllZombiesDead = false; // 모든 좀비를 처치해야 이동 가능한지 여부
    private bool isActivated = false;

    [Header("이펙트 설정")]
    public GameObject portalEffect; // 포탈 이펙트 (선택사항)
    public ParticleSystem activationParticle; // 활성화 시 파티클 (선택사항)

    void Start()
    {
        // 포탈 이펙트가 있다면 비활성화 상태로 시작
        if (portalEffect != null)
        {
            portalEffect.SetActive(!requiresAllZombiesDead);
        }

        if (!requiresAllZombiesDead)
        {
            isActivated = true;
        }
    }

    void Update()
    {
        // 모든 좀비를 처치해야 하는 경우, 좀비 수를 체크
        if (requiresAllZombiesDead && !isActivated)
        {
            CheckZombiesCleared();
        }
    }

    void CheckZombiesCleared()
    {
        // "Zombie" 태그를 가진 모든 오브젝트 찾기
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        if (zombies.Length == 0)
        {
            ActivateSpawnPoint();
        }
    }

    void ActivateSpawnPoint()
    {
        isActivated = true;
        Debug.Log("SpawnPoint 활성화! 이제 다음 씬으로 이동할 수 있습니다.");

        // 포탈 이펙트 활성화
        if (portalEffect != null)
        {
            portalEffect.SetActive(true);
        }

        // 활성화 파티클 재생
        if (activationParticle != null)
        {
            activationParticle.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 SpawnPoint에 도달했는지 확인
        if (other.CompareTag("Player") && isActivated)
        {
            Debug.Log("플레이어가 SpawnPoint에 도달! 다음 씬으로 이동합니다.");

            // GameManager를 통해 다음 씬 로드
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextScene();
            }
            else
            {
                Debug.LogError("GameManager를 찾을 수 없습니다!");
            }
        }
        else if (other.CompareTag("Player") && !isActivated)
        {
            Debug.Log("아직 모든 좀비를 처치하지 않았습니다!");
            // UI로 메시지 표시 가능
        }
    }

    // 에디터에서 SpawnPoint 위치를 시각적으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);

        // 화살표로 방향 표시
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);

        // 활성화 여부 표시
        if (!isActivated && requiresAllZombiesDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2.5f, Vector3.one * 0.5f);
        }
    }

    void OnDrawGizmosSelected()
    {
        // 선택되었을 때 더 큰 범위 표시
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}