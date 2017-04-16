using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    Vector3 direction;
    float speed;

	// Use this for initialization
	void Start () {
        Debug.Log("Starts the Car Controller");
        direction = Vector3.forward;
	}

    // Update is called once per frame
    void Update()
    {
        inputManager();
        moveCar();
    }

    private void moveCar()
    {
        if (speed > 0)
        {
            Debug.Log("Car moves with speed" + speed +".");
            gameObject.transform.Translate((direction * speed) * Time.deltaTime);
        }
        
    }

    private void brake()
    {
        Debug.Log("Brake");
        if (speed>0) speed--;
    }

    private void turnRight()
    {
        Debug.Log("Right");
        direction = Quaternion.AngleAxis(-1, Vector3.up) * direction;
    }

    private void turnLeft()
    {
        Debug.Log("Left");
        direction = Quaternion.AngleAxis(1, Vector3.up) * direction;
    }

    private void accelerate()
    {
        Debug.Log("Accelerate");
        speed++;
    }

    private void decelerate()
    {
        speed--;
    }

    void inputManager()
    {
        if (Input.GetKey("w"))
        {
            accelerate();
        }else
        {
            decelerate();
        }
        if (Input.GetKey("a"))
        {
            turnLeft();
        }
        if (Input.GetKey("d"))
        {
            turnRight();
        }
        if (Input.GetKey("s") || Input.GetKey("space"))
        {
            brake();
        }
    }
}
