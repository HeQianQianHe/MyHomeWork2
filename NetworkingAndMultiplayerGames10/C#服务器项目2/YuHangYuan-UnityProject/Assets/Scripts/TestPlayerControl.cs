using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerControl : MonoBehaviour
{
    public float rotatespeed = 3;
    public float jumpforce = 120;
    private Animator anim;
    private Rigidbody rig;
    public  float speed = 3;
    public  int space = 0;

    private GameObject player;

	void Start () 
    {
        
	}

    private void OnEnable()
    {
        if (GameFacade.Instance.player == 0)
        {
            Debug.Log("角色0");
            player = GameObject.Find("Player0");
            anim = player.GetComponent<Animator>();
            rig = player.GetComponent<Rigidbody>();
        }
        else if (GameFacade.Instance.player == 1)
        {
            Debug.Log("角色1");
            player = GameObject.Find("Player1");
            anim = player.GetComponent<Animator>();
            rig = player.GetComponent<Rigidbody>();
        }
        else if (GameFacade.Instance.player == -1)
        {
            Debug.Log("角色未指定");
        }
    }

    void FixedUpdate () 
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            anim.SetBool("GetDamage", true);
        }else
        {
            anim.SetBool("GetDamage", false);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_start")==false&&anim.IsInTransition(0)==false&& anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_end") == false)
            {
                space++;

                if (space == 1)
                {
                    anim.SetBool("Jump", true);
                    rig.AddForce(Vector3.up * jumpforce);
                }
                else if (space == 2)
                {
                    anim.SetBool("Jump2", true);
                    rig.AddForce(Vector3.up * jumpforce);
                }
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Jump2", false);

        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        anim.SetFloat("Run",Mathf.Abs(v));

        rig.MovePosition(rig.position + rig.transform.forward*v* Time.fixedDeltaTime * speed);
        rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(0, h, 0)  * rotatespeed));
	}

    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.tag=="DiMian")
        {
            space = 0;
            anim.SetBool("IsGround", true);
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "DiMian")
        {
            anim.SetBool("IsGround", false);
        }
    }
}
