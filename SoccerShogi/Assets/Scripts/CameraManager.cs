using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        // カメラの初期位置
        transform.position = new Vector3(0, 0, -11);
    }

}
