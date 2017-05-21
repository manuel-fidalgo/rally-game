using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


    private int ROTATION_SPEED = 5;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

        Transform car; float angle_x = 0; //float zoom = 0;


        car = gameObject.transform.root;
        angle_x = ROTATION_SPEED * Input.GetAxis("Mouse X");
        //zoom = Input.
        gameObject.transform.RotateAround(car.position, car.transform.up, angle_x);

    }
}
