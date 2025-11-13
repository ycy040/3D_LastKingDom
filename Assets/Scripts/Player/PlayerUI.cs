using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthText;
    public GameObject gameoverText;

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }

        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHP).ToString();
        }
    }

    public void ShowGameOver()
    {
        if (gameoverText != null)
        {
            gameoverText.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void HideGameOver()
    {
        if (gameoverText != null)
        {
            gameoverText.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
