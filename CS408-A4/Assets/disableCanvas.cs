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
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
            //Destroy(GameObject.Find("Canvas"));
        }
        foreach (char c in Input.inputString.ToLower())
        {
            switch (c)
            {
                case 'i'://instructions
                    {
                        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
                        break;
                    }
            }
        }

    }
}
