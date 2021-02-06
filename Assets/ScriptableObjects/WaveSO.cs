using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave.asset", menuName = "ScriptableObjects/Wave", order = 2)]
public class WaveSO : ScriptableObject
{
    public enum SpawnFormat
    {
        SEQUENCE,
        ALL_AT_ONCE
    }

    [SerializeField] public List<GameObject> enemies;
    [SerializeField] public SpawnFormat spawnMethod = SpawnFormat.SEQUENCE;
}
