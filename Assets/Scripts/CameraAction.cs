using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAction : MonoBehaviour {

    //滚轮放大和缩小的速度
    private float mouseScrollSpeed = 40;

    //单击右键旋转的速度
    private float mouseRightDragSpeed = 5;

    //按中键移动的速度
    private float mouseMidDragSpeed = 1.26f;

    //上一次光标位置
    private Vector3 mouseLastPosition = new Vector3(0, 0, 0);

    //光标位置变化量
    private Vector3 mousePositionDelta = new Vector3(0, 0, 0);

    //单击鼠标右键旋转相机速度
    private Vector3 rotateDelta = new Vector3(0, 0, 0);
    

    private void FixedUpdate()
    {
        MouseEvent();
    }

    void MouseEvent()
    {

        //按左键移动摄像机位置
        if(Input.GetMouseButton(0))
        {
            mousePositionDelta = Input.mousePosition - mouseLastPosition;
            mouseLastPosition = Input.mousePosition;
            if (mousePositionDelta.magnitude != 0)
            {
                rotateDelta = new Vector3(-mousePositionDelta.x * Time.deltaTime * mouseMidDragSpeed,
                    -mousePositionDelta.y * Time.deltaTime * mouseMidDragSpeed,0);
                transform.Translate(rotateDelta, Space.Self);
            }
        }
        else if(Input.GetMouseButton(1))
        {
            mousePositionDelta = Input.mousePosition - mouseLastPosition;
            mouseLastPosition = Input.mousePosition;
            if(mousePositionDelta.magnitude != 0)
            {
                rotateDelta = new Vector3(-mousePositionDelta.y * Time.deltaTime * mouseRightDragSpeed,
                    mousePositionDelta.x * Time.deltaTime * mouseRightDragSpeed,0);

                //按Alt + 鼠标右键
                if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    this.transform.Translate(new Vector3(0, 0, Time.deltaTime * mouseScrollSpeed * (rotateDelta.x + rotateDelta.y)), Space.Self);
                }
                //只按鼠标右键
                else
                {
                    transform.eulerAngles += rotateDelta;
                }
                
            }
        }
        //如果没有鼠标按键
        else
        {
            mouseLastPosition = Input.mousePosition;
        }
        //滑轮放大缩小
        if(Input.mouseScrollDelta.y != 0)
        {
            transform.Translate(new Vector3(0, 0, Time.deltaTime * mouseScrollSpeed * Input.mouseScrollDelta.y), Space.Self);
        }
    }
}
