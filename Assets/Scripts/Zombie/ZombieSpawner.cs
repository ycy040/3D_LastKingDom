using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject zombiePrefab;   // 스폰할 좀비 프리팹
    public Transform player;          // 플레이어
    public int spawnCount = 5;        // 스폰할 좀비 수
    public Transform spawnPlane;      // Plane 오브젝트 지정

    private Vector3 areaCenter;
    private Vector3 areaSize;

    void Start()
    {
        if (spawnPlane != null)
        {
            // Plane의 중심과 크기 계산
            areaCenter = spawnPlane.position;

            // Plane scale에 따른 실제 크기 계산
            // Plane 기본 크기는 10x10 유닛
            Vector3 planeScale = spawnPlane.localScale;
            areaSize = new Vector3(10f * planeScale.x, 0f, 10f * planeScale.z);
        }
        else
        {
            Debug.LogError("Spawn Plane을 지정하세요!");
        }

        SpawnZombies(); // 게임 시작과 동시에 좀비 스폰
    }

    void SpawnZombies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            GameObject zombie = Instantiate(zombiePrefab, randomPos, Quaternion.identity);

            // 스폰 후 AI 초기화
            MonsterAI ai = zombie.GetComponent<MonsterAI>();
            if (ai != null)
            {
                ai.player = player; // 플레이어 지정
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float y = 0f; // Plane 높이
        float z = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);

        return areaCenter + new Vector3(x, y, z);
    }

    // 스폰 영역 시각화
    void OnDrawGizmosSelected()
    {
        if (spawnPlane != null)
        {
            Vector3 planeScale = spawnPlane.localScale;
            Vector3 size = new Vector3(10f * planeScale.x, 0f, 10f * planeScale.z);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(spawnPlane.position, size);
        }
    }
}
