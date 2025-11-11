using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar; // UI 슬라이더로 체력 표시할 때 사용 (없으면 제거해도 됨)

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.maxValue = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");
        // 여기서 사망 애니메이션이나 리스폰, 게임오버 처리 가능
    }
}
