using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameObject EndGameCanvas;
    public Timer timer;
    public GameObject UICanvas;
    
    private float highScore;

    void Start()
    {
        highScore = PlayerPrefs.GetFloat("HighScore", Mathf.Infinity);

        if (timer == null)
        {
            timer = FindObjectOfType<Timer>();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EndGameCanvas.SetActive(true);
            Time.timeScale = 0f;
            UICanvas.SetActive(false);

            if (timer.GetTime() < highScore)
            {
                highScore = timer.GetTime();
                PlayerPrefs.SetFloat("HighScore", highScore);
                timer.updateHighScoreText();
            }
        }
    }
}
