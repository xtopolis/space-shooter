using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text _scoreText = null;
    [SerializeField] private Image _livesImage = null;
    [SerializeField] private Text _gameOverText = null;
    [SerializeField] private Text _restartText = null;

    [Header("Prefabs")]
    [SerializeField] private Sprite[] _livesSprites = null;

    // Properties
    private int _score = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (_livesSprites == null)
            Debug.LogError("_livesSprites is null");

        if (_livesImage == null)
            Debug.LogError("_livesImage is null");

        if (_scoreText == null)
            Debug.LogError("_scoreText is null");
        
        if (_gameOverText == null)
            Debug.LogError("_gameOverText is null");

        if (_restartText == null)
            Debug.LogError("_restartText is null");
        

        SetScoreText();

        // Does this (Enemy) create mem/perf issues in a real game? -- too many subs, does Destroy auto cleanup?
        Enemy.killedByPlayer += UpdateScore;
        Player.livesChanged += UpdateLives;
        Player.playerDied += Unsubscribe;
    }

    void Unsubscribe()
    {
        Player.livesChanged -= UpdateLives;
        Player.playerDied -= Unsubscribe;
    }

    void UpdateScore(int amount)
    {
        _score += amount;
        SetScoreText();
    }

    void SetScoreText()
    {
        _scoreText.text = $"Score: {_score}";
    }

    void UpdateLives(int remainingLives)
    {
        switch (remainingLives)
        {
            case 2:
                _livesImage.sprite = _livesSprites[2];
                break;
            case 1:
                _livesImage.sprite = _livesSprites[1];
                break;
            case 0:
                _livesImage.sprite = _livesSprites[0];
                ShowGameOverText();
                break;
            default:
                _livesImage.sprite = _livesSprites[3];
                break;
        }
    }

    void ShowGameOverText()
    {
        StartCoroutine(GameOverFlicker());
        _restartText.gameObject.SetActive(true);
    }

    IEnumerator GameOverFlicker()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
