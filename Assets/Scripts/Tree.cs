using UnityEngine;

public class Tree : MonoBehaviour {

	// Use this for initialization
	void Start () {
        printChildrens();
	}

    private void printChildrens() {
        printChildrensRec(gameObject,0);
    }

    //White box test
    private void printChildrensRec(GameObject gameObject, int deep) {
        int childrens = gameObject.transform.childCount;
        string ret = "";

        for (int j = 0; j < deep; j++) {
            ret = ret + " ";
        }

        if (childrens == 0) {

            Debug.Log(ret+gameObject.name); //Its a leaf

        } else {

            Debug.Log(ret + gameObject.name);
            deep++;
            for (int i = 0; i < childrens; i++) {   //Recursive call for each 
                printChildrensRec(gameObject.transform.GetChild(i).gameObject, deep);
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
