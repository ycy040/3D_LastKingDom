using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;

    void Start()
    {
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
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.GetCurrentHP();

            if (healthBar != null)
                healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
        else
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthBar != null)
                healthBar.value = currentHealth;

            if (currentHealth <= 0f)
                Die();
        }
    }

    // IDamageable 인터페이스 구현
    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        TakeDamage(damage); // 기존 로직 그대로 사용
    }

    public void Heal(float amount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Heal(amount);
        }
        else
        {
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
            GameManager.Instance.PlayerDied();
        else
            gameObject.SetActive(false);
    }
}
