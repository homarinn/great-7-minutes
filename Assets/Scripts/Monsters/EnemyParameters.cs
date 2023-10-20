using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyParameters", menuName = "MyGame/Create ParameterTable/Enemy")]
public class EnemyParameters : ScriptableObject
{
    public int maxHp;
    public int power;
    public int defense;
    public int experience;
}
