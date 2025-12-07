using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Header("아이콘 설정")]
    public Color iconColor = Color.green;
    public float iconSize = 2f;
    public float iconHeight = 45f; // 미니맵 카메라보다 약간 아래

    private GameObject iconInstance;

    void Start()
    {
        CreateIcon();
    }

    void CreateIcon()
    {
        // 원형 아이콘 생성 (Sphere)
        iconInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        iconInstance.name = gameObject.name + "_MinimapIcon";

        // 납작한 원 모양으로
        iconInstance.transform.localScale = new Vector3(iconSize, 0.3f, iconSize);

        // 머티리얼 설정 (발광 효과)
        Renderer renderer = iconInstance.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = iconColor;
        renderer.material = mat;

        // 콜라이더 제거 (충돌 방지)
        Collider col = iconInstance.GetComponent<Collider>();
        if (col != null)
            Destroy(col);

        Debug.Log($"{gameObject.name}의 미니맵 아이콘 생성: {iconColor}");
    }

    void LateUpdate()
    {
        if (iconInstance != null)
        {
            // 아이콘을 오브젝트 위에 위치시킴
            iconInstance.transform.position = new Vector3(
                transform.position.x,
                iconHeight,
                transform.position.z
            );

            // 항상 카메라를 바라보도록 (선택사항)
            iconInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴되면 아이콘도 제거
        if (iconInstance != null)
            Destroy(iconInstance);
    }

    // Scene View에서 아이콘 위치 확인
    void OnDrawGizmos()
    {
        Gizmos.color = iconColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, iconHeight, transform.position.z), iconSize / 2f);
    }
}