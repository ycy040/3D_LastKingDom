using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("리스폰 설정")]
    private Vector3 currentSavePoint; // 현재 세이브 포인트 위치
    private int currentSceneIndex; // 현재 씬 인덱스
    public int maxRespawnCount = 3; // 최대 리스폰 기회
    private int currentRespawnCount; // 현재 남은 리스폰 기회

    [Header("체력 설정")]
    public float maxHP = 100f;
    private float currentHP;

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
        currentRespawnCount = maxRespawnCount; // 리스폰 기회 초기화
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 첫 씬 시작 시 플레이어 위치 설정
        InitializeScene();
    }

    void OnDestroy()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;

        // 새로운 씬으로 넘어왔을 때 HP 전량 회복 & 리스폰 기회 초기화
        currentHP = maxHP;
        currentRespawnCount = maxRespawnCount;
        Debug.Log($"새로운 씬 로드 완료: {scene.name}, HP 전량 회복! 리스폰 기회: {currentRespawnCount}/{maxRespawnCount}");

        InitializeScene();
    }

    // 씬 초기화 (플레이어를 SavePoint로 이동)
    void InitializeScene()
    {
        // SavePoint 찾기
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

    // 플레이어를 SavePoint 위치로 이동
    void MovePlayerToSavePoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // CharacterController가 있는 경우 비활성화 후 이동
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

            // 플레이어의 HP를 현재 HP로 설정
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.currentHealth = currentHP;
                if (playerHealth.healthBar != null)
                {
                    playerHealth.healthBar.value = currentHP;
                }
            }
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다! Player 태그를 확인하세요.");
        }
    }

    // 다음 씬으로 이동
    public void LoadNextScene()
    {
        int nextSceneIndex = currentSceneIndex + 1;

        // 다음 씬이 존재하는지 확인
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"다음 씬으로 이동: Scene {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 씬입니다. 게임 클리어!");
            // 게임 클리어 처리 (엔딩 씬 등)
        }
    }

    // 플레이어 사망 처리
    public void PlayerDied()
    {
        currentRespawnCount--; // 리스폰 기회 차감

        Debug.Log($"플레이어 사망! 남은 기회: {currentRespawnCount}/{maxRespawnCount}");

        if (currentRespawnCount > 0)
        {
            // 리스폰 기회가 남아있으면 부활
            currentHP = maxHP; // HP 전량 회복
            Debug.Log("SavePoint에서 리스폰합니다...");

            // 잠시 대기 후 리스폰 (선택사항)
            Invoke("MovePlayerToSavePoint", 0.5f);
        }
        else
        {
            // 리스폰 기회를 모두 소진하면 게임 오버
            Debug.Log("게임 오버! 리스폰 기회를 모두 소진했습니다.");
            Invoke("GameOver", 1f);
        }
    }

    // 게임 오버 처리
    void GameOver()
    {
        Debug.Log("GAME OVER");
        // 게임 오버 UI 표시
        // 타이틀 화면으로 이동하거나 현재 씬 재시작

        // 옵션 1: 현재 씬 재시작
        currentRespawnCount = maxRespawnCount;
        currentHP = maxHP;
        SceneManager.LoadScene(currentSceneIndex);

        // 옵션 2: 타이틀 씬으로 이동
        // SceneManager.LoadScene(0);
    }

    // HP 관리 함수들
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP); // 0 이하로 내려가지 않도록

        Debug.Log($"플레이어 피격! 현재 HP: {currentHP}/{maxHP}");

        // 플레이어의 HP UI 업데이트
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.currentHealth = currentHP;
                if (playerHealth.healthBar != null)
                {
                    playerHealth.healthBar.value = currentHP;
                }
            }
        }

        if (currentHP <= 0)
        {
            PlayerDied();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP); // 최대 HP를 넘지 않도록
        Debug.Log($"HP 회복! 현재 HP: {currentHP}/{maxHP}");

        // 플레이어의 HP UI 업데이트
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.currentHealth = currentHP;
                if (playerHealth.healthBar != null)
                {
                    playerHealth.healthBar.value = currentHP;
                }
            }
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }

    public int GetRespawnCount()
    {
        return currentRespawnCount;
    }

    // 현재 세이브 포인트 업데이트
    public void UpdateSavePoint(Vector3 newSavePoint)
    {
        currentSavePoint = newSavePoint;
        Debug.Log($"세이브 포인트 업데이트: {newSavePoint}");
    }
}