using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBehaviour : StateMachineBehaviour
{
    GameObject specialSword;
    Transform rightHandTransform;
    PlayerManager player;
    Transform transformCache;
    Transform avatarTransform;
    Quaternion playerRotation;
    GameObject specialSwordObj;
    GameObject defaultWeaponObj;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (specialSword == null)
    //    {
    //        specialSword = Resources.Load("Arms/SpecialSword") as GameObject;
    //    }

    //    if (rightHandTransform == null)
    //    {
    //        rightHandTransform = GameObject.FindWithTag("RightHand").transform;
    //    }

    //    if (player == null)
    //    {
    //        player = animator.GetComponent<PlayerManager>();
    //        avatarTransform = animator.GetComponent<MoveHandler>().avatarTransform;
    //    }

    //    Quaternion rotation = transformCache.localRotation;
    //    playerRotation = rotation;

    //    Weapon defaultWeapon = player.weapon;
    //    if (defaultWeapon != null)
    //    {
    //        defaultWeaponObj = defaultWeapon.gameObject;
    //        defaultWeaponObj.SetActive(false);
    //    }

    //    specialSwordObj = Instantiate(specialSword, rightHandTransform);
    //    player.weapon = specialSwordObj.GetComponent<Weapon>();
    //    player.weapon.CanHit = true;
    //}

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Destroy(specialSwordObj);
    //    if (defaultWeaponObj != null)
    //    {
    //        defaultWeaponObj.SetActive(true);
    //        defaultWeaponObj.GetComponent<Weapon>().CanHit = false;
    //    }
    //    avatarTransform.Rotate(0, 20, 0, Space.World);
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (transformCache == null)
        {
            transformCache = animator.transform;
        }

        if (specialSword == null)
        {
            specialSword = Resources.Load("Arms/SpecialSword") as GameObject;
        }

        if (rightHandTransform == null)
        {
            rightHandTransform = GameObject.FindWithTag("RightHand").transform;
        }

        if (player == null)
        {
            player = animator.GetComponent<PlayerManager>();
            avatarTransform = animator.GetComponent<MoveHandler>().avatarTransform;
        }

        Quaternion rotation = transformCache.localRotation;
        playerRotation = rotation;

        Weapon defaultWeapon = player.weapon;
        if (defaultWeapon != null)
        {
            defaultWeaponObj = defaultWeapon.gameObject;
            defaultWeaponObj.SetActive(false);
        }

        specialSwordObj = Instantiate(specialSword, rightHandTransform);
        player.weapon = specialSwordObj.GetComponent<Weapon>();
        player.weapon.CanHit = true;
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        Destroy(specialSwordObj);
        if (defaultWeaponObj != null)
        {
            defaultWeaponObj.SetActive(true);
            defaultWeaponObj.GetComponent<Weapon>().CanHit = false;
        }
        transformCache.localRotation = Quaternion.Lerp(transformCache.localRotation, playerRotation, 1);
    }
}
