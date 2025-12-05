using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossZombieAI : MonoBehaviour, IDamageable
{
    public Transform player;
    public float followRange = 50f;
    public float attackRange = 2f;
    public float speed = 7f;
    public float attackDamage = 30f;
    public float attackCooldown = 1.5f;

    [Header("HP Settings")]
    public float maxHP = 200f;
    public float currentHP;

    [Header("Boss HP Bar")]
    public BossHealthUI bossHealthUI;

    public Animator anim;
    public Rigidbody rb;
    public float lastAttackTime = 0f;

    private bool isDead = false;

    public enum State
    {
        Idle,
        Follow,
        Attack,
        Dead
    }
    public State currentState = State.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator가 연결되지 않았습니다!");

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody가 연결되지 않았습니다!");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // HP 초기화
        currentHP = maxHP;

        // HP바 초기화
        if (bossHealthUI != null)
        {
            bossHealthUI.UpdateHealth(currentHP, maxHP);
            bossHealthUI.ShowHealthBar();
        }
        else
        {
            Debug.LogError("BossHealthUI가 연결되지 않았습니다!");
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 상태 결정
        if (distance <= attackRange)
            currentState = State.Attack;
        else if (distance <= followRange)
            currentState = State.Follow;
        else
            currentState = State.Idle;

        // 상태에 따른 행동
        switch (currentState)
        {
            case State.Idle:
                if (anim != null) anim.Play("Z_Idle");
                break;
            case State.Follow:
                if (anim != null) anim.Play("Z_Run_InPlace");
                MoveTowardsPlayer();
                break;
            case State.Attack:
                if (anim != null) anim.Play("Z_Attack");
                TryAttack();
                break;
        }
    }

    void LateUpdate()
    {
        // 더 이상 HP바 위치를 업데이트하지 않음
    }

    void MoveTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        Quaternion lookRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(lookRot);
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    void Attack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitPoint = player.position + Vector3.up * 1f;
                Vector3 hitNormal = (player.position - transform.position).normalized;
                damageable.OnDamage(attackDamage, hitPoint, hitNormal);
                Debug.Log("보스 좀비가 플레이어에게 " + attackDamage + " 피해를 입혔습니다!");
            }
        }
    }

    // IDamageable 인터페이스 구현
    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return;

        currentHP -= damage;
        Debug.Log("보스 좀비가 " + damage + " 피해를 입었습니다! 남은 HP: " + currentHP);

        // HP바 업데이트
        if (bossHealthUI != null)
        {
            bossHealthUI.UpdateHealth(currentHP, maxHP);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        currentState = State.Dead;

        // HP바 숨기기
        if (bossHealthUI != null)
        {
            bossHealthUI.HideHealthBar();
        }

        // 사망 애니메이션 재생
        if (anim != null)
        {
            anim.Play("Z_FallingBack");
        }

        // Rigidbody 비활성화
        rb.isKinematic = true;

        // 콜라이더 비활성화
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Debug.Log("보스 좀비가 사망했습니다!");

        // 일정 시간 후 오브젝트 제거
        Destroy(gameObject, 3f);
    }
}