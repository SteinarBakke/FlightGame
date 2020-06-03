using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartRaceGame()
    {
        SceneManager.LoadScene("PlaneRace");
    }

    public void LoadRaceGame()
    {
        SceneManager.LoadScene("PlaneRaceMenu");
    }
    public void StartFightingGame()
    {
        SceneManager.LoadScene("PlaneFight");
    }
    public void LoadFightingGame()
    {
        SceneManager.LoadScene("PlaneFightMenu");
    }



    public void QuitApplicationGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
