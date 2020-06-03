using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using TMPro;

public class AIFightPlane : Agent
{
    [Header("Changable Variables")]
    [SerializeField] private float PlaneSpeed = 0;
    [SerializeField] private float Acceleration = 0;
    [SerializeField] private float Sensitivity = 0;
    private bool dead;
    private bool won;
    private bool TriggeringCollider;

    public Transform Target1;
    public Transform Target2;
    public Transform Target3;
    public Transform Target4;
    public Transform Target5;



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



    public override void OnEpisodeBegin()
    {
        //this.gameObject.GetComponent<AudioSource>().Play();
        dead = false;
        animInProgress = false;
        privateScore = 0;
        health = maxHealth;
        PlaneSpeed = 20; // hardCoding to 20 now
        bullet.SetActive(false);
        //Debug.Log("plane pilot script added to: " + gameObject.name);
        this.transform.localPosition = new Vector3(Random.Range(0, 800), Random.Range(100,200), Random.Range(0, 800));
    }


    public override void OnActionReceived(float[] vectorAction)
    {

        if (health <= 0)
            dead = true;
        if (dead)
        {
            if (!animInProgress)
            {
                // scoreBoard.text = this.name + "\t\tHP[DEAD]\tScore[" + privateScore.ToString() + "]";
                //this.gameObject.GetComponent<AudioSource>().Stop();
                //guns.gameObject.GetComponent<AudioSource>().Stop();
                Explode();
            }
        }
        else
        {
            //if (shootInProgress)
            // guns.GetComponent<AudioSource>().Play();
            //else
            //guns.GetComponent<AudioSource>().Stop();


            AddReward(0.01f); // adding a small reward for just staying alive

            //scoreBoard.text = this.name + "\t\tHP[" + health + "]\tScore[" + privateScore.ToString() + "]";


            PlaneSpeed -= transform.forward.y * Time.deltaTime * Acceleration;
            transform.localPosition += transform.forward * Time.deltaTime * PlaneSpeed;

            if (PlaneSpeed < 5)
                PlaneSpeed = 5;



            if (this.gameObject.transform.localPosition.x > 1000 || this.gameObject.transform.localPosition.x < 0 || this.gameObject.transform.localPosition.z > 1000 || this.gameObject.transform.localPosition.z < 0)
                health = 0;
            // temp forcing plane to be in field


            Vector3 controlSignal = Vector3.zero;
            //controlSignal.x = vectorAction[0];
            //controlSignal.z = vectorAction[1];

            //if (Mathf.FloorToInt(vectorAction[0]) == 1)
            //{
            //    AddReward(0.0001f);
            //    transform.Rotate(Input.GetAxis("Vertical") * Sensitivity, 0.0f, -Input.GetAxis("Horizontal") * Sensitivity);
            //}
            if (Mathf.FloorToInt(vectorAction[0]) == 1)
            {
                controlSignal.x = vectorAction[0]; //up down
            }
            if (Mathf.FloorToInt(vectorAction[0]) == 2)
            {
                controlSignal.x = -vectorAction[0]; //up down
            }
            if (Mathf.FloorToInt(vectorAction[0]) == 3)
            {
                controlSignal.z = vectorAction[0];
            }
            if (Mathf.FloorToInt(vectorAction[0]) == 4)
            {
                controlSignal.z = -vectorAction[0];
            }
            if (Mathf.FloorToInt(vectorAction[0]) == 5)
            {
                Shoot();
            }
           transform.Rotate(controlSignal * Sensitivity);
        }
    }
    /*
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.rotation.z);
        sensor.AddObservation(this.transform.rotation.x);
        sensor.AddObservation(this.transform.rotation.y);
        sensor.AddObservation(this.transform.position.x);
        sensor.AddObservation(this.transform.position.y);
        sensor.AddObservation(this.transform.position.z);
        sensor.AddObservation(this.transform.forward);
        sensor.AddObservation(PlaneSpeed);
        sensor.AddObservation(health);
        sensor.AddObservation(Sensitivity);
        sensor.AddObservation(Acceleration);
        if (Target1 != null) sensor.AddObservation(Target1.position);
        if (Target2 != null) sensor.AddObservation(Target2.position);
        if (Target3 != null) sensor.AddObservation(Target3.position);
        if (Target4 != null) sensor.AddObservation(Target4.position);
        if (Target5 != null) sensor.AddObservation(Target5.position);
    }
    */
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        //if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("d") || Input.GetKey("s"))
        //    actionsOut[0] = 1;
        if (Input.GetKey("w") || Input.GetKey("up"))
            actionsOut[0] = 1;
        if (Input.GetKey("s") || Input.GetKey("down"))
            actionsOut[0] = 2;
        if (Input.GetKey("a") || Input.GetKey("left"))
            actionsOut[0] = 3;
        if (Input.GetKey("d") || Input.GetKey("right"))
            actionsOut[0] = 4;

