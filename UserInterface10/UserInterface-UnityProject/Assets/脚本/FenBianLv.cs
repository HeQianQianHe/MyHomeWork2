using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenBianLv : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 960, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int level;
    public string[] levels;
    int Level
    {
        get
        {
            return Mathf.Clamp(level, 0, levels.Length - 1);
        }
    }
    void Awake()
    {
        QualitySettings.SetQualityLevel(Level, true);
    }
    void OnValidate()
    {
        levels = QualitySettings.names;
        level = Level;
    }

}
