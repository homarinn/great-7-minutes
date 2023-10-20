using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField, Range(0f, 1000f), Tooltip("ランダム移動の範囲")]
    float wanderRange;

    [SerializeField, Range(0f, 60f), Tooltip("ランダム移動中のインターバル秒数")]
    float wanderWaitTime;

    [SerializeField, Range(0f, 100f), Tooltip("ランダム移動中の最小移動距離")]
    float minMoveDistance;

    [SerializeField, Range(0f, 100f), Tooltip("追跡対象と認識する距離")]
    float targetPlayerDistance;

    [SerializeField, Range(0f, 100f), Tooltip("追跡対象から外れる距離")]
    float removeTargetPlayerDistance;

    [SerializeField, Range(0f, 1000f), Tooltip("攻撃可能な距離")]
    float attackDistance;

    [SerializeField, Range(0f, 10f), Tooltip("攻撃してから次のAIを実行できるまでの時間")]
    float attackInterval;

    [SerializeField, Range(0f, 10000f), Tooltip("AIを動かすPlayerとの距離")]
    float runAiDistance;

    public enum State
    {
        DONOTHING,
        IDLE,
        WANDER,
        CHASE,
        RESETPOSITION,
        ATTACK,
        DEFEND
    }

    State currentState = State.DONOTHING;

    bool isInsideCamera;
    public bool IsStoppigChangeState { get; set; } = false;
    bool canAction = true;

    Coroutine wanderCoroutine = null;

    Vector3 centerPosition;

    [System.NonSerialized]
    public Animator animator;

    GameObject player;

    Transform transformCache;
    Transform playerTransformCache;

    bool IsWandering { get { return currentState == State.WANDER; } }
    bool IsChasing { get { return currentState == State.CHASE; } }
    bool IsResetting { get { return currentState == State.RESETPOSITION; } }
    public bool IsAttacking { get { return currentState == State.ATTACK; } }

    NavMeshAgent navMeshAgent;
    bool IsFinishNav
    {
        get { return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance; }
    }

    public void ChangeState(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transformCache = transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransformCache = player.transform;
        centerPosition = transformCache.position;
    }

    void FixedUpdate()
    {
        UpdateState();
    }

    void UpdateState()
    {
        SetState();

        if (!IsWandering)
        {
            BreakCoroutine(ref wanderCoroutine);
        }

        if (!canAction)
        {
            return;
        }

        switch (currentState)
        {
            case State.DONOTHING:
                DoNothing();
                break;
            case State.IDLE:
                Idle();
                break;
            case State.WANDER:
                Wander();
                break;
            case State.CHASE:
                Chase();
                return;
            case State.RESETPOSITION:
                ResetPosition();
                return;
            case State.ATTACK:
                Attack();
                break;
            case State.DEFEND:
                Defend();
                break;
        }
    }

    IEnumerator StopChangeState(Func<bool> callback)
    {
        IsStoppigChangeState = true;
        while (!callback())
        {
            yield return null;
        }
        IsStoppigChangeState = false;
    }

    IEnumerator StopChangeState(float time)
    {
        IsStoppigChangeState = true;
        yield return new WaitForSeconds(time);
        IsStoppigChangeState = false;
    }

    IEnumerator StopAction(float time)
    {
        canAction = false;
        yield return new WaitForSeconds(time);
        canAction = true;
    }

    void SetState()
    {
        if (IsStoppigChangeState)
        {
            return;
        }

        if (IsResetting)
        {
            if (IsFinishNav)
            {
                ChangeState(State.IDLE);
            }

            return;
        }

        float targetDistanceSquare = (transform.position - playerTransformCache.position).sqrMagnitude;

        if ((IsChasing || IsAttacking) && targetDistanceSquare <= attackDistance * attackDistance)
        {
            ChangeState(State.ATTACK);
        }
        else if ((IsChasing || IsAttacking) && targetDistanceSquare >= removeTargetPlayerDistance * removeTargetPlayerDistance)
        {
            ChangeState(State.RESETPOSITION);
        }
        else if (targetDistanceSquare <= targetPlayerDistance * targetPlayerDistance)
        {
            ChangeState(State.CHASE);
        }
        else if (isInsideCamera || targetDistanceSquare <= runAiDistance * runAiDistance)
        {
            ChangeState(State.WANDER);
        }
        else
        {
            ChangeState(State.DONOTHING);
        }
    }

    void DoNothing()
    {
        if (navMeshAgent.hasPath)
        {
            navMeshAgent.ResetPath();
        }
        animator.SetFloat("MoveSpeed", 0);
    }

    void Idle()
    {
        if (navMeshAgent.hasPath)
        {
            navMeshAgent.ResetPath();
        }
        animator.SetFloat("MoveSpeed", 0);
    }

    void Wander()
    {
        if (wanderCoroutine != null)
        {
            return;
        }

        if (IsFinishNav)
        {
            //hasPath エージェントが経路を持っているかどうか
            //navMeshAgent.velocity.sqrMagnitudeはスピード
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                wanderCoroutine = StartCoroutine(WanderInterval());
            }
        }
    }

    void Chase()
    {
        if (navMeshAgent.pathPending)
        {
            return;
        }

        animator.SetFloat("MoveSpeed", 2);
        navMeshAgent.SetDestination(playerTransformCache.position);
    }

    void ResetPosition()
    {
        navMeshAgent.SetDestination(centerPosition);

        Func<bool> func = () => {
            return IsFinishNav;
        };

        StartCoroutine(StopChangeState(func));
    }

    void Attack()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(playerTransformCache.position - transform.position), 600f * Time.fixedDeltaTime);
        if (!IsStoppigChangeState)
        {
            animator.SetFloat("MoveSpeed", 0);
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
            }
            StartCoroutine(AttackInterval());
            StartCoroutine(StopChangeState(attackInterval));
            StartCoroutine(StopAction(attackInterval));
        }
    }

    IEnumerator AttackInterval()
    {
        yield return new WaitForSeconds(attackInterval);
        animator.SetFloat("MoveSpeed", 0);
        if (navMeshAgent.hasPath)
        {
            navMeshAgent.ResetPath();
        }

        if ((transform.position - playerTransformCache.position).sqrMagnitude < attackDistance * attackDistance)
        {
            animator.SetTrigger("Attack");
        }
    }



    void Defend()
    {
        return;
    }

    IEnumerator WanderInterval()
    {
        animator.SetFloat("MoveSpeed", 0);
        yield return new WaitForSeconds(wanderWaitTime);

        float xDestinationPosition;
        float zDestinationPosition;

        float xDistanceFromCenter = UnityEngine.Random.Range(-wanderRange, wanderRange);
        xDestinationPosition = centerPosition.x + xDistanceFromCenter;

        float zMaxDistanceFromCenter = Mathf.Sqrt(wanderRange * wanderRange - xDistanceFromCenter * xDistanceFromCenter);

        float zMaxPosition = centerPosition.z + zMaxDistanceFromCenter;
        float zMinPosition = centerPosition.z - zMaxDistanceFromCenter;

        float xMoveDistance = Mathf.Abs(xDestinationPosition - transform.position.x);
        if (xMoveDistance < minMoveDistance)
        {
            float zMoveMinDistance = Mathf.Sqrt(minMoveDistance * minMoveDistance - xMoveDistance * xMoveDistance);

            float zLowPosition = transform.position.z - zMoveMinDistance;
            float zHighPosition = transform.position.z + zMoveMinDistance;


            if (zLowPosition < zMinPosition)
            {
                zDestinationPosition = UnityEngine.Random.Range(zHighPosition, zMaxPosition);
            }
            else if (zHighPosition > zMaxPosition)
            {
                zDestinationPosition = UnityEngine.Random.Range(zMinPosition, zLowPosition);
            }
            else
            {
                // よりランダムな移動を実現するための確率計算
                float zLessRange = zLowPosition - zMinPosition;
                float zGreaterRange = zMaxPosition - zHighPosition;
                if (zLessRange / (zLessRange + zGreaterRange) >= UnityEngine.Random.Range(0f, 1f))
                {
                    zDestinationPosition = UnityEngine.Random.Range(zMinPosition, zLowPosition);
                }
                else
                {
                    zDestinationPosition = UnityEngine.Random.Range(zHighPosition, zMaxPosition);
                }
            }
        }
        else
        {
            zDestinationPosition = UnityEngine.Random.Range(zMinPosition, zMaxPosition);
        }

        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(new Vector3(xDestinationPosition, transform.position.y, zDestinationPosition), out navMeshHit, 5, 1);

        navMeshAgent.SetDestination(navMeshHit.position);
        animator.SetFloat("MoveSpeed", 1);

        BreakCoroutine(ref wanderCoroutine);
    }

    void BreakCoroutine(ref Coroutine Coroutine)
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }

    void OnBecameVisible()
    {
        isInsideCamera = true;
    }

    void OnBecameInvisible()
    {
        isInsideCamera = false;
    }
}
