using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [SerializeField]
    Character character;

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

    public void GetSkill(Item item)
    {
        if (!character.skills.ContainsKey(item.name))
        {
            character.skills[item.name] = item;
        }
    }
}
