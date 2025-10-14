using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public float followRange = 10f;   // 추적 시작 거리
    public float attackRange = 1.5f;    // 공격 시작 거리
    public float speed = 5f;

    private Animator anim;
    private Rigidbody rb;

    private enum State
    {
        Idle,
        Follow,
        Attack
    }

    private State currentState = State.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogError("Animator가 연결되지 않았습니다!");

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody가 연결되지 않았습니다!");

        // 앞으로 쓰러지지 않게 회전 고정
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
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
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        // 방향 계산
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        // Rigidbody 이동
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        // y축 회전만 적용
        Quaternion lookRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(lookRot);
    }
}
