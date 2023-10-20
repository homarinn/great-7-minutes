using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [field: SerializeField]
    public int Power { get; set; }
    public Character equipper;
    public bool CanHit { get; set; } = false;

    public int Attack { get { return equipper.Power + Power; } }
}
