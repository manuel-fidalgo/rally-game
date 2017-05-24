using UnityEngine;
using System.Collections.Generic;
using System;

public class CarController : MonoBehaviour {

    private static readonly int LEFT = 0;
    private static readonly int RIGHT = 1;

    private static readonly string ROAD_MATERIAL_NAME = "MaxFriction";

    public List<WheelCollider> frontWheels; // front axle
    public List<WheelCollider> rearWheels;  //rear axle

    public List<GameObject> frontWheelsMesh;
    public List<GameObject> rearWheelsMesh;

    public GameObject FLL, FRR; //auxiliar elemets for the double rotation

    private float maxMotorTorque;    // maximum torque to wheel
    private float maxSteeringAngle;  // maximum steer angle the wheel can have
    private float BrakeTorquePower;


    private float currentMotorTorque;
    private float currentBrakeTorque;
    private float currentSteering;
    private float oldSteering;

    private ParticleSystem rightWheel;
    private ParticleSystem leftWheel;

    private Engine engine; //Car's engine

    public float SmokeRate;

    private WheelFrictionCurve default_;
    private WheelFrictionCurve braking_;

    public AudioSource skid;

    public void Start() {

        engine = Engine.getEngine();
        engine.startEngine();

        GetComponent<Rigidbody>().centerOfMass += new Vector3(0, -1.0f, 1.0f);

        maxMotorTorque = 5000;    // maximum torque to wheel
        maxSteeringAngle = 30;  // maximum steer angle the wheel can have
        BrakeTorquePower = 5000;

        currentMotorTorque = 0;
        currentBrakeTorque = 0;
        currentSteering = 0;
        oldSteering = 0;

        setFrictionConfig();
    }

    public void Update() {

        skidSound();
        wheelsAnimation();
        smokeEmitter();
    }



    public void FixedUpdate() {

        wheelsPhysics();
        //antiRollsPhysics(); //Better results without
    }

    public void wheelsPhysics() {

        //Updates the centerofmass

        manageInput();

        //Updates the torques and the steerings
        frontWheels[LEFT].steerAngle = currentSteering;
        frontWheels[RIGHT].steerAngle = currentSteering;


        rearWheels[LEFT].motorTorque = currentMotorTorque;
        rearWheels[RIGHT].motorTorque = currentMotorTorque;

        rearWheels[LEFT].brakeTorque = currentBrakeTorque;
        rearWheels[RIGHT].brakeTorque = currentBrakeTorque;
        frontWheels[LEFT].brakeTorque = currentBrakeTorque;
        frontWheels[RIGHT].brakeTorque = currentBrakeTorque;

    }

    private void antiRollsPhysics() {

        float AntiRoll = 35000.0f;
        WheelHit hit;

        float travelL = 1.0f;
        float travelR = 1.0f;
        float antiRollForce;
        bool groundedL, groundedR;

        //Front axle
        groundedL = frontWheels[LEFT].GetGroundHit(out hit);
        if (groundedL)
            travelL = (-frontWheels[LEFT].transform.InverseTransformPoint(hit.point).y - frontWheels[LEFT].radius) / frontWheels[LEFT].suspensionDistance;

        groundedR = frontWheels[RIGHT].GetGroundHit(out hit);
        if (groundedR)
            travelR = (-frontWheels[RIGHT].transform.InverseTransformPoint(hit.point).y - frontWheels[RIGHT].radius) / frontWheels[RIGHT].suspensionDistance;

        antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            GetComponent<Rigidbody>().AddForceAtPosition(frontWheels[LEFT].transform.up * -antiRollForce, frontWheels[LEFT].transform.position);
        if (groundedR)
            GetComponent<Rigidbody>().AddForceAtPosition(frontWheels[RIGHT].transform.up * antiRollForce, frontWheels[RIGHT].transform.position);

        //Rear Axle
        groundedL = rearWheels[LEFT].GetGroundHit(out hit);
        if (groundedL)
            travelL = (-rearWheels[LEFT].transform.InverseTransformPoint(hit.point).y - rearWheels[LEFT].radius) / rearWheels[LEFT].suspensionDistance;

        groundedR = rearWheels[RIGHT].GetGroundHit(out hit);
        if (groundedR)
            travelR = (-rearWheels[RIGHT].transform.InverseTransformPoint(hit.point).y - rearWheels[RIGHT].radius) / rearWheels[RIGHT].suspensionDistance;

        antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            GetComponent<Rigidbody>().AddForceAtPosition(rearWheels[LEFT].transform.up * -antiRollForce, rearWheels[LEFT].transform.position);
        if (groundedR)
            GetComponent<Rigidbody>().AddForceAtPosition(rearWheels[RIGHT].transform.up * antiRollForce, rearWheels[RIGHT].transform.position);



    }