        if (Input.GetKey("space"))
        {
            actionsOut[0] = 5;
           // shootInProgress = true;
        }
        //else
          //  shootInProgress = false;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            dead = true;
        if (dead)
        {
            if (!animInProgress)
            { 
               // scoreBoard.text = this.name + "\t\tHP[DEAD]\tScore[" + privateScore.ToString() + "]";
                //this.gameObject.GetComponent<AudioSource>().Stop();
                //guns.gameObject.GetComponent<AudioSource>().Stop();
                Explode();
            }
        }
        else
        {
            //if (shootInProgress)
               // guns.GetComponent<AudioSource>().Play();
            //else
                //guns.GetComponent<AudioSource>().Stop();


            AddReward(0.0001f); // adding a small reward for just staying alive

            //scoreBoard.text = this.name + "\t\tHP[" + health + "]\tScore[" + privateScore.ToString() + "]";


            PlaneSpeed -= transform.forward.y * Time.deltaTime * Acceleration;
            transform.localPosition += transform.forward * Time.deltaTime * PlaneSpeed;

            if (PlaneSpeed < 5)
                PlaneSpeed = 5;

            if (this.gameObject.transform.localPosition.x > 1000 || this.gameObject.transform.localPosition.x < 0 || this.gameObject.transform.localPosition.z > 1000 || this.gameObject.transform.localPosition.z < 0)
                health = 0;
            // temp forcing plane to be in field
        }

    }
    */

    void Shoot()
    {
        AddReward(0.01f); // small reward for shooting
        //RaycastHit hit = new RaycastHit();
        Vector3 fwd = guns.transform.TransformDirection(Vector3.forward);
        GameObject tempBullet;
        tempBullet = Instantiate(bullet, bullet.transform.position, bullet.transform.localRotation) as GameObject;
        tempBullet.GetComponent<Rigidbody>().isKinematic = false;
        tempBullet.SetActive(true);
        tempBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
        Destroy(tempBullet, 2.0f);
    }

    void Explode()
    {
        animInProgress = true;
        //ParticleSystem explodeP = ExplosionObject.GetComponent<ParticleSystem>();
        //AudioClip explodeA = ExplosionObject.GetComponent<AudioSource>().clip;
        //animationDuration = explodeP.main.duration;
        PlaneSpeed = 0;
        //explodeP.Play();
        //AudioSource.PlayClipAtPoint(explodeA, transform.position);
        //StartCoroutine(WaitForAnimation(animationDuration));
        AddReward(-1.0f);
        //Debug.Log(message: GetCumulativeReward());
        EndEpisode();

    }

    IEnumerator WaitForAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log(message: GetCumulativeReward());
        EndEpisode();
        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Wall")
        {
            AddReward(-2.0f);//big loss if crashing with terrain
            health = 0;
        }
        if (other.gameObject.tag == "Bullet")
        {
            AddReward(-0.1f);
            health -= other.gameObject.GetComponent<BulletScript>().damage;
            other.gameObject.GetComponent<BulletScript>().shooter.SendMessage("AddToScore", 1);
            Debug.Log(this.gameObject.name + " Got Hit By " + other.gameObject.GetComponent<BulletScript>().shooter.name);
        }
    }


    public void AddToScore(int points)
    {
        AddReward(2.0f);//huge reward for hitting target
        Debug.Log(message: GetCumulativeReward());
        this.privateScore += points;
    }

}
