using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CarController : MonoBehaviour {

    private static readonly int LEFT = 0;
    private static readonly int RIGHT = 1;


    public List<WheelCollider> frontWheels; // front axle
    public List<WheelCollider> rearWheels;  //rear axle

    public List<GameObject> frontWheelsMesh;
    public List<GameObject> rearWheelsMesh;

    public GameObject FLL, FRR;


    public Transform massCentre;
    public Rigidbody bodyCar;

    public float maxMotorTorque;    // maximum torque to wheel
    public float maxSteeringAngle;  // maximum steer angle the wheel can have
    public float BrakeTorquePower;


    private float currentMotorTorque;
    private float currentBrakeTorque;
    private float currentSteering;
    private float oldSteering;


    public void Start() {

        WheelFrictionCurve fordward_curve, sideways_curve;

        currentMotorTorque = 0;
        currentBrakeTorque = 0;
        currentSteering = 0;
        oldSteering = 0;

        fordward_curve = new WheelFrictionCurve();
        sideways_curve = new WheelFrictionCurve();

        setWheelFordwardFrictionCurveValues(fordward_curve);
        setWheelSidewaysFrictionCurveValues(sideways_curve);

        //setFrictionConfig(fordward_curve, sideways_curve);
     
        
    }

    public void Update() {

        wheelsAnimation();

    }

    public void FixedUpdate() {

        wheelsPhysics();
    }

    public void wheelsPhysics() {

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
            Transform FR, FL, RL, RR; //Meshes
            WheelCollider WFR, WFL, WRL, WRR; //Wheelcolliders

            //Gets the mesh wheels
            FL = frontWheelsMesh[LEFT].GetComponent<Transform>();
            FR = frontWheelsMesh[RIGHT].GetComponent<Transform>();
            RL = rearWheelsMesh[LEFT].GetComponent<Transform>();
            RR = rearWheelsMesh[RIGHT].GetComponent<Transform>();

            //Gets the WheelColliders
            WFL = frontWheels[LEFT].GetComponent<WheelCollider>();
            WFR = frontWheels[RIGHT].GetComponent<WheelCollider>();
            WRL = rearWheels[LEFT].GetComponent<WheelCollider>();
            WRR = rearWheels[RIGHT].GetComponent<WheelCollider>();


            /*Wheels rotation*/

            //Sets the wheels again in the initial position


            //Sets the new steering
            float aux_oldsteer, aux_steer, angle;
            aux_oldsteer = oldSteering + maxSteeringAngle;
            aux_steer = currentSteering + maxSteeringAngle;

            angle = aux_steer - aux_oldsteer;

            FL.Rotate(Vector3.up, angle);
            FR.Rotate(Vector3.up, angle);
            

            oldSteering = currentSteering; //Updates the old steering for the nect reajust

            //Calculates the angle for rotate the rear wheels
            float rpm = (rearWheels[LEFT].rpm + rearWheels[RIGHT].rpm) / 2;
            rotation_angle = (rpm * 360) / 60 * Time.deltaTime;

            //Rotates the rear wheels
            RL.Rotate(Vector3.right, rotation_angle, Space.Self);
            RR.Rotate(Vector3.right, rotation_angle, Space.Self);

            //Vector to rotate arround when wheels are steered
             
            Vector3 vec = Quaternion.AngleAxis(angle, Vector3.up) * gameObject.transform.right;

            FLL.transform.Rotate(Vector3.right, rotation_angle, Space.Self);
            FRR.transform.Rotate(Vector3.right, rotation_angle, Space.Self);
            
            /*Suspension effects*/
            suspensionEffect(WFL, FL);
            suspensionEffect(WFR, FR);
            suspensionEffect(WRL, RL);
            suspensionEffect(WRR, RR);

        }
    }


    public void suspensionEffect(WheelCollider wc, Transform mesh) {

        RaycastHit hit;
        Vector3 wheelCCenter = wc.transform.TransformPoint(wc.center);

        if (Physics.Raycast(wc.transform.localPosition, -wc.transform.up, out hit, wc.suspensionDistance + wc.radius)) {

            mesh.position = new Vector3(mesh.position.x, hit.point.y + (wc.transform.up.y * wc.radius), mesh.position.z);
        } else {

            //Bug here when the car is falling
            mesh.position = new Vector3(mesh.position.x, wheelCCenter.y - (wc.transform.up.y * wc.suspensionDistance), mesh.position.z);
        }

    }

    //Get the suspension's distance compressed in a wheel collider 
    public float getCompression(WheelCollider wc) {
        float balance, distance, default_balance;

        default_balance = 0.5f;
        balance = wc.suspensionSpring.targetPosition; // [0,1] 0,5 default, when the car is stopped
        distance = wc.suspensionDistance; //0.3 in this car

        Debug.Log("target position->" + balance);

        //factor_balance [-0.5 , 0.5] return -> [-0.15,0.15] //Half of the suspension distance
        float factor_balance = balance - default_balance;
        return -(factor_balance * distance); //+y up, -y down 
    }

    private void setWheelFordwardFrictionCurveValues(WheelFrictionCurve curve) {

        curve.extremumSlip = 0.4f;    //0.4
        curve.extremumValue = 1f;     //1
        curve.asymptoteSlip = 0.8f;   //0.8
        curve.asymptoteValue = 0.5f;  //0.5
        curve.stiffness = 1.0f;       //1
    }

    public void setWheelSidewaysFrictionCurveValues(WheelFrictionCurve curve) {
        
        curve.extremumSlip = 0.2f;          //0.2
        curve.extremumValue = 1.0f;     //1
        curve.asymptoteSlip = 0.5f;         //0.5
        curve.asymptoteValue = 0.75f;    //0.75
        curve.stiffness = 1.0f;             //1

    }

    private void setFrictionConfig(WheelFrictionCurve fordward_curve, WheelFrictionCurve sideways_curve) {
        foreach (var collider in frontWheels) {
            collider.forwardFriction = fordward_curve;
            collider.sidewaysFriction = sideways_curve;
        }

        foreach (var collider in rearWheels) {
            collider.forwardFriction = fordward_curve;
            collider.sidewaysFriction = sideways_curve;
        }
    }


}


class Engie {

    public readonly int GEAR_NUMS = 6;
    public readonly int MAX_REVS = 9000;
    public readonly int MIN_REVS = 1000;

    private int currentGear;
    private int currentRevs;

    public Engie() {

    }
}

