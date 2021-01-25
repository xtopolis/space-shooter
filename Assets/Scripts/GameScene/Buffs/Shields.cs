using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shields : MonoBehaviour
{
    [SerializeField] private GameObject _particleEffect = null;
    private void OnDisable()
    {
        if (_particleEffect)
        {
            GameObject particle = Instantiate(_particleEffect, transform.position, Quaternion.identity);
            particle.transform.parent = transform.parent;
        }
    }
}
