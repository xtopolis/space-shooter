using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectablesSO", menuName = "ScriptableObjects/CollectablesData", order = 1)]
public class CollectablesSO : ScriptableObject
{
    [SerializeField] public GameObject ammo;
    [SerializeField] public GameObject health;
}
