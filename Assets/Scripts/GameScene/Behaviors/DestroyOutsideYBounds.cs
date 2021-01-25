using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutsideYBounds : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private bool _upperYBound = true;
    [SerializeField] private bool _lowerYBound = true;

    private float _buffer = 5f;

    void Update()
    {
        float yRangeWithBuffer = GameDataSO.yRange + _buffer;

        // Both bounds
        if(_upperYBound == true && _lowerYBound == true)
        {
            if (transform.position.y > yRangeWithBuffer || transform.position.y < -yRangeWithBuffer)
            {
                DestroyParentIfExists();
            }
            return;
        }

        if(_upperYBound == true && _lowerYBound == false)
        {
            if (transform.position.y > yRangeWithBuffer)
            {
                DestroyParentIfExists();
            }
            return;
        }

        if(_lowerYBound == true && _upperYBound == false)
        {
            if (transform.position.y < -yRangeWithBuffer)
            {
                DestroyParentIfExists();
            }
            return;
        }

        Debug.LogWarning("Unhandled bounds logic");
    }

    private void DestroyParentIfExists()
    {
        if (transform.parent != null && transform.parent.CompareTag("ProjectilesContainer") == false)
        {
            Destroy(transform.parent.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }
}
