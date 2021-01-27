using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCreator : MonoBehaviour
{
    [SerializeField] private GameObject _blackHolePrefab = null;
    [SerializeField] private GameObject _explosionPrefab = null;
    private AudioSource _audioSource = null;

    private float endPosY;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = transform.GetComponent<AudioSource>();

        if (_explosionPrefab == null)
            Debug.LogError("_explosionPrefab is null");

        if (_blackHolePrefab == null)
            Debug.LogError("_blackHolePrefab is null");

        if (_audioSource == null)
            Debug.LogError("_audioSource is null");

        endPosY = Mathf.Min(transform.position.y + 5, GameDataSO.yRange - 2); // Arbitrary 2 offset
    }

    // Update is called once per frame
    void Update()
    {
        float dist = endPosY - transform.position.y;
        Vector3 endPos = new Vector3(transform.position.x, endPosY, transform.position.z);

        if (dist > 0.15)
        {
            transform.position = Vector3.Lerp(transform.position, endPos, 0.5f * Time.deltaTime); //Translate(Vector3.up * 1f * Time.deltaTime);
            transform.Rotate(new Vector3(0, 0, 1), 1f);
        } else
        {
            _audioSource.Play();
            Instantiate(_blackHolePrefab, transform.position, Quaternion.identity);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
