using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timer;
    public Text timerText;
    public Text highScoreText;

    private float highScore;

    void Start()
    {
        highScore = PlayerPrefs.GetFloat("HighScore", Mathf.Infinity);
        updateHighScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timerText != null)
        {
            timerText.text = "Time: " + GetTime().ToString("F2");
        }
    }

    public float GetTime()
    {
        return timer;
    }

    public void updateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Best Time: " + (highScore == Mathf.Infinity ? "N/A" : highScore.ToString("F2"));
        }
    }

    [ContextMenu("Clear high score.")]
    public void clearHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        Debug.Log("High score cleared.");
        highScore = Mathf.Infinity;
        updateHighScoreText();
    }
}
