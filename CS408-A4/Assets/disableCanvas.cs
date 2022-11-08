using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableCanvas : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.inputString != "")
        {
            //GetComponent<Canvas>().SetActive(false);
            Destroy(GameObject.Find("Canvas"));
            //Destroy(this);
        }
    }
}
