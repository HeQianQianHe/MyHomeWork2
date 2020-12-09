using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform playert;
    private Vector3 offect;
    public float smooth = 3;


	void Start ()
    {
        //playert = GameObject.Find("Player0").GetComponent<Transform>();
        //offect = transform.position - playert.position;
    }
	
	void Update ()
    {
        
        float Y = Input.GetAxis("Mouse Y");
        transform.Rotate(new Vector3(-Y,0,0));

        //Vector3 targetpos = playert.position + playert.TransformDirection(offect);
        //transform.position = Vector3.Slerp(transform.position, targetpos, Time.deltaTime * smooth);
        //transform.LookAt(playert.position);

    }
}
