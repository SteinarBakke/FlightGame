using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour
{
    public GameObject HighScoreText;
    public GameObject button;
    public static bool opened = false;

    public void LoadHighScore()
    {
        loadUpdatedHighScore();
        opened = !opened;
        if (opened)
        {
            HighScoreText.SetActive(true);
            button.SetActive(true);
        }
        else
        {
            HighScoreText.SetActive(false);
            button.SetActive(false);
        }

    }

    public void ResetHighScore()
    {
        if (PlayerPrefs.HasKey("currentHighScore"))
            PlayerPrefs.DeleteKey("currentHighScore");
        loadUpdatedHighScore();
    }

    private void loadUpdatedHighScore()
    {
        if (PlayerPrefs.HasKey("currentHighScore"))
            HighScoreText.GetComponent<Text>().text = PlayerPrefs.GetFloat("currentHighScore").ToString("F2");
        else
            HighScoreText.GetComponent<Text>().text = "No High Score";
    }
}
