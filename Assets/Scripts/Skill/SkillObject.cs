using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cinemachine;
using Dialogue;

public class SkillObject : BaseItemObject
{
    [SerializeField]
    OnGetSkill onGetSkill;

    AudioSource audioSource;
    public AudioClip getSkillSound;
    public CinemachineVirtualCamera memoryCamera;
    public Dialogue.Dialogue memoryDialogue;
    public bool ShouldPlayerHide;

    protected override void OnGet(Item skillItem)
    {
        if (skillItem.type == Item.Type.SKILLITEM)
        {
            onGetSkill.RaiseEvent(skillItem);
        }
    }

    public async void Get(Item skill)
    {
        if (skill.name == item.name)
        {
            GameObject effect = Resources.Load("SkillEffect") as GameObject;
            Instantiate(effect, transform);

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.PlayOneShot(getSkillSound);

            if (memoryDialogue)
            {
                ProhibitPlayerAction();
            }

            await Task.Delay(500);


            if (memoryDialogue)
            {
                ShowMemoryDialogue();
            }

            PlayerManager player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            switch (skill.name)
            {
                case "Attack":
                    player.GetSword();
                    return;
                case "Block":
                    player.GetShield();
                    return;
            }
        }
    }

    private void ProhibitPlayerAction()
    {
        if (ShouldPlayerHide)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            PlayerManager player = playerObject.GetComponent<PlayerManager>();
            Character character = playerObject.GetComponent<Character>();
            MoveHandler moveHandler = playerObject.GetComponent<MoveHandler>();

            player.CanNotUseSkill = true;
            character.IsInvincible = true;
            moveHandler.StopMove();
        }
    }

    private void AllowPlayerMove()
    {
        if (ShouldPlayerHide)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            PlayerManager player = playerObject.GetComponent<PlayerManager>();
            Character character = playerObject.GetComponent<Character>();
            MoveHandler moveHandler = playerObject.GetComponent<MoveHandler>();

            player.CanNotUseSkill = false;
            moveHandler.CanMove = true;
            character.IsInvincible = false;
        }
    }

    private void ShowMemoryDialogue()
    {
        if (ShouldPlayerHide)
        {
            Camera.main.LayerCullingHide("Player");
            memoryCamera.Priority = 20;
        } else
        {
            memoryCamera.Priority = 1;
        }
        DialoguePlayer dialoguePlayer = DialogueManager.instance.CreateDialoguePlayer(memoryDialogue);
        // ボスを出現させる処理
        dialoguePlayer.onFinishEvent.AddListener(ResetCamera);
        dialoguePlayer.onFinishEvent.AddListener(AllowPlayerMove);
        dialoguePlayer.Play();
    }

    private void ResetCamera()
    {
        if (ShouldPlayerHide)
        {
            Camera.main.LayerCullingShow("Player");
            memoryCamera.Priority = 1;
        }
    }
}
