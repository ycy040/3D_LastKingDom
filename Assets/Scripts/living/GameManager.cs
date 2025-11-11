using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameoverText;  // "Game Over" 텍스트 오브젝트
    public TMP_Text healthText;      // 체력 표시용 텍스트
    public Slider healthSlider;      // 체력 표시용 슬라이더
    public Player player;            // Player 스크립트 참조

    public bool isGameover;

    void Start()
    {
        isGameover = false;

        // 플레이어 사망 이벤트 연결
        if (player != null)
            player.onDeath += OnPlayerDeath;

        // 게임오버 UI 비활성화
        if (gameoverText != null)
            gameoverText.SetActive(false);

        // 슬라이더 초기화
        if (healthSlider != null && player != null)
        {
            healthSlider.maxValue = player.startingHealth;
            healthSlider.value = player.health;
        }
    }

    void Update()
    {
        if (!isGameover && player != null)
        {
            if (healthSlider != null)
            {
                healthSlider.value = player.health;
            }

            // 체력 텍스트 업데이트
            if (healthText != null)
                healthText.text = "" + Mathf.CeilToInt(player.health);
        }

        // 게임 오버 후 재시작 처리 (선택사항)
        if (isGameover && Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    void OnPlayerDeath()
    {
        isGameover = true;

        if (gameoverText != null)
        {
            // 게임 화면 멈춤
            Time.timeScale = 0f;
            gameoverText.SetActive(true);
        }

        Debug.Log("게임 오버!");
    }
}
