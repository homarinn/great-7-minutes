using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int calculateDamage(int attack, int defense)
    {
        int damage = attack - (int)Mathf.Floor((float)defense / 4);

        if (damage < 0)
        {
            damage = 0;
        }

        return damage;
    }
}
