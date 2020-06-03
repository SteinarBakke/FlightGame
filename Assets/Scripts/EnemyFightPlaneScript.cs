using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyFightPlaneScript : MonoBehaviour
{
    [Header("Changable Variables")]
    [SerializeField] private float PlaneSpeed = 0;
    [SerializeField] private float Acceleration = 0;
    [SerializeField] private float Sensitivity = 0;
    private bool dead;
    private bool won;
    private bool TriggeringCollider;


    public float ShootDistance;

    public int maxHealth;
    private int health;


    public GameObject guns;
    public GameObject bullet;
    public float bulletForce;

    private bool animInProgress;
    private bool shootInProgress;

    public GameObject ExplosionObject;
    [Header("HUD Elements")]

    [SerializeField] public int privateScore = 0;//

    public Text scoreBoard;


    private float animationDuration;
    // Start is called before the first frame update
    void Start()
    {
        //scoreText = GetComponent<Text>();
        privateScore = 0;
        health = maxHealth;
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
                guns.gameObject.GetComponent<AudioSource>().Stop();
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
                if (shootInProgress)
                {
                    GameObject tempBullet;
                    tempBullet = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
                    tempBullet.GetComponent<Rigidbody>().isKinematic = false;
                    tempBullet.SetActive(true);
                    tempBullet.GetComponent<Rigidbody>().AddForce(fwd * bulletForce);
                    Destroy(tempBullet, 2.0f);
                }
            }
            else
            {
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


            scoreBoard.text = this.name + "\t\tHP[" + health + "]\tScore[" + privateScore.ToString() + "]";
            transform.localPosition += transform.forward * Time.deltaTime * PlaneSpeed;

            transform.Rotate(Input.GetAxis("Vertical") * Sensitivity, 0.0f, -Input.GetAxis("Horizontal") * Sensitivity);// using general Transform applies to the whole gameobject script is on

            PlaneSpeed -= transform.forward.y * Time.deltaTime * Acceleration;

            if (PlaneSpeed < 5)
                PlaneSpeed = 5;

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
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            health = 0;
        }
        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<BulletScript>().damage;
            other.gameObject.GetComponent<BulletScript>().shooter.SendMessage("AddToScore", 1);
        }
    }


    public void AddToScore(int points)
    {
        this.privateScore += points;
    }

}
