using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundRules : MonoBehaviour
{
    [Header("Shooting")]
    public float bulletsGravityMultiplier;

    [Header("Movement")]
    [Range(0, 1)] public float slowTimeMultiplier;
    [Range(1, 3)] public float fastTimeMultiplier;

    //[Header("Winning")]
}
