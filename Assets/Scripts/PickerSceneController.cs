using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickerSceneController : MonoBehaviour {

    public List<GameObject> cars;
    public List<Material> cars_materials;
    public GameObject Audios;
    public AudioSource start;

    List<Color> avaliable_colors;
    Vector3[] positions;

    int currentPosition;
    int currentColor;

    public static GameObject selectedCar;

    public static string CAR_ID = "CAR_ID";
	// Use this for initialization
	void Start () {

        QualitySettings.antiAliasing = 8;
        positions = new Vector3[3];
        positions[0] = new Vector3(77.41f, -55.61f, 125.92f);
        positions[1] = new Vector3(86.0f, -55.61f, 125.92f);
        positions[2] = new Vector3(93.9f, -55.61f, 125.92f);
        currentPosition = 0;
        currentColor = 0;
        
        avaliable_colors = new List<Color>();

        createColors(avaliable_colors);

    }
    public void createColors(List<Color> lst) {

        lst.Add(Color.gray);
        lst.Add(Color.black);
        lst.Add(Color.blue);
        lst.Add(Color.cyan);
        lst.Add(Color.green);
        lst.Add(Color.red);
        lst.Add(Color.yellow);
        lst.Add(Color.white);
        lst.Add(new Color(0.2f, 0.2f, 0.2f, 1));
        lst.Add(Color.grey);
        lst.Add(new Color(0.7f, 0.7f, 0.7f, 1));
        
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown("d"))
            currentPosition = getNextCircular(currentPosition, positions.Length);

        if (Input.GetKeyDown("a"))
            currentPosition = getPreviusCircular(currentPosition, positions.Length);

        gameObject.transform.position = positions[currentPosition];

        if (Input.GetKey("right")) 
            cars[currentPosition].transform.Rotate(Vector3.up, 1);
            
        if (Input.GetKey("left")) 
            cars[currentPosition].transform.Rotate(Vector3.up, -1);

        if (Input.GetKeyDown("up")) {
            currentColor = getNextCircular(currentColor, avaliable_colors.Count);
            cars_materials[currentPosition].color = avaliable_colors[currentColor];
        }

        if (Input.GetKeyDown("down")) {
            currentColor = getPreviusCircular(currentColor, avaliable_colors.Count);
            cars_materials[currentPosition].color = avaliable_colors[currentColor];
        }

        if (Input.GetKey("space")) {

            start.Play();

            selectedCar = cars[currentPosition];
            DontDestroyOnLoad(selectedCar);
            DontDestroyOnLoad(Audios);
            SceneManager.LoadScene(1);
        }
    }
    public int getNextCircular(int current, int elements) {

        current++;
        if (current == elements)
            current = 0;

        return current;
    }

    public int getPreviusCircular(int current, int elements) {

        current--;
        if (current == -1)
            current = elements-1;

        return current;
    }
}
