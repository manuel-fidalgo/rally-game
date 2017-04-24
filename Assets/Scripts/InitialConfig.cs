using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialConfig : MonoBehaviour {


	// Use this for initialization
	void Start () {

        GameObject car, cam;
        CarController cc;

        car = PickerSceneController.selectedCar;
        cam = car.transform.Find("Camera").gameObject;
        cc = car.GetComponent<CarController>();

        if(cc!=null) cc.enabled = true;
        if(cam!=null) cam.SetActive(true);

        if(car!=null) Instantiate(car, new Vector3(25,6,25), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
