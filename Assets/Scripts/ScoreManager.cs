using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public Score CurrentScore = new Score();

    private HighScores highScores;

    private const int MAX_HIGH_SCORES = 10;

    private const string saveFilename = "savefile.json";
    private string savePath;

    [SerializeField] private string defaultPlayerName = "Player";

    public static ScoreManager Instance { get; private set; }

    [Serializable]
    public struct Score
    {
        public string PlayerName;
        public int PlayerScore;
    }

    [Serializable]
    private struct HighScores
    {
        public Score[] Scores;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        savePath = Application.persistentDataPath + "/" + saveFilename;
        Instance = this;
        LoadScores();
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerName(string name)
    {
        if (name == "")
        {
            name = defaultPlayerName;
        }
        CurrentScore.PlayerName = name;
    }

    public void AddScore(int score)
    {
        CurrentScore.PlayerScore += score;
    }

    /// <summary>
    /// Reset the current score to 0 and save the current score to the high scores
    /// </summary>
    public void ResetScore()
    {
        SaveCurrentScore();
        ClearScore();
    }

    /// <summary>
    /// Clear the current score without saving it
    /// </summary>
    public void ClearScore()
    {
        CurrentScore.PlayerScore = 0;
    }

    public Score GetBestScore()
    {
        return GetHighScores().FirstOrDefault();
    }

    public List<Score> GetHighScores()
    {
        return new List<Score>(highScores.Scores);
    }

    public bool SaveCurrentScore()
    {
        return SaveScore(CurrentScore);
    }

    public bool SaveScore(Score score)
    {
        bool scoreAdded = false;
        for (int i = 0; i < highScores.Scores.Length; i++)
        {
            if (score.PlayerScore > highScores.Scores[i].PlayerScore)
            {
                for (int j = highScores.Scores.Length - 1; j > i; j--)
                {
                    highScores.Scores[j] = highScores.Scores[j - 1];
                }
                highScores.Scores[i] = score;
                scoreAdded = true;
                break;
            }
        }

        if (scoreAdded)
        {
            SaveScores();
        }
        return scoreAdded;
    }

    public void ClearHighScores()
    {
        highScores.Scores = new Score[MAX_HIGH_SCORES];
        SaveScores();
    }

    public void SaveScores()
    {
        // Only save if there is at least one score
        if (highScores.Scores.Length == 0 || highScores.Scores[0].PlayerName == null || highScores.Scores[0].PlayerName == "" || highScores.Scores[0].PlayerScore == 0)
        {
            return;
        }
        string json = JsonUtility.ToJson(highScores);
        File.WriteAllText(savePath, json);
    }

    public void LoadScores()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            highScores = JsonUtility.FromJson<HighScores>(json);
            if (highScores.Scores == null)
            {
                highScores.Scores = new Score[MAX_HIGH_SCORES];
            }
            else if (highScores.Scores.Length != MAX_HIGH_SCORES)
            {
                int count = Math.Min(highScores.Scores.Length, MAX_HIGH_SCORES);
                Score[] newScores = new Score[MAX_HIGH_SCORES];
                for (int i = 0; i < count; i++)
                {
                    newScores[i] = highScores.Scores[i];
                }
                highScores.Scores = newScores;
            }
        }
        else
        {
            highScores.Scores = new Score[MAX_HIGH_SCORES];
        }
    }

    private void OnApplicationQuit()
    {
        SaveScores();
    }
}
