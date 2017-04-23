using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickerSceneController : MonoBehaviour {

    public List<GameObject> cars;
    public List<Material> cars_materials;

    List<Color> avaliable_colors;
    Vector3[] positions;

    int currentPosition;
    int currentColor;

	// Use this for initialization
	void Start () {
        positions = new Vector3[4];
        positions[0] = new Vector3(68.4f, -55.7f, 126);
        positions[1] = new Vector3(76.5f, -55.7f, 126);
        positions[2] = new Vector3(86.0f, -55.7f, 126);
        positions[3] = new Vector3(93.9f, -55.7f, 126);
        currentPosition = 0;
        currentColor = 0;
        for (int i = 0; i < cars.Count; i++) {
            Object.DontDestroyOnLoad(cars[i]);
        }

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


        Debug.Log(currentColor);

        if (Input.GetKey("space")) {
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
