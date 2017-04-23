using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {

    private static readonly int LEFT = 0;
    private static readonly int RIGHT = 1;


    public List<WheelCollider> frontWheels; // front axle
    public List<WheelCollider> rearWheels;  //rear axle

    public List<GameObject> frontWheelsMesh;
    public List<GameObject> rearWheelsMesh;

    public Material ColorMaterial;

    public Transform massCentre;
    public Rigidbody bodyCar;

    public float maxMotorTorque;    // maximum torque to wheel
    public float maxSteeringAngle;  // maximum steer angle the wheel can have
    public float BrakeTorquePower;


    private float currentMotorTorque;
    private float currentBrakeTorque;
    private float currentSteering;

    public void Start() {

        currentMotorTorque = 0;
        currentBrakeTorque = 0;
        currentSteering = 0;

    }

    public void FixedUpdate() {

        //Updates the centerofmass
        bodyCar.centerOfMass = massCentre.localPosition;
        manageInput();

        //Updates the torques and the steerings
        frontWheels[LEFT].steerAngle = currentSteering;
        frontWheels[RIGHT].steerAngle = currentSteering;


        rearWheels[LEFT].motorTorque = currentMotorTorque;
        rearWheels[RIGHT].motorTorque = currentMotorTorque;

        rearWheels[LEFT].brakeTorque = currentBrakeTorque;
        rearWheels[RIGHT].brakeTorque = currentBrakeTorque;

        //wheelsAnimation();       

    }

    public void manageInput() {

        currentSteering = maxSteeringAngle * Input.GetAxis("Horizontal");
        currentMotorTorque = maxMotorTorque * Input.GetAxis("Vertical");

        
        if (Input.GetKey("space"))
            toBrake();
        else
            currentBrakeTorque = 0.0f;
       
    }

    public void toBrake() {

        var rigidbody = gameObject.GetComponent<Rigidbody>();
        currentBrakeTorque = rigidbody.mass * BrakeTorquePower;
        currentMotorTorque = 0.0f;
    }

    public float carSpeedFromRMP(float rpm) {
        float radius = rearWheels[0].radius; //in meters

        return ((2 * Mathf.PI * rpm) / 60) * 3.6f; //* 3.6 transforms m/s -> kmh
    }



    public void wheelsAnimation() {

        if (frontWheelsMesh != null && rearWheelsMesh != null) {

            float rotation_angle = 0;
            Transform FR, FL, RL, RR;
            FL = frontWheelsMesh[LEFT].GetComponent<Transform>();
            FR = frontWheelsMesh[RIGHT].GetComponent<Transform>();
            RL = rearWheelsMesh[LEFT].GetComponent<Transform>();
            RR = rearWheelsMesh[RIGHT].GetComponent<Transform>();


            FL.transform.rotation = Quaternion.AngleAxis(currentSteering, Vector3.up);
            FR.transform.rotation = Quaternion.AngleAxis(currentSteering, Vector3.up);

            float rpm = (rearWheels[LEFT].rpm + rearWheels[RIGHT].rpm) / 2;
            rotation_angle = (rpm * 360) / 60 * Time.fixedDeltaTime;

            FL.transform.rotation = Quaternion.AngleAxis(rotation_angle, Vector3.right);
            FR.transform.rotation = Quaternion.AngleAxis(rotation_angle, Vector3.right);
            RL.transform.rotation = Quaternion.AngleAxis(rotation_angle, Vector3.right);
            RR.transform.rotation = Quaternion.AngleAxis(rotation_angle, Vector3.right);


        }
    }

}
