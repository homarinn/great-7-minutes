using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Cinemachine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class BossSpawn : MonoBehaviour
{
    bool isSpawned = false;
    public Dialogue.Dialogue lookAtMemoryDialogue;
    public Dialogue.Dialogue lookAtBossDialogue;
    public Dialogue.Dialogue startBossBattleDialogue;

    public CinemachineVirtualCamera memoryCamera;
    public CinemachineVirtualCamera bossCamera;
    private MoveHandler moveHandler;

    private GameObject boss;

    private void OnTriggerEnter(Collider other)
    {
        if (!isSpawned && other.CompareTag("Player"))
        {
            moveHandler = other.GetComponent<MoveHandler>();
            moveHandler.StopMove();
            // 絵を見るようにカメラを動かす処理
            LookAtMemory();
            // テキストを表示する処理
            ShowMemoryDialogue();

            isSpawned = true;
        }
    }

    private void LookAtMemory()
    {
        memoryCamera.Priority = 20;
    }

    public void LookAtBoss()
    {
        Camera camera = Camera.main;
        camera.LayerCullingShow("Player");
        CinemachineBrain brain = camera.GetComponent<CinemachineBrain>();
        brain.m_DefaultBlend.m_Time = 0;
        bossCamera.Priority = 30;
    }

    private async void ShowMemoryDialogue()
    {
        await Task.Delay(1000);
        Camera.main.LayerCullingHide("Player");
        DialoguePlayer dialoguePlayer = DialogueManager.instance.CreateDialoguePlayer(lookAtMemoryDialogue);
        // ボスを出現させる処理
        dialoguePlayer.onFinishEvent.AddListener(SpawnBoss);
        // ボスを見るようにカメラを動かす処理
        dialoguePlayer.onFinishEvent.AddListener(LookAtBoss);
        dialoguePlayer.onFinishEvent.AddListener(ShowBossBattleStartDialogue);
        dialoguePlayer.Play();
    }

    public void ShowBossBattleStartDialogue()
    {
        DialoguePlayer dialoguePlayer = DialogueManager.instance.CreateDialoguePlayer(lookAtBossDialogue);
        dialoguePlayer.onFinishEvent.AddListener(ResetCamera);
        dialoguePlayer.onFinishEvent.AddListener(ReleasePlayerMove);
        dialoguePlayer.onFinishEvent.AddListener(ReleasePBossAI);
        dialoguePlayer.Play();
    }

    public void SpawnBoss()
    {
        boss = Instantiate(Resources.Load("Orc") as GameObject);
    }

    public void ResetCamera()
    {
        Camera camera = Camera.main;
        CinemachineBrain brain = camera.GetComponent<CinemachineBrain>();
        brain.m_DefaultBlend.m_Time = 1;
        memoryCamera.Priority = 1;
        bossCamera.Priority = 1;
    }

    public void ReleasePlayerMove()
    {
        moveHandler.CanMove = true;
        DialoguePlayer dialoguePlayer = DialogueManager.instance.CreateDialoguePlayer(startBossBattleDialogue);
        dialoguePlayer.Play();
    }

    public void ReleasePBossAI()
    {
        boss.GetComponent<EnemyAI>().enabled = true;
    }
}
