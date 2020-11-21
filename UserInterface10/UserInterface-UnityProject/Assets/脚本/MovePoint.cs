using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    public GameObject CotrolObj =null;
    public float depth = 10f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           RaycastHit hit;
           if(Physics.Raycast(ray, out hit))
           {
                if (hit.transform.CompareTag("control"))
                {
                    CotrolObj = hit.collider.gameObject;
                }              
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            CotrolObj = null;
        }
        if (CotrolObj != null)
        {   //获取鼠标位置
            Vector3 mouseScreen = Input.mousePosition;
            //锁定z轴的位置
            mouseScreen.z = depth;
            //将屏幕坐标转换为世界坐标
            Vector3 mouseWorld = GetComponent<Camera>().ScreenToWorldPoint(mouseScreen);
            //再将鼠标转化后的世界坐标赋给这个物体，并保持z轴位置不变
            CotrolObj.transform.position = mouseWorld;
        }
    }
}
