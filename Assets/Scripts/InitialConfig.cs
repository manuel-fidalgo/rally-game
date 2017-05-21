using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialConfig : MonoBehaviour {


	// Use this for initialization
	void Start () {

        GameObject car, cam;
        CarController cc;
        SoundManager sm;

        car = PickerSceneController.selectedCar;
        cam = car.transform.Find("Camera").gameObject;
        cc = car.GetComponent<CarController>();
        sm = car.GetComponent<SoundManager>();

        if (cc!=null) cc.enabled = true;
        if (sm != null) sm.enabled = true;
        if (cam!=null) cam.SetActive(true);
        

        if(car!=null) car.transform.position = new Vector3(483, 6, 77);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
