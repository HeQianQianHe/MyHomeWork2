using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject center;
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    Animator anim;
    public GameObject vfxPrefab;
    float timer = 0;
    public Transform rayTrans;
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        center.transform.position = transform.position+new Vector3(0,2,0);
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (controller.isGrounded)
        {
            if (x != 0 || y != 0)
            {
                anim.SetBool("run", true);
            }
            else
            {
                anim.SetBool("run", false);
            }

            moveDirection = new Vector3(x, 0, y);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;


            if (Input.GetMouseButton(0)&& Time.time - timer >= 0.2f)
            {
                timer = Time.time;
                if (Random.Range(0,6)<=3)
                {
                    anim.SetTrigger("attack1");
                }
                else if (Random.Range(0, 3) == 4)
                {
                    anim.SetTrigger("attack2");
                }
                else
                {
                    anim.SetTrigger("attack3");
                }
               
            }

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

    }

}
