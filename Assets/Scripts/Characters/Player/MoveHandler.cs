using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;

public class MoveHandler : MonoBehaviour
{
    const string MoveSkillName = "Move";

    public float speed;

    public bool CanMove { get; set; } = true;
    public bool IsDeceleration { get; set; } = false;

    public float StopStartTime { get; set; }
    public float DecelerationTime { get; set; } = 1f;

    [System.NonSerialized]
    public Vector3 lastVelocity = Vector3.zero;

    bool isGround = true;
    bool isFall = false;
    float fallTime = 0f;

    public LayerMask groundLayer;

    public Transform avatarTransform;

    public float accelerateSpeed;
    bool isAcceleration = false;
    Vector3 accelerateVector;

    [System.NonSerialized]
    public Rigidbody rigidBody;

    [System.NonSerialized]
    public Animator animator;

    public Transform transformCache;
    Transform mainCameraTransform;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (isAcceleration)
        {
            rigidBody.velocity = accelerateVector;
        }

    }

    public void Move()
    {
        if (animator.GetBool("IsAcceleration"))
        {
            return;
        }

        SetIsGround();
        SetIsFall();

        if (isGround)
        {
            fallTime = 0f;
        } else
        {
            fallTime += Time.fixedDeltaTime;
        }

        if (!CanMove)
        {
            lastVelocity = Vector3.zero;
            rigidBody.velocity = new Vector3(0, Physics.gravity.y * fallTime, 0);
            animator.SetBool("IsFalling", isFall);

            return;
        }

        if (IsDeceleration)
        {
            // 停止時の制御
            StopStartTime += Time.fixedDeltaTime;

            if (StopStartTime >= DecelerationTime)
            {
                lastVelocity = Vector3.zero;
            }
            else
            {
                // 徐々に減速
                lastVelocity *= 0.97f;
            }
            lastVelocity.y += Physics.gravity.y * fallTime;
            rigidBody.velocity = lastVelocity;
            return;
        }

        Vector3 moveVector = GetMoveVector();
        float moveMagnitude = moveVector.magnitude;

        // アニメーション再生
        if (isFall)
        {
            animator.SetBool("IsFalling", true);
            animator.SetFloat("MoveSpeed", 0);
        } else
        {
            animator.SetBool("IsFalling", false);
            animator.SetFloat("MoveSpeed", moveMagnitude);
        }

        if (moveMagnitude == 0)
        {
            // 停止時の制御
            StopStartTime += Time.fixedDeltaTime;

            if (StopStartTime >= 0.24f)
            {
                lastVelocity = Vector3.zero;
            }
            else
            {
                lastVelocity *= 0.8f;
            }
        }
        else
        {
            // 移動時の制御
            StopStartTime = 0;

            // 操作キャラクターの移動速度決定
            Vector3 accelVelocity = speed / 9 * moveVector;
            if ((lastVelocity + accelVelocity).magnitude < speed)
            {
                // 徐々に加速
                lastVelocity += accelVelocity;
            }
            else
            {
                lastVelocity = speed * moveVector.normalized;
            }

            // 操作キャラクターの回転
            if (moveVector.magnitude > 0)
            {
                avatarTransform.localRotation = Quaternion.Lerp(avatarTransform.localRotation, Quaternion.LookRotation(moveVector), 600f * Time.fixedDeltaTime);
            }
        }

        // キャラクターの移動
        lastVelocity.y += Physics.gravity.y * fallTime;
        rigidBody.velocity = lastVelocity;
    }

    public Vector3 GetMoveVector()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // キャラクターの移動方向決定
        Quaternion horizontalRotation = Quaternion.AngleAxis(mainCameraTransform.eulerAngles.y, Vector3.up);
        return horizontalRotation * new Vector3(horizontal, 0, vertical).normalized;
    }

    void SetIsGround()
    {
        isGround = CheckIsGround();
    }

    bool CheckIsGround()
    {
        Vector3 rightStartPoint = transformCache.position + Vector3.right * 0.2f + Vector3.up * 0.1f;
        Vector3 leftStartPoint = transformCache.position - Vector3.right * 0.2f + Vector3.up * 0.1f;
        Vector3 endPoint = transformCache.position - Vector3.up * 0.02f;

        return Physics.Linecast(rightStartPoint, endPoint, groundLayer)
            || Physics.Linecast(leftStartPoint, endPoint, groundLayer);
    }

    void SetIsFall()
    {
        isFall = CheckIsFall();
    }

    public bool CheckIsFall()
    {
        return ! Physics.Linecast(transformCache.position + Vector3.up * 0.1f, transformCache.position - Vector3.up * 0.5f, groundLayer);
    }

    public void ReleaseMove(Item skill)
    {
        if (skill.Name == MoveSkillName)
        {
            enabled = true;
        }
    }

    public async void Accelerate()
    {
        if (isAcceleration)
        {
            return;
        }

        isAcceleration = true;
        animator.SetBool("IsAcceleration", true);

        Vector3 moveVector = GetMoveVector();
        float moveMagnitude = moveVector.magnitude;

        if (moveMagnitude == 0)
        {
            moveVector = avatarTransform.forward;
        }

        lastVelocity = Vector3.zero;
        accelerateVector = moveVector.normalized * accelerateSpeed;

        await Task.Delay(500);

        animator.SetBool("IsAcceleration", false);
        isAcceleration = false;
    }

    public void StopMove()
    {
        CanMove = false;
        animator.SetFloat("MoveSpeed", 0f);
    }
}
