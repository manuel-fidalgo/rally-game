using UnityEngine;
using System.Collections;
using System.Collections.Generic;
        
public class CarController : MonoBehaviour
{
    public List<WheelCollider> frontWheels; // front axle
    public List<WheelCollider> rearWheels;  //rear axle

    public Transform massCentre;
    public Rigidbody bodyCar;

    public float maxMotorTorque;    // maximum torque to wheel
    public float maxSteeringAngle;  // maximum steer angle the wheel can have

    public void Start()
    {
        
    }
    //Shoudl i use Update or FixedUpdate?
    public void FixedUpdate()
    {   
        //Updates the centerofmass
        bodyCar.centerOfMass = massCentre.localPosition;

        //gets the torque steering values
       float motor = maxMotorTorque * Input.GetAxis("Vertical");
       float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        
       frontWheels[0].steerAngle = steering;
       frontWheels[1].steerAngle = steering;
            
       rearWheels[0].motorTorque = motor;
       rearWheels[1].motorTorque = motor;
            
    }
}
