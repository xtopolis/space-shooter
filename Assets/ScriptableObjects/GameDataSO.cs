using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName ="ScriptableObjects/BoundaryData", order = 1)]
public class GameDataSO : ScriptableObject
{
    static public float xRange = 8.5f;
    static public float yRange = 4.5f;

    static public Vector3 RandomSpawnPositionNearBounds()
    {
        float randomX = Random.Range(-xRange, xRange);
        float offsetY = yRange + 3f;

        Vector3 position = new Vector3(randomX, offsetY, 0);
        return position;
    }
}
