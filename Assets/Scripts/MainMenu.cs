using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public TMP_Text HighScoreUI;
    string nextSceneName = "SampleScene";

    //public AudioSource channel;
   // public AudioClip backGroundMusic;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;


        Debug.Log("Main Menu Start Method Called");
        gameObject.SetActive(true);

        // channel.PlayOneShot(backGroundMusic);
        //Set the hig score text
        int highScore = SaveLoadManager.instance.LoadHighScore();
        HighScoreUI.text = $"Top Wave Survived: {highScore}";
    }

    public void StartNewGame()
    {
        Debug.Log("Loading New Game Scene");

        // channel.Stop();
        SceneManager.LoadScene(nextSceneName);
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif
    }
}
