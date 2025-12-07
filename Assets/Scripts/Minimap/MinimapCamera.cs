using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapEnemyTracker : MonoBehaviour
{
    [Header("참조")]
    public RectTransform minimapRect; // Minimap RawImage
    public GameObject enemyIconPrefab; // UI Image 프리팹

    [Header("설정")]
    public float mapSize = 20f; // 미니맵 카메라의 orthographicSize와 같게
    public Color enemyColor = Color.red;
    public float iconSize = 10f;

    private Dictionary<GameObject, GameObject> enemyIcons = new Dictionary<GameObject, GameObject>();

    void Update()
    {
        // 모든 Enemy 찾기
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 새로운 Enemy 아이콘 생성
        foreach (GameObject enemy in enemies)
        {
            if (!enemyIcons.ContainsKey(enemy))
            {
                CreateEnemyIcon(enemy);
            }
        }

        // 아이콘 위치 업데이트 및 죽은 Enemy 제거
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var pair in enemyIcons)
        {
            GameObject enemy = pair.Key;
            GameObject icon = pair.Value;

            if (enemy == null)
            {
                Destroy(icon);
                toRemove.Add(enemy);
            }
            else
            {
                UpdateIconPosition(enemy.transform, icon.GetComponent<RectTransform>());
            }
        }

        // 죽은 Enemy 딕셔너리에서 제거
        foreach (GameObject enemy in toRemove)
        {
            enemyIcons.Remove(enemy);
        }
    }

    void CreateEnemyIcon(GameObject enemy)
    {
        GameObject icon = new GameObject(enemy.name + "_Icon");
        icon.transform.SetParent(minimapRect);

        Image img = icon.AddComponent<Image>();
        img.color = enemyColor;

        RectTransform rect = icon.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(iconSize, iconSize);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        enemyIcons.Add(enemy, icon);
    }

    void UpdateIconPosition(Transform enemy, RectTransform iconRect)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Enemy와 Player의 상대 위치 계산
        Vector3 offset = enemy.position - player.transform.position;

        // 미니맵 좌표로 변환
        float x = (offset.x / mapSize) * (minimapRect.rect.width / 2f);
        float y = (offset.z / mapSize) * (minimapRect.rect.height / 2f);

        iconRect.anchoredPosition = new Vector2(x, y);
    }
}