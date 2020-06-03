using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    [Header("Bullet Collide Element")]
    [SerializeField] private GameObject explosion = null;

    [Header("Damage of bullet")]
    [SerializeField] public int damage;

    [Header("Player who sent the bullet")]
    [SerializeField] public Transform shooter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Bullet")
        {
            //Debug.Log("I'm hittin an object = " + other.gameObject.name);
            explosion.GetComponent<ParticleSystem>().Play();
            explosion.GetComponent<AudioSource>().Play();
        }
    }
}
