using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject tutorial;

    public bool isActive = true;
    // Start is called before the first frame update
    void Start()
    {
        tutorial.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKey(KeyCode.Return))
       {
        tutorial.SetActive(false);
        isActive = false;
       }

       if (Input.GetKey(KeyCode.Tab))
       {
        isActive = !isActive;
        tutorial.SetActive(isActive);
       }
    }
}
