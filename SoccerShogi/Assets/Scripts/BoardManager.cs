using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static int boardWidth;       // 幅
    public static int boardHeight;      // 高さ

    public static int boardLeft = 1;    // 左端
    public static int boardRight;       // 右端
    public static int boardBottom = 1;  // 下端
    public static int boardTop;         // 上端

    public static Vector2 centerPos;    // 盤の中心の座標

    [SerializeField]
    private GameObject tilePrefab;      // タイルのPrefab

    [SerializeField]
    private GameObject frameTilePrefab; // タイルの外枠のPrefab

    // 駒のPrefab
    [SerializeField]
    List<GameObject> prefabPieces;

    // 駒の初期配置(9*9)
    private readonly int[,] boardSetting_1 =
    {
        {2, 0, 1, 0, 0, 0, 11, 0, 12},
        {3, 6, 1, 0, 0, 0, 11,17, 13},
        {4, 0, 1, 0, 0, 0, 11, 0, 14},
        {5, 0, 1, 0, 0, 0, 11, 0, 15},
        {8, 0, 1, 0, 0, 0, 11, 0, 19},
        {5, 0, 1, 0, 0, 0, 11, 0, 15},
        {4, 0, 1, 0, 0, 0, 11, 0, 14},
        {3, 7, 1, 0, 0, 0, 11,16, 13},
        {2, 0, 1, 0, 0, 0, 11, 0, 12},
    };

    private readonly int[,] boardSetting_2 =
    {
        {3, 0, 1,0, 11, 0,13},
        {4, 6, 1,0, 11, 17,14},
        {5, 0, 1,0, 11, 0,15},
        {8, 0, 1,0, 11, 0,19},
        {5, 0, 1,0, 11, 0,15},
        {4, 7, 1,0, 11, 16,14},
        {3, 0, 1,0, 11, 0,13},
    };

    private readonly int[,] boardSetting_3 =
    {
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {8, 0, 0,0, 0, 0,0,0,19},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
    };

    private readonly int[,] boardSetting_4 =
    {
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,6, 0, 0,0,0,0},
        {8, 0, 0,0, 0, 0,0,0,19},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
        {0, 0, 0,0, 0, 0,0,0,0},
    };

    int[,] boardSetting;    

    private void Awake()
    {
        boardSetting = boardSetting_1;
        // 盤のサイズの初期化
        boardHeight = boardSetting.GetLength(0);
        boardWidth = boardSetting.GetLength(1);
        boardRight = boardWidth;
        boardTop = boardHeight;
        // 中心の座標を計算
        centerPos = new Vector2((boardWidth + 1) / 2.0f, (boardHeight + 1) / 2.0f);
    }

    private void Start()
    {
        BallController.SetBall();   // ボールの座標を設定
    }

    public IEnumerator SetBoard()
    {
        // 盤と駒の作成
        for (int i = 1; i <= boardWidth; i++)
        {
            for (int j = 1; j <= boardHeight; j++)
            {
                Vector3 pos = new Vector3(i, j);

                // 盤の作成
                Instantiate(tilePrefab, pos, Quaternion.identity);

                // 駒の作成
                int type = boardSetting[i - 1, j - 1] % 10;     // 駒の種類
                int player = boardSetting[i - 1, j - 1] / 10;   // プレイヤー

                if (type == 0) continue;

                GameObject prefab = prefabPieces[type - 1]; // 駒のPrefab
                GameObject piece = Instantiate(prefab, pos, Quaternion.Euler(0, 0, player * 180)); // プレイヤーによって適切な向きに回転

                // 駒にプレイヤーのレイヤーを付ける
                if (player == 0)
                {
                    piece.layer = LayerMask.NameToLayer(GameManager.firstPlayerLayer);
                }
                else if (player == 1)
                {
                    piece.layer = LayerMask.NameToLayer(GameManager.secondPlayerLayer);
                }

                // 王と玉のオブジェクトを取得する
                if (type == 8)
                {
                    GameManager.gameManager.firstPlayerKingObj = piece;
                }
                else if (type == 9)
                {
                    GameManager.gameManager.secondPlayerKingObj = piece;
                }
            }
        }

        // 上枠
        for (int x = 1; x <= boardWidth; x++)
        {
            Instantiate(frameTilePrefab, new Vector3(x, boardHeight + 1), Quaternion.identity);
        }
        // 下枠
        for (int x = 1; x <= boardWidth; x++)
        {
            Instantiate(frameTilePrefab, new Vector3(x, 0), Quaternion.identity);
        }
        yield return null;
    }

}
