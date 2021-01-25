using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerUpData", order = 1)]
public class PowerUpsSO : ScriptableObject
{
    [SerializeField] public GameObject tripleShot;
    [SerializeField] public GameObject speedBoost;
    [SerializeField] public GameObject shield;
}
