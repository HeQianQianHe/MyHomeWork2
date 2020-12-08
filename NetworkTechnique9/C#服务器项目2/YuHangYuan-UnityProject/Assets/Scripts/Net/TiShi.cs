using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiShi : MonoBehaviour {
    private Color touming;
	// Use this for initialization
	void Start () 
    {
        touming = Color.white;
        touming.a = 0;
        gameObject.GetComponent<Text>().color = touming;
	}
	
	// Update is called once per frame
	void Update () 
    {
        gameObject.GetComponent<Text>().color = Color.Lerp(gameObject.GetComponent<Text>().color, touming, Time.deltaTime);

    }

    public void ShowXinXi(string data)
    {
        gameObject.GetComponent<Text>().text = data;
        gameObject.GetComponent<Text>().color = Color.white;
    }
}
