using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("리스폰 설정")]
    public Vector3 currentSavePoint; // 현재 세이브 포인트 위치
    public int currentSceneIndex; // 현재 씬 인덱스
    public int maxRespawnCount = 3; // 최대 리스폰 기회
    public int currentRespawnCount; // 현재 남은 리스폰 기회

    [Header("체력 설정")]
    public float maxHP = 100f;
    public float currentHP;

    public bool isGameOver = false;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentHP = maxHP;
        currentRespawnCount = maxRespawnCount;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeScene();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        currentHP = maxHP;
        currentRespawnCount = maxRespawnCount;
        isGameOver = false;

        Debug.Log($"새로운 씬 로드 완료: {scene.name}, HP 전량 회복! 리스폰 기회: {currentRespawnCount}/{maxRespawnCount}");
        InitializeScene();
    }

    void InitializeScene()
    {
        SavePoint savePoint = FindObjectOfType<SavePoint>();
        if (savePoint != null)
        {
            currentSavePoint = savePoint.transform.position;
            MovePlayerToSavePoint();
        }
        else
        {
            Debug.LogWarning("SavePoint를 찾을 수 없습니다!");
        }
    }

    void MovePlayerToSavePoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = currentSavePoint;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = currentSavePoint;
            }

            Debug.Log($"플레이어를 SavePoint로 이동: {currentSavePoint}");
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isGameOver) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        Debug.Log($"플레이어 피격! 현재 HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
            PlayerDied();
    }

    public void Heal(float amount)
    {
        if (isGameOver) return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        Debug.Log($"HP 회복! 현재 HP: {currentHP}/{maxHP}");
    }

    public void PlayerDied()
    {
        currentRespawnCount--;
        Debug.Log($"플레이어 사망! 남은 기회: {currentRespawnCount}/{maxRespawnCount}");

        if (currentRespawnCount > 0)
        {
            currentHP = maxHP;
            Debug.Log("SavePoint에서 리스폰합니다...");
            Invoke("MovePlayerToSavePoint", 0.5f);
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        isGameOver = true;

        // UI는 PlayerUI에서 처리하도록 분리
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 씬입니다. 게임 클리어!");
        }
    }

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public int GetRespawnCount() => currentRespawnCount;
    public void UpdateSavePoint(Vector3 newSavePoint) => currentSavePoint = newSavePoint;
}
