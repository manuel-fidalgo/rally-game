using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


    private int ROTATION_SPEED = 5;
	// Use this for initialization
	void Start () {
		
	}

    void FixedUpdate() {

        Transform car; float angle_x = 0; 

        car = gameObject.transform.root;
        angle_x = ROTATION_SPEED * Input.GetAxis("Mouse X");

        //Not sure if its correct
        gameObject.transform.RotateAround(car.position,Vector3.up, angle_x);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
