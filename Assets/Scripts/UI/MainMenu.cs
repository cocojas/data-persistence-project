using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    private UIDocument document;
    private Label bestScoreLabel;

    private static Dictionary<string, Func<MainMenu, Action>> buttonActions = new Dictionary<string, Func<MainMenu, Action>>
    {
        { "StartButton", (obj) => obj.OnStartButtonClicked },
        { "QuitButton", (m) => m.OnQuitClicked },
        { "HighScoresButton", (m) => m.OnHighScoresButtonClicked },
        { "TestButton", (m) => m.OnTestButtonClicked }
    };

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        bestScoreLabel = root.Q<Label>("BestScoreLabel");
    }

    private void OnEnable()
    {
        var root = document.rootVisualElement;
        foreach (string buttonName in buttonActions.Keys)
        {
            var button = root.Q<Button>(buttonName);
            button.clicked += buttonActions[buttonName](this);
        }

        if (bestScoreLabel == null) return;

        var bestScore = ScoreManager.Instance.GetBestScore();
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
        Debug.Log("Start button clicked");
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void OnHighScoresButtonClicked()
    {
        Debug.Log("High Scores button clicked");
    }

}
