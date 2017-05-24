using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLap : MonoBehaviour {

   
    public GameObject terrain;
	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter(Collider other) {
      
        //Finish lap
        RaceSceneController rc = terrain.GetComponent<RaceSceneController>();
        rc.finishLap();
    }

    
}
