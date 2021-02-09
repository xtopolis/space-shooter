using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shields : MonoBehaviour
{
    [SerializeField] private GameObject _particleEffect = null;

    public event Action OnShieldDestroyed;

    private SpriteRenderer _shieldSprite = null;
    private int _hitsRemaining = 3;

    private void Start()
    {
        _shieldSprite = transform.GetComponent<SpriteRenderer>();

        if (_shieldSprite == null)
            Debug.LogError("_shieldSprite is null");
    }

    private void OnDisable()
    {
        if (_particleEffect)
        {
            GameObject particle = Instantiate(_particleEffect, transform.position, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }

    public void TakeDamage()
    {
        _hitsRemaining -= 1;

        if(_hitsRemaining <= 0)
        {
            OnShieldDestroyed?.Invoke();
        }

        UpdateShieldVisuals();
    }

    void UpdateShieldVisuals()
    {
        Color spriteColor = _shieldSprite.color;

        switch (_hitsRemaining)
        {
            case 2:
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                spriteColor.a = .55f;
                break;
            case 1:
                transform.localScale = new Vector3(1f, 1f, 1f);
                spriteColor.a = .25f;
                break;
            default:
                transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
                spriteColor.a = 1f;
                break;
        }
        _shieldSprite.color = spriteColor;
    }
    
    public void SetShieldHits(int numberOfHits)
    {
        _hitsRemaining = numberOfHits;
    }
}
