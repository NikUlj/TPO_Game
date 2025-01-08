using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ScoreManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ScoreManager");
                    _instance = go.AddComponent<ScoreManager>();
                }
            }
            return _instance;
        }
    }

    private int _currentScore = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        Debug.Log($"Score increased! Current score: {_currentScore}");
    }

    public void AddKillScore()
    {
        AddScore(1);
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }

    public void ResetScore()
    {
        _currentScore = 0;
        Debug.Log("Score reset to 0");
    }
}