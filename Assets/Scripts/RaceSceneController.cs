using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceSceneController : MonoBehaviour {


    GameObject car, cam;
    CarController cc;
    SoundManager sm;

    Chrono chronometer;



    public GameObject timeonscreen;
    public GameObject besttime;

    static readonly string LAP = "Lap: ";
    static readonly string BEST = "Best: ";
    static readonly string SEC = " s.";

    static readonly string BEST_MIN = "BEST MIN";
    static readonly string BEST_SEC = "BEST SEC";

    int frames = 20;

    // Use this for initialization
    void Start() {
   
        try {
            car = PickerSceneController.selectedCar;
            cam = car.transform.Find("Camera").gameObject;
            cc = car.GetComponent<CarController>();
            sm = car.GetComponent<SoundManager>();

            if (cc != null) cc.enabled = true;
            if (sm != null) sm.enabled = true;
            if (cam != null) cam.SetActive(true);
        }
        catch (NullReferenceException) {
            Debug.Log("Missing Component");
            return; //If the scene is played from the editor without car
        }

        if (car != null) {
            setCar();
        }

        setBestTime(0, 0);
        chronometer = new Chrono();
        startLap();
    }

    // Update is called once per frame
    void Update() {
        upDateChrono();

        if (Input.GetKey(KeyCode.R)) {
            setCar();
            startLap();
        }
        if (Input.GetKey(KeyCode.Escape)) {
            returnToSelector();
        }

    }

    private void upDateChrono() {
        try {
            string st = LAP + chronometer.getTime() + SEC;
            UnityEngine.UI.Text tx = timeonscreen.GetComponent<UnityEngine.UI.Text>();
            tx.text = st;
        }
        catch (Exception e) {
            Debug.Log("Exception chrono" + e.Data);
        }
    }

    private void returnToSelector() {

        PlayerPrefs.Save();
        Destroy(car);
        Destroy(GameObject.Find("Audios"));
        SceneManager.LoadScene(0);

    }

    public void setCar() {

        car.transform.position = new Vector3(483, 1, 77);
        car.transform.localEulerAngles = new Vector3(0, 0, 0);

    }

    public void finishLap() {
        int min, sec = 0;
        min = chronometer.minutes;
        sec = chronometer.seconds;
        setBestTime(min, sec);
        startLap();
    }

    public void setBestTime(int lap_min, int lap_sec) {
        int min, sec;
        min = PlayerPrefs.GetInt(BEST_MIN);
        sec = PlayerPrefs.GetInt(BEST_SEC);

       
        Debug.Log(String.Format("lap->{0}:{1}   best->{2}:{3}",lap_min,lap_sec,min,sec));
        if (!(lap_min==0 && lap_sec == 0)) {
            if (lap_min < min) {
                min = lap_min;
                sec = lap_sec;
            }else if(lap_min == min && lap_sec< sec){
                min = lap_min;
                sec = lap_sec;
            }
        }
        if(min==0 && sec == 0) {
            min = lap_min;
            sec = lap_sec;
        }

        PlayerPrefs.SetInt(BEST_MIN,min);
        PlayerPrefs.SetInt(BEST_SEC,sec);

        UnityEngine.UI.Text tx = besttime.GetComponent<UnityEngine.UI.Text>();
        tx.text = string.Format("Best: {0:00}:{1:00}", min, sec);
    }

    public void startLap() {
        chronometer.Start();
    }
}
