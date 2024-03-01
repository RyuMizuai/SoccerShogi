using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private int boardLeft;     // 左端
    private int boardRight;    // 右端
    private int boardBottom;   // 下端
    private int boardTop;      // 上端

    [SerializeField]
    private GameObject tilePrefab;  // タイルのPrefab

    [SerializeField]
    private GameObject frameTilePrefab;  // タイルのPrefab

    private void Awake()
    {
        // 盤の初期化
        boardLeft = GameManager.boardLeft;
        boardRight = GameManager.boardRight;
        boardBottom = GameManager.boardBottom;
        boardTop = GameManager.boardTop;
    }

    void Start()
    {
        // 盤の作成
        for (int x = boardLeft; x <= boardRight; x++)
        {
            for (int y = boardBottom; y <= boardTop; y++)
            {
                Instantiate(tilePrefab, new Vector3Int(x, y, 0), Quaternion.identity);
            }
        }
        // 上枠
        for (int x = boardLeft; x <= boardRight; x++)
        {
            Instantiate(frameTilePrefab, new Vector3Int(x, boardTop + 1, 0), Quaternion.identity);
        }
        // 下枠
        for (int x = boardLeft; x <= boardRight; x++)
        {
            Instantiate(frameTilePrefab, new Vector3Int(x, boardBottom - 1, 0), Quaternion.identity);
        }
    }

}
