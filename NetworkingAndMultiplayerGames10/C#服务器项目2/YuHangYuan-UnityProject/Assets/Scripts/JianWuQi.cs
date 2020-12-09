using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JianWuQi : MonoBehaviour {
    public int id;

	void Start () {

	}
	

	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"&&other.gameObject.GetComponent<Weapon>().nowWeapon==-1)
        {
            other.gameObject.GetComponent<Weapon>().JianWuQi(id);
            Destroy(gameObject);
        }
    }
}
