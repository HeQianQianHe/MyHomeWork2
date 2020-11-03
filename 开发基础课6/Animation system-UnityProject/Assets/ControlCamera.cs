using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamera : MonoBehaviour {
    private Transform camerat;
    public  float rotatespeed = 6;
    private Transform playert;

	// Use this for initialization
	void Start ()
    {
        camerat = gameObject.GetComponent<Transform>();
        playert = GameObject.FindWithTag("Player").GetComponent<Transform>();


    }

    // Update is called once per frame
    void Update()
    {


        float y = Input.GetAxis("Mouse Y");
        float x = Input.GetAxis("Mouse X");

        playert.Rotate(0, x * rotatespeed, 0);
        //camerat.Rotate(y * -rotatespeed, 0, 0);


    }
}
