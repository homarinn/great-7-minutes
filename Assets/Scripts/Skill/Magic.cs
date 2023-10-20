using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    const int PowerPerFrame = 1;
    const float StretchSpeed = 0.6f;
    const float maxScaleZ = 20f;

    [SerializeField]
    Transform parentTransform;

    readonly Dictionary<int, EnemyManager> enemies = new Dictionary<int, EnemyManager>();

    bool IsHit { get; set; } = false;
    bool IsReached { get; set; } = false;
    float accumulatedScale;

    private void Start()
    {
        accumulatedScale = parentTransform.localScale.z;
    }

    private void Update()
    {
        if (IsHit)
        {
            if (enemies.Count != 0)
            {
                foreach (EnemyManager enemy in enemies.Values)
                {
                    enemy.Damage(PowerPerFrame);
                }
            }
        }

        float scaleZ = parentTransform.localScale.z;
        if (IsReached)
        {
            if (accumulatedScale >= maxScaleZ)
            {
                scaleZ -= StretchSpeed;
                if (scaleZ < 0f)
                {
                    scaleZ = 0f;
                }
                parentTransform.Translate(0, 0, StretchSpeed, Space.Self);
            }
        } else
        {
            scaleZ += StretchSpeed;
            if (scaleZ > maxScaleZ)
            {
                scaleZ = maxScaleZ;
                IsReached = true;
            }
        }

        if (scaleZ == 0f)
        {
            Destroy(parentTransform.gameObject);
            return;
        }

        parentTransform.localScale = new Vector3(parentTransform.localScale.x, parentTransform.localScale.y, scaleZ);

        accumulatedScale += StretchSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IsHit = true;
        IsReached = true;

        if (collision.gameObject.TryGetComponent(out EnemyManager hitEnemy))
        {
            enemies[hitEnemy.GetInstanceID()] = hitEnemy;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyManager hitEnemy))
        {
            if (enemies.ContainsKey(hitEnemy.GetInstanceID()))
            {
                enemies.Remove(hitEnemy.GetInstanceID());
            }
        }
    }
}
