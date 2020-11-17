using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIFollowWorldObj : MonoBehaviour
{
    [SerializeField]
    public  GameObject worldPos;//3D物体（人物）
    [SerializeField]
    RectTransform rectTrans;//UI元素
    public Vector2 offset;//偏移量
    Transform playerCamera;
    Image myImage;

    void Awake()
    {
        playerCamera = Camera.main.transform;
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (worldPos == null)
        {
            return;
        }
        float angle = Vector3.Angle(playerCamera.forward, worldPos.transform.position - playerCamera.position);
        if (angle >= 90)
        {
            myImage.enabled = false;
            return;
        }
        else
        {
            myImage.enabled = true;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos.transform.position);
            rectTrans.position = screenPos + offset;
        }
    }
}
