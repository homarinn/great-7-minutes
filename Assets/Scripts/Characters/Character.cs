using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEditor.Progress;

public class Character : MonoBehaviour
{
    public int MaxHp { get { return parameters.maxHpUnit * Level; } }
    public int Hp { get; set; }
    public int Power { get { return parameters.powerUnit * Level; } }
    public int Defense { get { return parameters.defenseUnit * Level; } }
    public int Level;
    public int Experience { get; set; } = 0;
    public int LevelUpExperienceUnit { get { return parameters.levelUpExperienceUnit; } }
    int NeedExperience { get { return Level * Level * LevelUpExperienceUnit; } }

    public bool CanAutoHeal { get; set; } = true;
    public bool CanUnlockAutoHeal { get; set; } = true;
    float autoHealAmount = 0;

    public bool IsDead { get; set; } = false;

    public bool IsInvincible { get; set; } = false;

    public CharacterParameters parameters;

    [System.NonSerialized]
    public Animator animator;

    Coroutine unlockAutoHealCoroutine;

    public HpSliderHandler hpSliderHandler;

    public Dictionary<string, Item> skills = new Dictionary<string, Item>();

    public Transform avatarTransform;

    [SerializeField]
    Item magic;

    public AudioClip magicSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Level = parameters.initialLevel;
        hpSliderHandler.SetMaxValue(MaxHp);
        SetParameters();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (IsDead)
        {
            return;
        }

        if (CanAutoHeal)
        {
            AutoHeal();
        }
    }

    public void SetParameters()
    {
        SetHp(MaxHp);
        hpSliderHandler.SetValue(Hp, 0);
    }

    void SetHp(int value)
    {
        Hp = value;
        if (Hp < 0)
        {
            Hp = 0;
        } else if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }

    public void Damage(int damage)
    {
        if (IsDead)
        {
            return;
        }

        if (IsInvincible)
        {
            return;
        }

        if (animator.GetBool("Blocking"))
        {
           damage = Mathf.FloorToInt(damage * 0.1f);
        }

        SetHp(Hp - damage);
        hpSliderHandler.SetValue(Hp);

        if (Hp == 0)
        {
            Die();
        } else
        {
            GetHit(damage);
        }
    }

    void GetHit(int damage)
    {
        if (animator.GetBool("Blocking"))
        {
            return;
        }

        if (damage < 30)
        {
            animator.SetTrigger("HurtLight");
        } else
        {
            animator.SetTrigger("HurtHeavy");
        }
    }

    void Die()
    {
        IsDead = true;
        animator.SetTrigger("Die");
    }

    public void Revive()
    {
        IsDead = false;
        animator.SetTrigger("Revive");
    }

    public void UpExperiense(int Exp)
    {
        Experience += Exp;
        if (Experience >= NeedExperience)
        {
            UpLevel();
        }
    }

    void UpLevel()
    {
        while (Experience >= NeedExperience)
        {
            Experience -= NeedExperience;
            Level += 1;
        }
        hpSliderHandler.SetMaxValue(MaxHp);
        SetParameters();

        GameObject effect = Resources.Load("LevelUpEffect") as GameObject;
        Instantiate(effect, transform);
        PlayerManager playerManager = GetComponent<PlayerManager>();
        playerManager.PlayLevelUpSound();
    }

    public void Heal(int hp)
    {
        SetHp(Hp + hp);
        hpSliderHandler.SetValue(Hp);
    }

    void AutoHeal()
    {
        if (Hp == MaxHp)
        {
            return;
        }

        autoHealAmount += MaxHp / parameters.fullHealTime * Time.deltaTime;
        int healHp = Mathf.FloorToInt(autoHealAmount);

        if (Hp == MaxHp)
        {
            autoHealAmount = 0;
        }
        else
        {
            autoHealAmount -= healHp;
        }

        SetHp(Hp + healHp);
        hpSliderHandler.SetValue(Hp + autoHealAmount, 0);
    }

    public void LockAutoHeal()
    {
        CanAutoHeal = false;
        if (unlockAutoHealCoroutine != null)
        {
            StopCoroutine(unlockAutoHealCoroutine);
            unlockAutoHealCoroutine = null;
        }
    }

    public void UnlockAutoHeal()
    {
        if (CanUnlockAutoHeal)
        {
            CanAutoHeal = true;
        }
    }

    public void StartUnlockAutoHealCoroutine(float time)
    {
        unlockAutoHealCoroutine = StartCoroutine(UnlockAutoHealCoroutine(time));
    }

    IEnumerator UnlockAutoHealCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        UnlockAutoHeal();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyManager>(out EnemyManager enemy))
        {
            if (enemy.AI.IsAttacking)
            {
                int damage = BattleManager.instance.calculateDamage(enemy.Power, Defense);
                Damage(damage);
            }
        }
    }

    public void AcquireSkill(Item item)
    {
        if (!skills.ContainsKey(item.name))
        {
            skills[item.name] = item;
        }
    }

    public void UseSkill(string skillName)
    {
        Item skill = skills[skillName];
        animator.SetTrigger(skill.name);
    }

    public bool CanUseSkill(string skillName)
    {
        return skills.ContainsKey(skillName);
    }

    public void CreateMagic()
    {
        GameObject magicObj = Resources.Load("Magic") as GameObject;
        Vector3 magicObjPosition = avatarTransform.rotation * magicObj.transform.position;
        Vector3 characterPosition = transform.position;
        Vector3 position = new Vector3(magicObjPosition.x + characterPosition.x, magicObjPosition.y + characterPosition.y, magicObjPosition.z + characterPosition.z);
        Instantiate(magicObj, position, avatarTransform.rotation * magicObj.transform.rotation);
        PlayMagicSound();
    }

    void PlayMagicSound()
    {
        audioSource.PlayOneShot(magicSound);
    }
}
