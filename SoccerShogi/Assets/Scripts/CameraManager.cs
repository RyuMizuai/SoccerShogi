using UnityEngine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        Vector3 pos = BoardManager.centerPos;
        // カメラの初期位置
        transform.position = new Vector3(pos.x, pos.y + 0.2f, -11);
    }

}
