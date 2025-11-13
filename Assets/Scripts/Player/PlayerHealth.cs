using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar; // UI 슬라이더로 체력 표시할 때 사용 (없으면 제거해도 됨)

    void Start()
    {
        // GameManager가 있으면 GameManager의 HP를 사용
        if (GameManager.Instance != null)
        {
            maxHealth = GameManager.Instance.GetMaxHP();
            currentHealth = GameManager.Instance.GetCurrentHP();
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        // GameManager와 HP 동기화
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.GetCurrentHP();

            if (healthBar != null)
            {
                healthBar.value = currentHealth;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // GameManager를 통해 데미지 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
        else
        {
            // GameManager가 없으면 직접 처리
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthBar != null)
                healthBar.value = currentHealth;

            if (currentHealth <= 0f)
                Die();
        }
    }

    public void Heal(float amount)
    {
        // GameManager를 통해 회복 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Heal(amount);
        }
        else
        {
            // GameManager가 없으면 직접 처리
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthBar != null)
                healthBar.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");

        // GameManager를 통해 사망 처리 (리스폰 시스템)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            // GameManager가 없으면 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}