using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private GameObject _particle = null;
    private AudioSource _laserAudioSource = null;

    [SerializeField] private float _speed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if(_particle == null)
            Debug.LogError("_particle is null");
        if (_speed == 0f)
            Debug.LogWarning("_speed is 0");

        _laserAudioSource = transform.GetComponent<AudioSource>();
        if (_laserAudioSource != null)
        {
            _laserAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("_laserAudioSource is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameObject particle = Instantiate(_particle, transform.position, Quaternion.identity);
        Destroy(particle, 0.5f);
    }
}
