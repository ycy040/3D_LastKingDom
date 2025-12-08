
using UnityEngine;

public class WeaponInteractionManager : MonoBehaviour
{
    [Header("설정")]
    public Transform weaponSlot;    // 무기가 장착될 손 위치 (WeaponSlot)
    public float rayDistance = 3f;  // 아이템 습득 거리
    public LayerMask itemLayer;     // 아이템 레이어 (설정 권장)

    [Header("키 설정")]
    public KeyCode pickupKey = KeyCode.E; // 상호작용 키

    private GameObject currentWeaponObj; // 현재 장착 중인 무기 오브젝트
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (weaponSlot == null) Debug.LogError("WeaponSlot이 연결되지 않았습니다! 인스펙터에서 할당해주세요.");
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupWeapon();
        }
    }

    private void TryPickupWeapon()
    {
        // 화면 중앙 Raycast
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 아이템 레이어에 닿았는지 확인
        if (Physics.Raycast(ray, out hit, rayDistance, itemLayer))
        {
            // 맞은 물체에 WeaponPickupData가 있는지 확인
            WeaponPickupData pickupData = hit.collider.GetComponent<WeaponPickupData>();

            if (pickupData != null)
            {
                // 이미 들고 있는 무기가 아닐 때만 줍기
                if (hit.collider.gameObject != currentWeaponObj)
                {
                    EquipWeapon(hit.collider.gameObject, pickupData);
                }
            }
        }
    }

    // 무기 장착 로직
    public void EquipWeapon(GameObject newWeapon, WeaponPickupData data)
    {
        // 1. 기존 무기가 있다면 버리기 (Drop)
        if (currentWeaponObj != null)
        {
            DropCurrentWeapon();
        }

        // 2. 새 무기 장착 처리
        currentWeaponObj = newWeapon;

        // 부모를 WeaponSlot으로 변경
        currentWeaponObj.transform.SetParent(weaponSlot);

        // 위치/회전/크기 데이터 적용
        currentWeaponObj.transform.localPosition = data.equipPosition;
        currentWeaponObj.transform.localRotation = Quaternion.Euler(data.equipRotation);
        currentWeaponObj.transform.localScale = data.equipScale;

        // 무기 상태 변경 (물리 끄기, 스크립트 켜기)
        data.OnEquip();

        Debug.Log($"{newWeapon.name} 장착 완료!");
    }

    // 현재 무기 버리기 로직
    private void DropCurrentWeapon()
    {
        if (currentWeaponObj == null) return;

        WeaponPickupData data = currentWeaponObj.GetComponent<WeaponPickupData>();

        // 부모 해제 (세상으로 내보냄)
        currentWeaponObj.transform.SetParent(null);

        // 플레이어 앞쪽으로 살짝 이동 (몸에 끼지 않게)
        currentWeaponObj.transform.position = transform.position + transform.forward * 1.0f + Vector3.up * 1.0f;

        // 무기 상태 변경 (물리 켜기, 스크립트 끄기)
        if (data != null)
        {
            data.OnDrop();

            // 살짝 앞으로 던지는 효과
            Rigidbody rb = currentWeaponObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * data.throwForce, ForceMode.Impulse);
            }
        }

        currentWeaponObj = null;
    }

    // 디버그용 (Ray 그리기)
    private void OnDrawGizmos()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam != null)
        {
            Gizmos.color = Color.cyan;
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Gizmos.DrawRay(ray.origin, ray.direction * rayDistance);
        }
    }
}