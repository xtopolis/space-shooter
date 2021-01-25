using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float _seconds = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (_seconds == 0)
            Debug.LogWarning("_seconds is 0");

        Destroy(this.gameObject, _seconds);
    }
}
