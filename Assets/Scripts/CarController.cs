using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    Vector3 direction;
    double speed;

	// Use this for initialization
	void Start () {
        Debug.Log("Starts the Car Controller");
	}
	
	// Update is called once per frame
	void Update () {
        //Testing
        gameObject.transform.Rotate(Vector3.up, 1);
    }
   
}
