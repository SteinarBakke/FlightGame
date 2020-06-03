using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Note :         //Can use here : instead of currentHighScore, currentHighScore1,2,3 for top3

public class FightPlanePilot : MonoBehaviour
{
    [Header("Changable Variables")]
    [SerializeField] private float PlaneSpeed =0;
    [SerializeField] private float Acceleration = 0;
    [SerializeField] private float Sensitivity = 0;
    private bool dead;
    private bool won;
    private bool TriggeringCollider;

    public Vector3 aimPoint;

    public float ShootDistance;

    public HealthBarScript healthbar;
    public int maxHealth;
    private int health;


    public GameObject guns;
    public GameObject bullet;
    public float bulletForce;

    private bool animInProgress;
    private bool shootInProgress;

    public GameObject GameOverText;
    public GameObject ExplosionObject;
    [Header("HUD Elements")]
    [SerializeField] private Transform crosshair = null;// = null;
    public Text scoreBoard;
    [SerializeField] public int privateScore = 0;//

    public LevelManagerScript levelmanager;

    private float animationDuration;
    // Start is called before the first frame update
    void Start()
    {
        //scoreText = GetComponent<Text>();
        privateScore = 0;
        health = maxHealth;
        healthbar.setMaxHealth(maxHealth);
        bullet.SetActive(false);
        Debug.Log("plane pilot script added to: " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            dead = true;
        if (dead)
        {
            if (!animInProgress)
            {
                scoreBoard.text = this.name + "\t\tHP[DEAD]\tScore[" + privateScore.ToString() + "]";
                this.gameObject.GetComponent<AudioSource>().Stop();
                Explode();
            }
        }
        else if (won)
        {
            this.gameObject.GetComponent<AudioSource>().Stop();
            guns.gameObject.GetComponent<AudioSource>().Stop();
            StartCoroutine(WaitForAnimation(5f));
        }
        else
        {
            if (Input.GetKeyDown("space"))
            {
                shootInProgress = !shootInProgress;
                guns.GetComponent<AudioSource>().Play();
            }

            if (Input.GetKeyUp("space"))
            {
                shootInProgress = !shootInProgress;
                guns.GetComponent<AudioSource>().Stop();
            }

            RaycastHit hit = new RaycastHit();
            Vector3 fwd = guns.transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(guns.transform.position, fwd, out hit, ShootDistance))
           {
                //Debug.DrawRay(guns.transform.position, fwd * ShootDistance, Color.red);
                aimPoint = guns.transform.position + (fwd * (hit.distance-3));//Reducing by 10 to not make object collide
                //Debug.DrawRay(guns.transform.position, aimPoint, Color.blue);
                crosshair.GetComponent<SpriteRenderer>().color = Color.red;
                if (shootInProgress)
                {
                    GameObject tempBullet;
                    tempBullet = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
                    //tempBullet.transform.Rotate(Vector3.left * 90);
                    tempBullet.GetComponent<Rigidbody>().isKinematic = false;
                    tempBullet.SetActive(true);
                    tempBullet.GetComponent<Rigidbody>().AddForce(fwd * bulletForce);
                    Destroy(tempBullet, 2.0f);
                }
           }
           else
           {
                aimPoint = guns.transform.position + (fwd * ShootDistance); // placing aimpoint at shootDistance
                crosshair.GetComponent<SpriteRenderer>().color = Color.black;
                if (shootInProgress)
                {
                    GameObject tempBullet;
                    tempBullet = Instantiate(bullet, bullet.transform.position, bullet.transform.localRotation) as GameObject;
                    tempBullet.GetComponent<Rigidbody>().isKinematic = false;
                    tempBullet.SetActive(true);
                    tempBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
                    Destroy(tempBullet, 3.0f);
                }
            }


            scoreBoard.text = this.name + "\t\tHP[" + health+"]\tScore[" + privateScore.ToString()+"]";
            //Debug.Log(hit.point);
            //var crossPos = Camera.main.WorldToViewportPoint(aimPoint);
            //crosshair.transform.position = aimPoint;
            crosshair.transform.position = guns.transform.position + (fwd * ShootDistance);
            crosshair.transform.rotation = (this.transform.rotation);// = new Quaternion(0.0f, 0.0f, 0.0f);

            //scoreText.GetComponent<Text>().text = "Damage Inflicted : " + privateScore.ToString();
            //Debug.DrawRay(this.transform.position, crosshair.position, Color.blue);
            //Debug.Log("AimPoint = " + aimPoint + "    crosshairPos = " + crosshair.position);
            // crosshair.transform.position = aimPoint;
            //Move Forward (Using Delta Time so dist won't be affected by fps)
            transform.localPosition += transform.forward * Time.deltaTime * PlaneSpeed;

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
        }

    }

    void Explode()
    {
        animInProgress = true;
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
        levelmanager.LoadFightingGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            health = 0;
            healthbar.setHealth(health);
        }
        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<BulletScript>().damage;
            other.gameObject.GetComponent<BulletScript>().shooter.SendMessage("AddToScore", 1);
            healthbar.setHealth(health);
        }
    }


    public void AddToScore(int points)
    {
        this.privateScore += points;
    }

}
