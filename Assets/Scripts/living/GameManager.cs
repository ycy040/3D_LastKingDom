using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("리스폰 설정")]
    public Vector3 currentSavePoint;
    public int currentSceneIndex;

    [Header("체력 설정 - 유일한 HP 관리")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("UI 참조")]
    public PlayerUI playerUI; // PlayerUI 참조 추가
    public Slider healthSlider;
    public TMP_Text healthText;
    public TMP_Text respawnCountText;
    public GameObject gameoverText;

    [Header("상태")]
    public bool isGameOver = false;
    public bool isRespawning = false; // 리스폰 중 여부
    public float respawnInvincibleTime = 2f; // 리스폰 후 무적 시간

    // HP 변경 이벤트
    public event System.Action<float, float> onHealthChanged;

    void Awake()
    {
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
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeScene();
        FindUIElements();
        UpdateUI();
    }

    void Update()
    {
        // 매 프레임 UI 업데이트
        UpdateUI();

        // 게임 오버 후 재시작
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        currentHP = maxHP;
        isGameOver = false;
        Time.timeScale = 1f;

        Debug.Log($"새로운 씬 로드: {scene.name}, HP 전량 회복!");
        InitializeScene();
        FindUIElements();
        UpdateUI();
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

    void FindUIElements()
    {
        // PlayerUI 자동 찾기
        if (playerUI == null)
        {
            playerUI = FindObjectOfType<PlayerUI>();
        }

        // 개별 UI 요소 자동 찾기
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();
        }
        if (healthText == null)
        {
            healthText = GameObject.Find("HealthText")?.GetComponent<TMP_Text>();
        }
        if (respawnCountText == null)
        {
            respawnCountText = GameObject.Find("RespawnCountText")?.GetComponent<TMP_Text>();
        }
        if (gameoverText == null)
        {
            gameoverText = GameObject.Find("GameOverText");
        }

        if (gameoverText != null)
        {
            gameoverText.SetActive(false);
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

            // 플레이어 컴포넌트들과 HP 동기화
            SyncPlayerHealth(player);
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    void SyncPlayerHealth(GameObject player)
    {
        // LivingEntity 동기화
        LivingEntity living = player.GetComponent<LivingEntity>();
        if (living != null)
        {
            living.health = currentHP;
            living.dead = false;
        }

        // Player 동기화
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.health = currentHP;
            playerScript.dead = false;
        }

        // PlayerHealth 동기화
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.currentHealth = currentHP;
        }
    }

    // ========== HP 관리 함수 (유일한 HP 제어) ==========
    public void TakeDamage(float damage)
    {
        // 게임 오버 중이거나 리스폰 중이면 데미지 무시
        if (isGameOver || isRespawning) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        Debug.Log($"플레이어 피격! 현재 HP: {currentHP}/{maxHP}");

        // 모든 플레이어 컴포넌트 동기화
        SyncAllPlayerComponents();

        // UI 업데이트 (즉시!)
        UpdateUI();

        // 이벤트 발생
        onHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            PlayerDied();
        }
    }

    public void Heal(float amount)
    {
        if (isGameOver) return;

        float oldHP = currentHP;
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);

        float actualHealed = currentHP - oldHP;
        Debug.Log($"HP 회복! +{actualHealed} (현재 HP: {currentHP}/{maxHP})");

        // 모든 플레이어 컴포넌트 동기화
        SyncAllPlayerComponents();

        // UI 업데이트 (즉시!)
        UpdateUI();

        // 이벤트 발생
        onHealthChanged?.Invoke(currentHP, maxHP);
    }

    void SyncAllPlayerComponents()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SyncPlayerHealth(player);
        }
    }

    // ========== UI 업데이트 ==========
    void UpdateUI()
    {
        // PlayerUI를 통한 업데이트
        if (playerUI != null)
        {
            playerUI.UpdateHealth(currentHP, maxHP);
        }

        // 개별 UI 요소 직접 업데이트
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }

        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHP).ToString();
        }

        // 리스폰 기회 텍스트 제거
    }

    // ========== 사망 처리 ==========
    public void PlayerDied()
    {
        // 이미 게임 오버 중이면 무시
        if (isGameOver) return;

        Debug.Log("플레이어 사망! 게임 오버");
        GameOver();
    }

    void StartRespawn()
    {
        isRespawning = true;

        Debug.Log("플레이어 비활성화 및 리스폰 시작...");

        // 플레이어 완전히 비활성화
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }

        // 1초 후 리스폰
        Invoke("CompleteRespawn", 1f);
    }

    void CompleteRespawn()
    {
        // HP 회복
        currentHP = maxHP;

        // 플레이어 찾기 (비활성화된 오브젝트 포함)
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        GameObject player = null;

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player") && obj.scene.isLoaded)
            {
                player = obj;
                break;
            }
        }

        if (player != null)
        {
            // SavePoint로 이동
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

            // HP 동기화
            SyncPlayerHealth(player);

            // 플레이어 활성화
            player.SetActive(true);

            Debug.Log($"플레이어를 SavePoint로 이동: {currentSavePoint}");
        }

        Debug.Log("리스폰 완료!");
        isRespawning = false;
    }

    void EndRespawnInvincibility()
    {
        isRespawning = false;
        Debug.Log("무적 상태 종료!");
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - R키를 눌러 재시작");
        isGameOver = true;
        Time.timeScale = 0f;

        // PlayerUI를 통한 게임 오버 표시
        if (playerUI != null)
        {
            playerUI.ShowGameOver();
        }
        else if (gameoverText != null)
        {
            gameoverText.SetActive(true);
        }
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        currentHP = maxHP;
        isGameOver = false;

        // PlayerUI를 통한 게임 오버 숨김
        if (playerUI != null)
        {
            playerUI.HideGameOver();
        }

        SceneManager.LoadScene(currentSceneIndex);
    }

    // ========== 씬 전환 ==========
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

    // ========== Getter 함수 ==========
    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public void UpdateSavePoint(Vector3 newSavePoint) => currentSavePoint = newSavePoint;
}