using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static int boardWidth;       // ��
    public static int boardHeight;      // ����

    public static int boardLeft = 1;    // ���[
    public static int boardRight;       // �E�[
    public static int boardBottom = 1;  // ���[
    public static int boardTop;         // ��[

    public static Vector2 centerPos;    // �Ղ̒��S�̍��W

    [SerializeField]
    private GameObject tilePrefab;      // �^�C����Prefab

    [SerializeField]
    private GameObject frameTilePrefab; // �^�C���̊O�g��Prefab

    // ���Prefab
    [SerializeField]
    List<GameObject> prefabPieces;

    // ��̏����z�u(9*9)
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
        // �Ղ̃T�C�Y�̏�����
        boardHeight = boardSetting.GetLength(0);
        boardWidth = boardSetting.GetLength(1);
        boardRight = boardWidth;
        boardTop = boardHeight;
        // ���S�̍��W���v�Z
        centerPos = new Vector2((boardWidth + 1) / 2.0f, (boardHeight + 1) / 2.0f);
    }

    private void Start()
    {
        BallController.SetBall();   // �{�[���̍��W��ݒ�
    }

    public IEnumerator SetBoard()
    {
        // �ՂƋ�̍쐬
        for (int i = 1; i <= boardWidth; i++)
        {
            for (int j = 1; j <= boardHeight; j++)
            {
                Vector3 pos = new Vector3(i, j);

                // �Ղ̍쐬
                Instantiate(tilePrefab, pos, Quaternion.identity);

                // ��̍쐬
                int type = boardSetting[i - 1, j - 1] % 10;     // ��̎��
                int player = boardSetting[i - 1, j - 1] / 10;   // �v���C���[

                if (type == 0) continue;

                GameObject prefab = prefabPieces[type - 1]; // ���Prefab
                GameObject piece = Instantiate(prefab, pos, Quaternion.Euler(0, 0, player * 180)); // �v���C���[�ɂ���ēK�؂Ȍ����ɉ�]

                // ��Ƀv���C���[�̃��C���[��t����
                if (player == 0)
                {
                    piece.layer = LayerMask.NameToLayer(GameManager.firstPlayerLayer);
                }
                else if (player == 1)
                {
                    piece.layer = LayerMask.NameToLayer(GameManager.secondPlayerLayer);
                }

                // ���Ƌʂ̃I�u�W�F�N�g���擾����
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

        // ��g
        for (int x = 1; x <= boardWidth; x++)
        {
            Instantiate(frameTilePrefab, new Vector3(x, boardHeight + 1), Quaternion.identity);
        }
        // ���g
        for (int x = 1; x <= boardWidth; x++)
        {
            Instantiate(frameTilePrefab, new Vector3(x, 0), Quaternion.identity);
        }
        yield return null;
    }

}
