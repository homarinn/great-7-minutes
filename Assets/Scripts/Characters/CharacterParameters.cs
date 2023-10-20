using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterParameters", menuName = "MyGame/Create ParameterTable/Character")]
public class CharacterParameters : ScriptableObject
{
    public int maxHpUnit;
    public int powerUnit;
    public int defenseUnit;
    public int initialLevel;
    public int levelUpExperienceUnit;
    public int fullHealTime;
}
