using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFront : MonoBehaviour
{
    public  Transform t;
    void Start()
    {
        
    }


    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,t.rotation,Time.deltaTime*10);
    }
}
