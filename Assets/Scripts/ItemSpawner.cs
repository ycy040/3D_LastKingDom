using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject weaponPrefab;
    public int spawnCount = 5;
    public Vector3 spawnArea = new Vector3(20f, 0f, 20f);

    // 추가: 무기가 생성될 높이를 지정합니다.
    public float spawnHeight = 50f;

    void Start()
    {
        SpawnWeapons();
    }

    void SpawnWeapons()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            float randomX = UnityEngine.Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
            float randomZ = UnityEngine.Random.Range(-spawnArea.z / 2, spawnArea.z / 2);

            // 스포너의 위치를 기준으로, 지정된 높이(spawnHeight)에 생성되도록 수정합니다.
            Vector3 spawnPos = transform.position + new Vector3(randomX, spawnHeight, randomZ);

            Instantiate(weaponPrefab, spawnPos, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, 0.1f, spawnArea.z));
    }
}