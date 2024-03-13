using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int boardWidth;     // 幅
    private int boardHeight;    // 高さ
    private float boardTop;
    private float boardBottom;

    [SerializeField]
    private GameObject tilePrefab;      // タイルのPrefab

    [SerializeField]
    private GameObject frameTilePrefab; // タイルの外枠のPrefab

    // 駒のPrefab
    [SerializeField]
    List<GameObject> prefabPieces;

    // 駒の初期配置
    int[,] boardSetting =
    {
        {2, 0, 1, 0, 0, 0, 11, 0, 12},
        {3, 6, 1, 0, 0, 0, 11,16, 13},
        {4, 0, 1, 0, 0, 0, 11, 0, 14},
        {5, 0, 1, 0, 0, 0, 11, 0, 15},
        {8, 0, 1, 0, 0, 0, 11, 0, 18},
        {5, 0, 1, 0, 0, 0, 11, 0, 15},
        {4, 0, 1, 0, 0, 0, 11, 0, 14},
        {3, 7, 1, 0, 0, 0, 11,17, 13},
        {2, 0, 1, 0, 0, 0, 11, 0, 12},
    };

    private void Awake()
    {
        // 盤のサイズの初期化
        boardHeight = 9;
        boardWidth = 9;
        boardTop = (boardHeight - 1) / 2.0f;
        boardBottom = -boardTop;
    }

    void Start()
    {
        // 盤と駒の作成
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                // (0,0)を中心とした座標に直す
                float x = i - boardWidth / 2;
                float y = j - boardHeight / 2;
                Vector3 pos = new Vector3(x, y);

                // 盤の作成
                Instantiate(tilePrefab, pos, Quaternion.identity);

                // 駒の作成
                int type = boardSetting[i, j] % 10;     // 駒の種類
                int player = boardSetting[i, j] / 10;   // プレイヤー

                if (type == 0) continue;

                GameObject prefab = prefabPieces[type - 1]; // 駒のPrefab
                Instantiate(prefab, pos, Quaternion.Euler(0, player * 180, 0)); // プレイヤーによって適切な向きに回転
            }
        }

        // 上枠
        for (int i = 0; i < boardWidth; i++)
        {
            float x = i - boardWidth / 2;
            Instantiate(frameTilePrefab, new Vector3(x, boardTop + 1), Quaternion.identity);
        }
        // 下枠
        for (int i = 0; i < boardWidth; i++)
        {
            float x = i - boardWidth / 2;
            Instantiate(frameTilePrefab, new Vector3(x, boardBottom - 1), Quaternion.identity);
        }
    }

}
