using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
    public int nowWeapon = -1;//存储现在持有的武器

    public GameObject banshou;//用于控制自己身上的武器组件的显示和隐藏
    public GameObject luoxuan;

    private int banshouID = 0;//暂时没用，用来记录武器的序号
    private int luoxuanID = 1;

    public GameObject banshou_P;//用于丢掉武器，在地面实例化
    public GameObject luoxuan_P;

    void Start () 
    {
        banshou.GetComponent<MeshRenderer>().enabled = false;
        luoxuan.GetComponent<MeshRenderer>().enabled = false;
    }
	

	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            RengWuQi();
        }
	}

    void RengWuQi()
    {
        if (nowWeapon == 0)
        {
            nowWeapon = -1;
            banshou.GetComponent<MeshRenderer>().enabled = false;
            Instantiate(banshou_P, gameObject.transform.position +transform.right*2+Vector3.up+transform.forward*2, Quaternion.Euler(-48, 8, -76));
        }
        else if (nowWeapon == 1)
        {
            nowWeapon = -1;
            luoxuan.GetComponent<MeshRenderer>().enabled = false;
            Instantiate(luoxuan_P, gameObject.transform.position + transform.right*2 + Vector3.up + transform.forward*2, Quaternion.Euler(-48, 8, -76));
        }
    }


    public  void JianWuQi(int id)
    {
        if(id==0)
        {
            nowWeapon = 0;
            banshou.GetComponent<MeshRenderer>().enabled = true;
        }else if(id==1)
        {
            nowWeapon = 1;
            luoxuan.GetComponent<MeshRenderer>().enabled = true;
        }
        
    }


}
