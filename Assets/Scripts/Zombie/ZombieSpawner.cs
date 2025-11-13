using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject zombiePrefab;   // 스폰할 좀비 프리팹
    public Transform player;          // 플레이어 Transform
    public int spawnCount = 5;        // 스폰할 좀비 수
    public Vector3 spawnArea = new Vector3(200f, 0f, 300f); // 스폰 영역 크기

    private Vector3 areaCenter;

    void Start()
    {
        areaCenter = transform.position; // 스폰 기준은 스크립트가 붙은 오브젝트 위치
        SpawnZombies();
    }

    void SpawnZombies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            GameObject zombie = Instantiate(zombiePrefab, randomPos, Quaternion.identity);

            // 좀비 AI 초기화
            MonsterAI ai = zombie.GetComponent<MonsterAI>();
            if (ai != null)
                ai.player = player;
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
        float y = 0f; // 높이 고정 (필요시 조정 가능)
        float z = Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f);

        return areaCenter + new Vector3(x, y, z);
    }

    // 스폰 영역 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
