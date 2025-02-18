using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        // Create a score manager if we're in the editor and didn't start from the main menu
        if (ScoreManager.Instance == null)
        {
            new GameObject("ScoreManager", typeof(ScoreManager));
            ScoreManager sm = ScoreManager.Instance;
            sm.SetPlayerName("Player");
            sm.ClearScore();
        }
#endif

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        UpdateScoreLabel();
        UpdateBestScoreLabel();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ScoreManager.Instance.ResetScore();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        ScoreManager sm = ScoreManager.Instance;
        sm.AddScore(point);
        UpdateScoreLabel();
    }

    public void GameOver()
    {
        ScoreManager.Instance.ResetScore();
        m_GameOver = true;
        GameOverText.SetActive(true);
        UpdateBestScoreLabel();
    }

    private void UpdateScoreLabel()
    {
        ScoreManager.Score current = ScoreManager.Instance.CurrentScore;
        ScoreText.text = $"Score : {current.PlayerScore}";
    }

    private void UpdateBestScoreLabel()
    {
        ScoreManager.Score best = ScoreManager.Instance.GetBestScore();
        if (best.PlayerScore == 0)
        {
            BestScoreText.text = "Best Score : None";
            return;
        }
        BestScoreText.text = $"Best Score : {best.PlayerName} : {best.PlayerScore}";
    }
}
