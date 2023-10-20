using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    const int HpSliderDisplayDistance = 30;
    public int Hp { get; set; }
    public int Power { get { return parameters.power; } }
    public int Defense { get { return parameters.defense; } }
    public int Experiense { get { return parameters.experience; } }

    public bool IsDead { get; set; } = false;

    public EnemyAI AI { get; set; }
    public EnemyParameters parameters;
    Animator animator;
    public BoxCollider triggerBoxCollider;

    public HpSliderHandler hpSliderHandler;

    AudioSource audioSource;
    public AudioClip dieSound;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        AI = GetComponent<EnemyAI>();
        AI.animator = animator;

        Hp = parameters.maxHp;
        hpSliderHandler.SetMaxValue(parameters.maxHp);
        hpSliderHandler.SetValue(Hp, 0);

        audioSource = GetComponent<AudioSource>();
    }

    public void Damage(int damage)
    {
        if (IsDead)
        {
            return;
        }

        SetHp(Hp - damage);
        hpSliderHandler.SetValue(Hp);

        if (Hp == 0)
        {
            Die();
        }
        else
        {
            GetHit();
        }
    }

    void SetHp(int value)
    {
        Hp = value;
        if (Hp < 0)
        {
            Hp = 0;
        }
        else if (Hp > parameters.maxHp)
        {
            Hp = parameters.maxHp;
        }
    }

    void GetHit()
    {
        animator.SetTrigger("Hurt");
    }

    void Die()
    {
        IsDead = true;
        animator.SetTrigger("Die");
        PlayDieSound();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Weapon>(out Weapon weapon))
        {
            if (weapon.CanHit && !IsDead)
            {
                weapon.CanHit = false;

                int damage = BattleManager.instance.calculateDamage(weapon.Attack, Defense);

                Damage(damage);
                if (IsDead)
                {
                    weapon.equipper.UpExperiense(Experiense);
                }
            }
        }
    }

    public void PlayDieSound()
    {
        audioSource.PlayOneShot(dieSound);
    }
}
