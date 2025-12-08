using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayerIcon : MonoBehaviour
{
    [Header("참조")]
    public RectTransform minimapRect; // Minimap RawImage
    public Transform playerTransform; // 플레이어 Transform

    [Header("설정")]
    public Color playerColor = Color.green;
    public float iconSize = 12f;
    public float mapSize = 120f; // MinimapEnemyTracker의 mapSize와 동일하게

    private GameObject playerIcon;
    private RectTransform iconRect;

    void Start()
    {
        // playerTransform이 할당되지 않았으면 자동으로 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        CreatePlayerIcon();
    }

    void CreatePlayerIcon()
    {
        // 아이콘 생성
        playerIcon = new GameObject("PlayerIcon");
        playerIcon.transform.SetParent(minimapRect);

        // Image 컴포넌트 추가
        Image img = playerIcon.AddComponent<Image>();
        img.color = playerColor;

        // RectTransform 설정
        iconRect = playerIcon.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(iconSize, iconSize);

        // 미니맵 중앙 기준으로 설정
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero; // 초기 위치는 중앙

        Debug.Log("플레이어 미니맵 아이콘 생성 완료");
    }

    void Update()
    {
        if (playerTransform != null && iconRect != null)
        {
            UpdatePlayerIconPosition();
        }
    }

    void UpdatePlayerIconPosition()
    {
        // 월드 원점(0, 0, 0)을 기준으로 플레이어의 위치 계산
        Vector3 playerPos = playerTransform.position;

        // 미니맵 좌표로 변환
        float x = (playerPos.x / mapSize) * (minimapRect.rect.width / 2f);
        float y = (playerPos.z / mapSize) * (minimapRect.rect.height / 2f);

        iconRect.anchoredPosition = new Vector2(x, y);
    }
}