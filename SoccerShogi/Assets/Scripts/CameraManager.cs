using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        // �J�����̏����ʒu
        transform.position = new Vector3(GameManager.centerPos.x, GameManager.centerPos.y, -11);
    }

}
