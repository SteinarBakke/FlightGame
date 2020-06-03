using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Note :         //Can use here : instead of currentHighScore, currentHighScore1,2,3 for top3

public class PlanePilot : MonoBehaviour
{
    [Header("Changable Variables")]
    [SerializeField] private float PlaneSpeed;
    [SerializeField] private float Acceleration;
    [SerializeField] private float Sensitivity;

    public GameObject GameOverText;
    public GameObject RestartButton;
    public Text ScoreText;
    public Text TimeText;
    public Text HighScoreText;
    private bool dead;
    private bool won;
    public int score;
    private int maxScore;
    public static float timeScore;
    private bool TriggeringCollider;
    private bool RaceStarted;

    private bool animInProgress;

    public GameObject ScoreObject;
    public GameObject ExplosionObject;

    public LevelManagerScript levelmanager;

    private float animationDuration;
    // Start is called before the first frame update
    void Start()
    {

        timeScore = 0;
        maxScore = 12;
        score = 0;
        Debug.Log("plane pilot script added to: " + gameObject.name);
        ScoreText.text = "RINGS [" + score + "/"+maxScore+"]";

        if (PlayerPrefs.HasKey("currentHighScore"))
        {
            HighScoreText.text = "High Score : " + PlayerPrefs.GetFloat("currentHighScore").ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            if (!animInProgress)
            {
                this.gameObject.GetComponent<AudioSource>().Stop();
                Explode();
            }
        }
        else if (won)
        {
            this.gameObject.GetComponent<AudioSource>().Stop();
            StartCoroutine(WaitForAnimation(5f));
        }
        else
        {
            if (score == maxScore)
            {
                WinningConditionMet();
            }
            //Move Forward (Using Delta Time so dist won't be affected by fps)
            transform.position += transform.forward * Time.deltaTime * PlaneSpeed;
    
            //These Inputs are automatic in Unity, can find them in Edit- InputManager
            //Including - infront, since originally it was flipped
            transform.Rotate(Input.GetAxis("Vertical") * Sensitivity, 0.0f, -Input.GetAxis("Horizontal") * Sensitivity);// using general Transform applies to the whole gameobject script is on
    
            //Changing speed based on plane Slope (If it's upwards, positive value, etc)
            PlaneSpeed -= transform.forward.y * Time.deltaTime * Acceleration;
            //Don't want plane to go backwards
            //If I put this On, I allow for "speed cheating"
            if (PlaneSpeed < 5)
                PlaneSpeed = 5;

            //Using this as collide - CHANGED collide to be terrain and box collide
            /*
            float terrainHeightPosition = Terrain.activeTerrain.SampleHeight(transform.position);
            if (terrainHeightPosition >= transform.position.y)
            {
                Debug.Log("Crashing");
                Explode();
                // Add Animation and Game Over here
            }
            */

            if (RaceStarted)
                TimeText.text = "TIME ["+(Time.time - timeScore).ToString()+"]";
        }

    }

    void Explode()
    {
        animInProgress = true;
        RaceStarted = false;
        ParticleSystem explodeP = ExplosionObject.GetComponent<ParticleSystem>();
        AudioClip explodeA = ExplosionObject.GetComponent<AudioSource>().clip;
        animationDuration = explodeP.main.duration;
        PlaneSpeed = 0;
        explodeP.Play();
        AudioSource.PlayClipAtPoint(explodeA, transform.position);
        StartCoroutine(WaitForAnimation(animationDuration));

    }

    IEnumerator WaitForAnimation(float time)
    {
        GameOverText.SetActive(true);// = true;
        //yield on a new YieldInstruction that waits for animation.
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        levelmanager.LoadRaceGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain")
            dead = true;
        else
        {
            TriggeringCollider = true;//add here for particle If I get particle working -> Ring could trigger Fireworks when passed
            //other.gameObject.GetComponent<ParticleSystem>().Play(); //Bad animation and makes it lagg
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TriggeringCollider)
        {
            //ParticleSystem scoreP = ScoreObject.GetComponent<ParticleSystem>();
            AudioClip scoreA = ScoreObject.GetComponent<AudioSource>().clip;
            AudioSource.PlayClipAtPoint(scoreA, transform.position);
            //scoreP.Play();
            if (score == 0) //activate time on first circle
            {
                RaceStarted = true;
                timeScore = Time.time;
            }
            score += 1;
            ScoreText.text = "RINGS [" + score + "/" + maxScore + "]";
            other.gameObject.SetActive(false);
            TriggeringCollider = false;
        }
    }

    private void WinningConditionMet()
    {
        float WinningTime = (Time.time - timeScore);
        GameOverText.SetActive(true);
        ScoreText.text = "";
        TimeText.text = "";
        HighScoreText.text = "";
        RaceStarted = false;
        if (PlayerPrefs.HasKey("currentHighScore"))
        {
            if (WinningTime < PlayerPrefs.GetFloat("currentHighScore"))
            {
                // New High Score!!
                GameOverText.GetComponent<TextMeshProUGUI>().text = "You Won! NEW HIGH SCORE! : " + WinningTime.ToString("F2");
                PlayerPrefs.SetFloat("currentHighScore", WinningTime);
                PlayerPrefs.Save();
            }
            else
                GameOverText.GetComponent<TextMeshProUGUI>().text = "You Won! Time " + WinningTime.ToString("F2");
        }
        else
        {
            GameOverText.GetComponent<TextMeshProUGUI>().text = "You Won Your First Game! : " + WinningTime.ToString("F2");
            PlayerPrefs.SetFloat("currentHighScore", WinningTime);
            PlayerPrefs.Save();
        }
        won = true;
    }
}