    public void manageInput() {

        float vertical_axe, horizontal_axe;
        float speed = getSpeed();

        bool is_braking = false;

        
        vertical_axe = Input.GetAxis("Vertical");
        horizontal_axe = Input.GetAxis("Horizontal");

        currentSteering = maxSteeringAngle * horizontal_axe;

        if (vertical_axe >= 0) {
            currentMotorTorque = -maxMotorTorque * vertical_axe;
        } else {
            if (speed < 30) {
                currentMotorTorque = -maxMotorTorque / 10 * vertical_axe; //vertical axe < 0
            }else {
                is_braking = true;
            }
        }

        //Manages if the hand brake is activated, changes the wheels' sideway frictions
        if (Input.GetKey("space")) {
    
            engine.setRevsAndGearFromSpeed(speed, -1);
            setBrakingFriction();
            is_braking = true;
           
        } else {

            engine.setRevsAndGearFromSpeed(speed, vertical_axe);
            setNormalFriction();
        }

        //Ligths and braketorques.
        if (is_braking) {
            toBrake();
            setLights(true);
          
        } else {
            currentBrakeTorque = 0.0f;
            setLights(false);

        }

    }

    public void skidSound() {

        bool on_road = false; //At least one is touching the road
        float fordward_aver, side_aver;

        float volume_factor = 0.75f;
        if (!skid.isPlaying) {
            skid.Play();
        }
        WheelHit FL, FR, RL, RR;

        frontWheels[LEFT].GetGroundHit(out FL);
        frontWheels[RIGHT].GetGroundHit(out FR);
        rearWheels[LEFT].GetGroundHit(out RL);
        rearWheels[RIGHT].GetGroundHit(out RR);

        fordward_aver = (FL.forwardSlip + FR.forwardSlip + RL.forwardSlip + RR.forwardSlip) / 4;
        side_aver = (FL.sidewaysSlip + FR.sidewaysSlip + RL.sidewaysSlip + RR.sidewaysSlip) / 4;

        try {
            on_road  = FL.collider.material.name.Contains(ROAD_MATERIAL_NAME) ||
                            FR.collider.material.name.Contains(ROAD_MATERIAL_NAME) ||
                            RL.collider.material.name.Contains(ROAD_MATERIAL_NAME) ||
                            RR.collider.material.name.Contains(ROAD_MATERIAL_NAME);
        }
        catch(NullReferenceException) {
        
            on_road = false; //If the car is in the air;
        }

        if (fordward_aver > 0.5 && on_road) {
            skid.volume = fordward_aver * volume_factor;
        }else if(side_aver > 0.3 && on_road) {
            skid.volume = side_aver * volume_factor;
        }else {
            skid.volume = 0;
        }

    }

    private void setLights(bool status) {
        transform.Find("BrakeLights").gameObject.SetActive(status);
    }

    private void setNormalFriction() {

        foreach (var collider in rearWheels)
            collider.sidewaysFriction = default_;

    }

    private void setBrakingFriction() {

        foreach (var collider in rearWheels)
            collider.sidewaysFriction = braking_;

    }

    public void toBrake() {
        var rigidbody = gameObject.GetComponent<Rigidbody>();
        currentBrakeTorque = rigidbody.mass * BrakeTorquePower;
        //currentMotorTorque = 0.0f;

    }

