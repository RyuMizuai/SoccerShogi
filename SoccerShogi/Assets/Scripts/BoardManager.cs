using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int boardWidth;     // ��
    private int boardHeight;    // ����
    private float boardTop;
    private float boardBottom;

    [SerializeField]
    private GameObject tilePrefab;      // �^�C����Prefab

    [SerializeField]
    private GameObject frameTilePrefab; // �^�C���̊O�g��Prefab

    // ���Prefab
    [SerializeField]
    List<GameObject> prefabPieces;

    // ��̏����z�u
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
        // �Ղ̃T�C�Y�̏�����
        boardHeight = 9;
        boardWidth = 9;
        boardTop = (boardHeight - 1) / 2.0f;
        boardBottom = -boardTop;
    }

    void Start()
    {
        // �ՂƋ�̍쐬
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                // (0,0)�𒆐S�Ƃ������W�ɒ���
                float x = i - boardWidth / 2;
                float y = j - boardHeight / 2;
                Vector3 pos = new Vector3(x, y);

                // �Ղ̍쐬
                Instantiate(tilePrefab, pos, Quaternion.identity);

                // ��̍쐬
                int type = boardSetting[i, j] % 10;     // ��̎��
                int player = boardSetting[i, j] / 10;   // �v���C���[

                if (type == 0) continue;

                GameObject prefab = prefabPieces[type - 1]; // ���Prefab
                Instantiate(prefab, pos, Quaternion.Euler(0, player * 180, 0)); // �v���C���[�ɂ���ēK�؂Ȍ����ɉ�]
            }
        }

        // ��g
        for (int i = 0; i < boardWidth; i++)
        {
            float x = i - boardWidth / 2;
            Instantiate(frameTilePrefab, new Vector3(x, boardTop + 1), Quaternion.identity);
        }
        // ���g
        for (int i = 0; i < boardWidth; i++)
        {
            float x = i - boardWidth / 2;
            Instantiate(frameTilePrefab, new Vector3(x, boardBottom - 1), Quaternion.identity);
        }
    }

}
