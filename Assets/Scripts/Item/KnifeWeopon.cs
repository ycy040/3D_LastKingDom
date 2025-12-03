using UnityEngine;

// ========== 칼 (근접 무기) ==========
public class KnifeWeapon : WeaponBase
{
    [Header("Attack Animation")]
    public bool useRotationAnimation = true;
    public float rotationSpeed = 10f; // 회전 속도

    private Vector3 originalRotation = new Vector3(25.037f, 77.865f, 61.947f);
    private Vector3 attackRotation = new Vector3(22.456f, 84.055f, 96.882f);
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        // 시작 시 현재 회전값 저장
        originalRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        base.Update();

        // 회전 애니메이션 처리
        if (useRotationAnimation && isAttacking)
        {
            // 공격 회전으로 보간
            transform.localEulerAngles = Vector3.Lerp(
                transform.localEulerAngles,
                attackRotation,
                Time.deltaTime * rotationSpeed
            );

            // 목표 회전에 거의 도달하면 원래 위치로 복귀 시작
            if (Vector3.Distance(transform.localEulerAngles, attackRotation) < 1f)
            {
                isAttacking = false;
            }
        }
        else if (useRotationAnimation && !isAttacking)
        {
            // 원래 회전으로 복귀
            transform.localEulerAngles = Vector3.Lerp(
                transform.localEulerAngles,
                originalRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    protected override void Attack()
    {
        Debug.Log("칼 공격!");

        // 회전 애니메이션 시작
        if (useRotationAnimation)
        {
            isAttacking = true;
        }

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Attack");

        // 1단계: 범위 내 적 찾기
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("범위 내에 적이 없습니다.");
            return;
        }

        // 2단계: 카메라 중심에서 레이캐스트
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
            return;
        }

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 화면 중심
        RaycastHit rayHit;

        // 레이캐스트로 조준한 적 찾기
        if (Physics.Raycast(ray, out rayHit, attackRange, enemyLayer))
        {
            // 3단계: 범위 내 + 조준한 적인지 확인
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.gameObject == rayHit.collider.gameObject)
                {
                    // 범위 안 + 조준함 = 타격 성공!
                    IDamageable damageable = enemy.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.OnDamage(damage, rayHit.point, rayHit.normal);
                        Debug.Log(enemy.name + "을(를) 정확히 타격! " + damage + " 피해!");
                        return; // 한 번만 타격
                    }
                }
            }

            Debug.Log("조준은 했지만 범위 밖입니다.");
        }
        else
        {
            Debug.Log("범위 내에 적이 있지만 조준하지 않았습니다.");
        }
    }
}