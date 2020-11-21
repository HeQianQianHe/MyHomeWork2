using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene5Control : MonoBehaviour
{
    public GameObject prefab_chengxu,prefab_yishu,prefab_nv;
   public  GameObject _chengxu, _yishu, _nv;
    public Transform targetAll, targeteye1, targeteye2, targetbi, targetzui, targeter1, targeter2;
    public GameObject heibai;
    bool isheibai = false;
    public GameObject pointlight, pointlight2,dayLight;
    bool islight = false;
    bool isdaylight = false;
    public GameObject fanHuiPage;
    Music55 music;
    void Start()
    {
        music = FindObjectOfType<Music55>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Createchengxu();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Createnv();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Createyishu();
        }
    }

    public void Createchengxu()
    {
        if (_chengxu != null)
        {
            return;
        }
        if (_nv != null)
        {
            Destroy(_nv);
        }
        if (_yishu != null)
        {
            Destroy(_yishu);
        }

        _chengxu = Instantiate(prefab_chengxu, new Vector3(-2.61f, 0.96f, -7.6f),Quaternion.Euler(0,180,0));

        AddTargetFollow(_chengxu);
    }

    public void Createnv()
    {
        if (_nv != null)
        {
            return;
        }
        if (_chengxu != null)
        {
            Destroy(_chengxu);
        }
        if (_yishu != null)
        {
            Destroy(_yishu);
        }
        _nv = Instantiate(prefab_nv, new Vector3(-1.13f, 0.85f, -10.26f), Quaternion.Euler(0, 180, 0));
        AddTargetFollow(_nv);
    }

    public void Createyishu()
    {
        if (_yishu != null)
        {
            return;
        }
        if (_nv != null)
        {
            Destroy(_nv);
        }
        if (_chengxu != null)
        {
            Destroy(_chengxu);
        }
        _yishu = Instantiate(prefab_yishu, new Vector3(-3.81f, 1.01f,-9.34f), Quaternion.Euler(0, 180, 0));
        AddTargetFollow(_yishu);
    }

    public void HeiBai()
    {
        music.BlankWhritMusic();
        if (isheibai == false)
        {
            heibai.SetActive(true);
            isheibai = !isheibai;
        }
        else
        {
            isheibai = !isheibai;
            heibai.SetActive(false);
        }
    }

    public void KaiDeng()
    {
        music.CloseLightMusic();
        if (islight == false)
        {
            pointlight.SetActive(false); pointlight2.SetActive(false);
            islight = !islight; music.CloseLightMusic();
        }
        else
        {
            music.LightMusic();
            pointlight2.SetActive(true); pointlight.SetActive(true);
            islight = !islight;
        }
    }

    public void RiGuang()
    {
        if (isdaylight == false)
        {
            dayLight.SetActive(false);
            music.CloseLightMusic();
            isdaylight = !isdaylight;
        }
        else
        {
            dayLight.SetActive(true);
            music.DayLightMusic();
            isdaylight = !isdaylight;
        }
    }

    public void FanHuiZhuCaiDan()
    {
        music.ClickMusic();
        fanHuiPage.SetActive(true);
    }
    public void ShiDe()
    {
        music.ClickMusic();
        SceneManager.LoadScene("Scene0_");
    }
    public void BuShi()
    {
        music.ClickMusic();
        fanHuiPage.SetActive(false);
    }



    void AddTargetFollow(GameObject go)
    {
        PhysicsMove[] father = go.GetComponentsInChildren<PhysicsMove>();
        foreach (PhysicsMove item in father)
        {
            if (item.transform.gameObject.name == "bbi")
            {
                item.GetComponent<RotateToFront>().t = targetbi;
                item.targetObject = targetbi;
            }
            else if (item.transform.gameObject.name == "zzui")
            {
                item.GetComponent<RotateToFront>().t = targetzui;
                item.targetObject = targetzui;
            }
            else if (item.transform.gameObject.name == "eer")
            {
                item.GetComponent<RotateToFront>().t = targeter1;
                item.targetObject = targeter1;
            }
            else if (item.transform.gameObject.name == "eer2")
            {
                item.GetComponent<RotateToFront>().t = targeter2;
                item.targetObject = targeter2;
            }
            else if (item.transform.gameObject.name == "yyan")
            {
                item.GetComponent<RotateToFront>().t = targeteye1;
                item.targetObject = targeteye1;
            }
            else if (item.transform.gameObject.name == "yyan2")
            {
                item.GetComponent<RotateToFront>().t = targeteye2;
                item.targetObject = targeteye2;
            }
            else
            {
                item.targetObject = targetAll;
            }
        }
    }
}
