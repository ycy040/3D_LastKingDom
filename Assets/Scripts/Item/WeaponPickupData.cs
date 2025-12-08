
using UnityEngine;

public class WeaponPickupData : MonoBehaviour
{
    [Header("장착 시 트랜스폼 설정")]
    public Vector3 equipPosition = Vector3.zero;
    public Vector3 equipRotation = Vector3.zero; // 여기가 장착될 때의 각도입니다.
    public Vector3 equipScale = Vector3.one;

    [Header("물리 설정")]
    public float throwForce = 5f;

    private Rigidbody rb;
    private Collider col;
    private WeaponBase weaponScript;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        weaponScript = GetComponent<WeaponBase>();
    }

    void Start()
    {
        // 시작 시 공격 스크립트 끄기 & 물리 회전 제거
        if (weaponScript != null) weaponScript.enabled = false;
        if (rb != null) rb.angularVelocity = Vector3.zero;
    }

    public void OnEquip()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (col != null) col.enabled = false;
        if (weaponScript != null) weaponScript.enabled = true;
    }

    public void OnDrop()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (col != null) col.enabled = true;
        if (weaponScript != null) weaponScript.enabled = false;
    }

    // ========================================================
    // ★ [새로운 기능] 현재 트랜스폼 값을 바로 데이터로 저장하는 버튼
    // ========================================================
    [ContextMenu("현재 위치/각도를 장착 데이터로 저장")]
    void SaveCurrentTransform()
    {
        equipPosition = transform.localPosition;
        equipRotation = transform.localEulerAngles;
        equipScale = transform.localScale;

        Debug.Log($"저장 완료! Pos: {equipPosition}, Rot: {equipRotation}");
    }
}