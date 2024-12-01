using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameLogic : MonoBehaviour
{
    public void restart()
    {
        SceneManager.LoadScene("_Scene_0");
        Time.timeScale = 1f;
    }

    public void quit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        Debug.Log("Game quit.");
        #endif
    }
}
