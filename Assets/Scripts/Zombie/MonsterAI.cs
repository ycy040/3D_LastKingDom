using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterAI : MonoBehaviour, IDamageable
{
    public Transform player;
    public float followRange = 10f;
    public float attackRange = 1.5f;
    public float speed = 5f;
    public float attackDamage = 5f;
    public float attackCooldown = 1.5f;

    [Header("HP Settings")]
    public float maxHP = 30f;
    public float currentHP;

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
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("플레이어를 자동으로 찾았습니다!");
            }
            else
            {
                Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }

        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogError("Animator가 연결되지 않았습니다!");

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody가 연결되지 않았습니다!");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // HP 초기화
        currentHP = maxHP;
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
                anim.Play("Z_Idle");
                break;
            case State.Follow:
                anim.Play("Z_Run_InPlace");
                MoveTowardsPlayer();
                break;
            case State.Attack:
                anim.Play("Z_Attack");
                TryAttack();
                break;
        }
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
                Debug.Log("플레이어에게 " + attackDamage + " 피해를 입혔습니다!");
            }
        }
    }

    // IDamageable 인터페이스 구현
    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return;

        currentHP -= damage;
        Debug.Log("좀비가 " + damage + " 피해를 입었습니다! 남은 HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        currentState = State.Dead;

        // 사망 애니메이션 재생 (애니메이션이 있다면)
        if (anim != null)
        {
            anim.Play("Z_FallingBack"); // 또는 "Z_Death" 등
        }

        // Rigidbody 비활성화
        rb.isKinematic = true;

        // 콜라이더 비활성화 (선택사항)
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Debug.Log("좀비가 사망했습니다!");

        // 일정 시간 후 오브젝트 제거
        GameManager.Instance.AddDeadZombieCount();
        Destroy(gameObject, 3f);
    }
}