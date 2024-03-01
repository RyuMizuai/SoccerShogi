using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private int boardLeft;     // ���[
    private int boardRight;    // �E�[
    private int boardBottom;   // ���[
    private int boardTop;      // ��[

    [SerializeField]
    private GameObject tilePrefab;  // �^�C����Prefab

    [SerializeField]
    private GameObject frameTilePrefab;  // �^�C����Prefab

    private void Awake()
    {
        // �Ղ̏�����
        boardLeft = GameManager.boardLeft;
        boardRight = GameManager.boardRight;
        boardBottom = GameManager.boardBottom;
        boardTop = GameManager.boardTop;
    }

    void Start()
    {
        // �Ղ̍쐬
        for (int x = boardLeft; x <= boardRight; x++)
        {
            for (int y = boardBottom; y <= boardTop; y++)
            {
                Instantiate(tilePrefab, new Vector3Int(x, y, 0), Quaternion.identity);
            }
        }
        // ��g
        for (int x = boardLeft; x <= boardRight; x++)
        {
            Instantiate(frameTilePrefab, new Vector3Int(x, boardTop + 1, 0), Quaternion.identity);
        }
        // ���g
        for (int x = boardLeft; x <= boardRight; x++)
        {
            Instantiate(frameTilePrefab, new Vector3Int(x, boardBottom - 1, 0), Quaternion.identity);
        }
    }

}
