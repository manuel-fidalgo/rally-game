using UnityEngine;

public class TachometerController : MonoBehaviour {

    GameObject Speed, Needle, car;
    UnityEngine.UI.Text speed_text;
    CarController car_controller;
    Engine engine;

    static readonly string KMH = " Km/h";

	// Use this for initialization
	void Start () {

        Speed = gameObject.transform.Find("Speed").gameObject;
        Needle = gameObject.transform.Find("Needle").gameObject;

        speed_text = Speed.GetComponent<UnityEngine.UI.Text>();
        car = PickerSceneController.selectedCar;
        car_controller = car.GetComponent<CarController>(); 
        engine = Engine.getEngine();
    }
	
	// Update is called once per frame
	void Update () {

        float car_speed, car_revs, angle, max_revs;
        
        car_speed = car_controller.getSpeed();
        car_revs = engine.currentRevs;
        max_revs = engine.MAX_REVS;

        speed_text.text = Mathf.RoundToInt(car_speed) + KMH;
        angle = (car_revs * (-199)) / max_revs; //Final angle.
        
        Needle.transform.localRotation = Quaternion.AngleAxis(angle,Vector3.forward);

    }
}
