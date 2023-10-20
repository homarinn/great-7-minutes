using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Cinemachine;

public class LookMemory : MonoBehaviour
{
    public Dialogue.Dialogue closeMemoryDialogue;
    public CinemachineVirtualCamera memoryCamera;

    bool isPlayerClose = false;
    bool isInsideCamera = false;
    bool isDialoguePlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        isPlayerClose = other.CompareTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isDialoguePlayed && isPlayerClose && isInsideCamera)
        {
            isDialoguePlayed = true;
            ShowMemoryDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerClose = ! other.CompareTag("Player");
    }

    private void ProhibitPlayerAction()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerManager player = playerObject.GetComponent<PlayerManager>();
        Character character = playerObject.GetComponent<Character>();
        MoveHandler moveHandler = playerObject.GetComponent<MoveHandler>();

        player.CanNotUseSkill = true;
        character.IsInvincible = true;
        moveHandler.StopMove();
    }

    private void AllowPlayerMove()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerManager player = playerObject.GetComponent<PlayerManager>();
        Character character = playerObject.GetComponent<Character>();
        MoveHandler moveHandler = playerObject.GetComponent<MoveHandler>();

        player.CanNotUseSkill = false;
        moveHandler.CanMove = true;
        character.IsInvincible = false;
    }

    private void ShowMemoryDialogue()
    {
        Camera.main.LayerCullingHide("Player");
        memoryCamera.Priority = 20;
        DialoguePlayer dialoguePlayer = DialogueManager.instance.CreateDialoguePlayer(closeMemoryDialogue);
        // ボスを出現させる処理
        dialoguePlayer.onFinishEvent.AddListener(ResetCamera);
        dialoguePlayer.onFinishEvent.AddListener(AllowPlayerMove);
        dialoguePlayer.Play();
    }

    private void ResetCamera()
    {
        Camera.main.LayerCullingShow("Player");
        memoryCamera.Priority = 1;
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
