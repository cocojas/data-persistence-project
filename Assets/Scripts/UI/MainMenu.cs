using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    private UIDocument document;
    private Label bestScoreLabel;
    private TextField playerNameField;
    private VisualElement highScoresPanel;

    private static Dictionary<string, Func<MainMenu, Action>> buttonActions = new Dictionary<string, Func<MainMenu, Action>>
    {
        { "StartButton", (obj) => obj.OnStartButtonClicked },
        { "QuitButton", (m) => m.OnQuitClicked },
        { "HighScoresButton", (m) => m.OnHighScoresButtonClicked },
        { "ClearHighScoresButton", (m) => m.OnClearHighScoresButtonClicked },
        { "BackToMenuButton", (m) => m.OnHighScoresDismissedClicked }
        // { "TestButton", (m) => m.OnTestButtonClicked }
    };

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        bestScoreLabel = root.Q<Label>("BestScoreLabel");
        playerNameField = root.Q<TextField>("PlayerNameField");
        highScoresPanel = root.Q<VisualElement>("HighScoresPanel");
    }

    private void OnEnable()
    {
        var root = document.rootVisualElement;
        foreach (string buttonName in buttonActions.Keys)
        {
            var button = root.Q<Button>(buttonName);
            button.clicked += buttonActions[buttonName](this);
        }

        if (ScoreManager.Instance != null)
        {
            UpdateScoresAndPlayerNames();
        }
        playerNameField.RegisterValueChangedCallback((evt) => ScoreManager.Instance.SetPlayerName(evt.newValue));
    }

    private void OnDisable()
    {
        var root = document.rootVisualElement;
        if (root == null) return;
        foreach (string buttonName in buttonActions.Keys)
        {
            var button = root.Q<Button>(buttonName);
            if (button == null) continue;
            button.clicked -= buttonActions[buttonName](this);
        }

        playerNameField.UnregisterValueChangedCallback((evt) => ScoreManager.Instance.SetPlayerName(evt.newValue));
    }

    private void Start()
    {
        UpdateScoresAndPlayerNames();
    }

    private void OnTestButtonClicked()
    {
        var root = document.rootVisualElement;
        foreach (string buttonName in buttonActions.Keys)
        {
            var button = root.Q<Button>(buttonName);
            if (button == null) continue;
            button.clicked -= buttonActions[buttonName](this);
        }
    }

    public void OnStartButtonClicked()
    {
        if (ScoreManager.Instance.CurrentScore.PlayerName == "")
        {
            ScoreManager.Instance.SetPlayerName(playerNameField.value);
        }
        SceneManager.LoadScene(0);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void ShowHighScores()
    {
        highScoresPanel.visible = true;
    }

    public void HideHighScores()
    {
        highScoresPanel.visible = false;
    }

    private void OnHighScoresDismissedClicked()
    {
        HideHighScores();
    }

    private void OnHighScoresButtonClicked()
    {
        ShowHighScores();
        UpdateScoresAndPlayerNames();
    }

    private void OnClearHighScoresButtonClicked()
    {
        Debug.Log("==> Clearning High Scores");
        ScoreManager.Instance.ClearHighScores();
        UpdateScoresAndPlayerNames();
    }

    private void UpdateScoresAndPlayerNames()
    {
        var highScores = ScoreManager.Instance.GetHighScores();
        if (highScores == null || highScores.Count == 0)
        {
            bestScoreLabel.text = "Best Score : None";
            return;
        }

        var bestScore = highScores[0];
        if (bestScore.PlayerName != null && bestScore.PlayerScore != 0)
        {
            if (bestScore.PlayerName != "")
            {
                bestScoreLabel.text = $"Best Score : {bestScore.PlayerName} : {bestScore.PlayerScore}";
            }
            else
            {
                bestScoreLabel.text = $"Best Score : {bestScore.PlayerScore}";
            }
        }
        else
        {
            bestScoreLabel.text = "Best Score : None";
        }

        int maxScores = Math.Min(10, highScores.Count);
        for (int i = 0; i < maxScores; i++)
        {
            var scoreLabel = highScoresPanel.Q<Label>($"hs-slot-{i}");
            if (i < highScores.Count && highScores[i].PlayerScore != 0)
            {
                var score = highScores[i];
                scoreLabel.text = $"#{i + 1} : {score.PlayerName} : {score.PlayerScore}";
                scoreLabel.style.display = DisplayStyle.Flex;
            }
            else
            {
                scoreLabel.text = "";
                scoreLabel.style.display = DisplayStyle.None;
            }
        }

        playerNameField.value = ScoreManager.Instance.CurrentScore.PlayerName;
    }

}
