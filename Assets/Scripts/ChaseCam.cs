using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCam : MonoBehaviour
{
    public GameObject PlaneObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //instead of transform.up which would always look up to plane, doing Vector3.up is universal UP
        Vector3 moveCamTo = PlaneObject.transform.position - PlaneObject.transform.forward * 10.0f + Vector3.up * 5.0f;

        //Using Spring Function" method to make it not snap to object
        //Really nice method to smoothen out camera
        float bias = 0.99f;
        Camera.main.transform.position = Camera.main.transform.position * bias + moveCamTo * (1.0f-bias);
        //adding this to look 30m infront of the plane
        Camera.main.transform.LookAt(PlaneObject.transform.position + PlaneObject.transform.forward * 30.0f); ;
        
    }
}