    public float getSpeed() {

        return GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
    }
    //UnUsed
    public float carSpeedFromRMP(float rpm) {

        float speed, radius;

        radius = rearWheels[LEFT].radius; //in meters
        speed = ((2 * Mathf.PI * rpm) / 60) * 3.6f; //* 3.6 transforms m/s -> kmh
        return -speed;
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

            //Sets the new steering
            float aux_oldsteer, aux_steer, angle;
            aux_oldsteer = oldSteering + maxSteeringAngle;
            aux_steer = currentSteering + maxSteeringAngle;

            angle = aux_steer - aux_oldsteer;

            FL.Rotate(Vector3.up, angle);       //rotates for the steering
            FR.Rotate(Vector3.up, angle);


            oldSteering = currentSteering; //Updates the old steering for the next reajust

            //Calculates the angle for rotate the rear wheels
            float rpm = (rearWheels[LEFT].rpm + rearWheels[RIGHT].rpm) / 2;
            rotation_angle = (-rpm * 360) / 60 * Time.deltaTime;

            //Rotates the rear wheels
            RL.Rotate(Vector3.right, rotation_angle, Space.Self);
            RR.Rotate(Vector3.right, rotation_angle, Space.Self);


            FLL.transform.Rotate(Vector3.right, rotation_angle, Space.Self); //rotates for the speed
            FRR.transform.Rotate(Vector3.right, rotation_angle, Space.Self);


            /*Suspension effects*/

            suspensionEffect(WFL, FL);
            suspensionEffect(WFR, FR);
            suspensionEffect(WRL, RL);
            suspensionEffect(WRR, RR);

        }
    }

    private void smokeEmitter() {

        WheelHit hit;

        if (rearWheels[RIGHT].GetGroundHit(out hit)) {

            if (!rearWheels[RIGHT].GetComponent<ParticleSystem>().isPlaying)
                rearWheels[RIGHT].GetComponent<ParticleSystem>().Play();

            if (Math.Abs(hit.sidewaysSlip) > 0.25 && hit.collider.material.name.Contains(ROAD_MATERIAL_NAME)) {

                var emission = rearWheels[RIGHT].GetComponent<ParticleSystem>().emission;
                emission.rateOverTime = Math.Abs(hit.sidewaysSlip) * SmokeRate;

            } else {
                var emission = rearWheels[RIGHT].GetComponent<ParticleSystem>().emission;
                emission.rateOverTime = 0;
            }
        }

        if (rearWheels[LEFT].GetGroundHit(out hit)) {

            if (!rearWheels[LEFT].GetComponent<ParticleSystem>().isPlaying)
                rearWheels[LEFT].GetComponent<ParticleSystem>().Play();

            if (Math.Abs(hit.sidewaysSlip) > 0.25 && hit.collider.material.name.Contains(ROAD_MATERIAL_NAME)) {

                var emission = rearWheels[LEFT].GetComponent<ParticleSystem>().emission;
                emission.rateOverTime = Math.Abs(hit.sidewaysSlip) * SmokeRate;
            } else {

                var emission = rearWheels[LEFT].GetComponent<ParticleSystem>().emission;
                emission.rateOverTime = 0;
            }
        }

    }


    public void suspensionEffect(WheelCollider Wheel, Transform WheelMesh) {

        RaycastHit hit;
        Vector3 wheelCCenter = Wheel.transform.TransformPoint(Wheel.center);


        if (Physics.Raycast(wheelCCenter, -Wheel.transform.up, out hit, Wheel.suspensionDistance + Wheel.radius)) {

            WheelMesh.position = hit.point + (Wheel.transform.up * Wheel.radius);
        } else {

            WheelMesh.position = wheelCCenter - (Wheel.transform.up * Wheel.suspensionDistance);
        }
    }


    //Get the suspension's distance compressed in a wheel collider 
    public float getCompression(WheelCollider wc) {
        float balance, distance, default_balance;

        default_balance = 0.5f;
        balance = wc.suspensionSpring.targetPosition; // [0,1] 0,5 default, when the car is stopped
        distance = wc.suspensionDistance; //0.3 in this car


        //factor_balance [-0.5 , 0.5] return -> [-0.15,0.15] //Half of the suspension distance
        float factor_balance = balance - default_balance;
        return -(factor_balance * distance); //+y up, -y down 
    }

    private void setFrictionConfig() {


        default_ = new WheelFrictionCurve();
        default_.extremumSlip = 0.2f;
        default_.extremumValue = 1f;
        default_.asymptoteSlip = 0.5f;
        default_.asymptoteValue = 0.75f;
        default_.stiffness = 1f;


        braking_ = new WheelFrictionCurve();
        braking_.extremumSlip = 0.2f;
        braking_.extremumValue = 1f;
        braking_.asymptoteSlip = 0.5f;
        braking_.asymptoteValue = 0.75f;
        braking_.stiffness = 0.5f;


        foreach (var collider in frontWheels) {

            collider.sidewaysFriction = default_;
        }

        foreach (var collider in rearWheels) {
            collider.sidewaysFriction = default_;
        }

    }


}


