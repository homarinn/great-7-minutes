using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Dialogue;
using Cinemachine;
using System.Threading.Tasks;


public class PlayerManager : MonoBehaviour
{
    Rigidbody rigidBody;
    Animator animator;
    AudioSource audioSource;
    MoveHandler moveHandler;
    Character character;
    public Weapon weapon;
    public Dialogue.Dialogue startDialogue;
    public AudioClip attackSound;
    public AudioClip specialChargeSound;
    public AudioClip specialAttackSound;
    public AudioClip footstepsSound;
    public AudioClip levelUpSound;

    public CinemachineVirtualCamera startCamera;
    public CinemachineVirtualCamera kissCamera;

    public bool CanNotUseSkill { get; set; } = false;
    public bool CanGetSkill { get; set; } = false;

    bool isNotFirstLand = true;

    private void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        character = GetComponent<Character>();
        character.animator = animator;

        moveHandler = GetComponent<MoveHandler>();
        moveHandler.rigidBody = rigidBody;
        moveHandler.animator = animator;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanNotUseSkill)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (character.CanUseSkill("Attack"))
            {
                character.UseSkill("Attack");
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (character.CanUseSkill("Accelerate"))
            {
                character.UseSkill("Accelerate");
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (character.CanUseSkill("Magic"))
            {
                character.UseSkill("Magic");
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (character.CanUseSkill("Special"))
            {
                character.UseSkill("Special");
                return;
            }
        }

        if (character.CanUseSkill("Block"))
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                character.UseSkill("Block");
                return;
            } else if (Input.GetKeyUp(KeyCode.B))
            {
                animator.SetBool("Blocking", false);
                return;
            }
        }
    }

    void FixedUpdate()
    {
        if (moveHandler.enabled)
        {
            moveHandler.Move();
        } else
        {
            animator.SetBool("IsFalling", moveHandler.CheckIsFall());
            if (moveHandler.CheckIsFall())
            {
                rigidBody.velocity = new Vector3(0, Physics.gravity.y * 0.5f, 0);
                animator.SetBool("IsFalling", true);
            }
            else
            {
                if (isNotFirstLand)
                {
                    isNotFirstLand = false;
                    FirstLanding();
                }
            }
        }
    }

    private async void FirstLanding()
    {
        animator.SetBool("IsFalling", false);
        startCamera.Priority = 5;
        kissCamera.Priority = 20;

        await Task.Delay(2400);

        DialoguePlayer dialoguePlayer = DialogueManager.instance.CreateDialoguePlayer(startDialogue);
        dialoguePlayer.onFinishEvent.AddListener(ReleaseGetSkill);
        dialoguePlayer.Play();
    }

    private void ReleaseGetSkill()
    {
        CanGetSkill = true;
    }

    public void GetSword()
    {
        GameObject sword = Resources.Load("Arms/Sword") as GameObject;
        sword = Instantiate(sword, GameObject.FindWithTag("RightHand").transform);
        weapon = sword.GetComponent<Weapon>();
        weapon.equipper = character;
        animator.SetBool("HasSword", true);
    }

    public void GetShield()
    {
        GameObject shield = Resources.Load("Arms/Shield") as GameObject;
        shield = Instantiate(shield, GameObject.FindWithTag("LeftHand").transform);
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }

    public void PlaySpecialChargeSound()
    {
        audioSource.PlayOneShot(specialChargeSound);
    }

    public void PlaySpecialAttackSound()
    {
        audioSource.PlayOneShot(specialAttackSound);
    }

    public void PlayFootstepsSound()
    {
        audioSource.PlayOneShot(footstepsSound);
    }

    public void PlayLevelUpSound()
    {
        audioSource.PlayOneShot(levelUpSound);
    }
}
