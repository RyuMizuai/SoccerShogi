using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        Vector3 pos = BoardManager.centerPos;
        // ƒJƒƒ‰‚Ì‰ŠúˆÊ’u
        transform.position = new Vector3(pos.x, pos.y, -11);
    }

}
