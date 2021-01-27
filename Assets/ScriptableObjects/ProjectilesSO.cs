using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectiles", menuName = "ScriptableObjects/ProjectilesData", order = 1)]
public class ProjectilesSO : ScriptableObject
{
    [SerializeField] public GameObject laser;
    [SerializeField] public GameObject tripleShot;
    [SerializeField] public GameObject blackHoleCreator;
}