public class Engine {

    public bool neutral;
    public bool accelerating;
    public bool deccelerating;

    public readonly int GEAR_NUMS = 5;
    public readonly int MAX_REVS = 10000;
    public readonly int CHANGE_REVS = 7000;
    public readonly int MIN_REVS = 850;

    static int STOP = 1, MAX_SPEED = 250;
    static int CHANGE_1 = 30, CHANGE_2 = 70, CHANGE_3 = 120, CHANGE_4 = 180;



    public int currentGear { get; set; }
    public float currentRevs { get; set; }

    private static Engine singleton;

    private Engine() {
        currentGear = 0;
        currentRevs = 0;
    }

    public static Engine getEngine() {
        if (singleton == null) {
            singleton = new Engine();
        }
        return singleton;
    }

    public void startEngine() {
        currentGear = 0;
        currentRevs = MIN_REVS;
    }

    public void setRevsAndGearFromSpeed(float speed, float acc) {
        if (speed <= STOP) {
            currentGear = 0;
            currentRevs = MIN_REVS;
        } else if (speed <= CHANGE_1) {
            currentGear = 1;
            if (acc > 0)
                setRevs(speed, STOP, CHANGE_1);
            else
                setRevsAccelerating(speed, STOP, CHANGE_1, acc);
        } else if (speed <= CHANGE_2) {
            currentGear = 2;
            setRevsAccelerating(speed, CHANGE_1, CHANGE_2, acc);
        } else if (speed <= CHANGE_3) {
            currentGear = 3;
            setRevsAccelerating(speed, CHANGE_2, CHANGE_3, acc);
        } else if (speed <= CHANGE_4) {
            currentGear = 4;
            setRevsAccelerating(speed, CHANGE_3, CHANGE_4, acc);
        } else if (speed <= MAX_SPEED) {
            currentGear = 5;
            setRevsAccelerating(speed, CHANGE_4, MAX_SPEED, acc);
        } else {
            currentGear = 5;
            currentRevs = MAX_REVS;
        }
    }
    private void setRevs(float speed, int minlimit, int maxLimit) {
        float norm_speed = speed - minlimit;
        float norm_limit = maxLimit - minlimit;

        currentRevs = (MAX_REVS * norm_speed) / norm_limit;
    }

    private void setRevsAccelerating(float speed, int minlimit, int maxLimit, float acceleratig) {
        float norm_speed = speed - minlimit;
        float norm_limit = maxLimit - minlimit;

        float norm_revs = MAX_REVS - CHANGE_REVS;

        if (acceleratig > 0) {
            currentRevs = ((norm_revs * norm_speed) / norm_limit) + CHANGE_REVS; //(7000 - 10000)
        } else {
            currentRevs = ((norm_revs * norm_speed) / norm_limit) + MIN_REVS;
        }
    }
}


