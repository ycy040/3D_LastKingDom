using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapEnemyTracker : MonoBehaviour
{
    [Header("참조")]
    public RectTransform minimapRect; // Minimap RawImage
    public GameObject enemyIconPrefab; // UI Image 프리팹

    [Header("설정")]
    public float mapSize = 120f; // 미니맵 카메라의 orthographicSize와 같게

    private Dictionary<GameObject, GameObject> enemyIcons = new Dictionary<GameObject, GameObject>();

    void Update()
    {
       
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