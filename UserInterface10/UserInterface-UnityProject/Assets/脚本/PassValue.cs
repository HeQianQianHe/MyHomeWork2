using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PassValue : MonoBehaviour
{
    PostProcessVolume ppv;
    void Start()
    {
        ppv = GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderDrag(float a)
    {
        ppv.weight = a;
    }
}
