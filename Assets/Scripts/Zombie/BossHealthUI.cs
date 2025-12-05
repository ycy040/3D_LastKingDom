using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider;

    void Start()
    {
        // 시작 시 강제 활성화
        gameObject.SetActive(true);
        Debug.Log("BossHealthBar 활성화됨");
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }
    }

    public void ShowHealthBar()
    {
        gameObject.SetActive(true);
    }

    public void HideHealthBar()
    {
        gameObject.SetActive(false);
    }
}