using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionValue : MonoBehaviour
{
    Material m;
    void Start()
    {
        m = GetComponent<MeshRenderer>().material;
    }

    float value = -0.58f;
    void Update()
    {
        value = value + Time.deltaTime*3;
        if (value >= 2.28f)
        {
            value = -0.58f;
        }
        m.SetFloat("_Radius",value);
    }
}
