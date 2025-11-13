using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;

    void Start()
    {
        // GameManager에서 HP 가져오기
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
        // GameManager와 항상 동기화
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.GetCurrentHP();
            maxHealth = GameManager.Instance.GetMaxHP();

            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
            }
        }
    }

    // IDamageable 인터페이스 구현
    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        // GameManager를 통해서만 데미지 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
        else
        {
            // GameManager가 없으면 로컬 처리
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
        // GameManager를 통해서만 회복 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Heal(amount);
        }
        else
        {
            // GameManager가 없으면 로컬 처리
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthBar != null)
                healthBar.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}