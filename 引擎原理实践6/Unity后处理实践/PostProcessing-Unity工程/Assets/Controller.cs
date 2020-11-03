using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(
        1920,
        1080,
        false);
        Invoke("ReLoad",5);
    }
    void ReLoad()
    {
        Scene scene = SceneManager.GetActiveScene();
        
        SceneManager.LoadScene(scene.name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
