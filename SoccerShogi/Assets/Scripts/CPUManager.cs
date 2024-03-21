using System.Collections.Generic;
using UnityEngine;

public class CPUManager : MonoBehaviour
{
    public static List<Member> CPUPos = new List<Member>(); // CPU�̑I����
    private static GameManager gameManager;

   
    private void Start()
    {
        gameManager = GameManager.gameManager;
        CPUPos.Clear(); // List����ɂ���
    }

    public static void MovingCPU()
    {
        int count = CPUPos.Count;                   // �I�����̐�

        // �����𔭐�������
        System.Random random = new System.Random();
        int rndIndex = random.Next(count);          // 0�ȏ�count�����̐����𔭐�������

        // �����ɂ���ē�����ƃ}�X�̑g�ݍ��킹�œ�����
        GameObject pieceObj = CPUPos[rndIndex].Object;
        PieceController pc = pieceObj.GetComponent<PieceController>();
        ClickObject.selectingPiece = pieceObj;      // �I�𒆂̋�ɂ���
        pc.MovePiece(CPUPos[rndIndex].Position);    // �ړ�

        // ���邩�I������ꍇ
        if (GameManager.isButtonClicked == false)
        {
            PieceType type = pc.pieceType;  // ��̎��

            switch (type)
            {
                // �����p����ԂȂ琬��
                case PieceType.pawn:
                case PieceType.bishop:
                case PieceType.rook:
                    gameManager.PromoteButton();
                    break;
                // ����ȊO�Ȃ�2����1�Ő���
                case PieceType.lance:
                case PieceType.knight:
                case PieceType.silver:
                    if (new System.Random().Next(0, 2) == 1)
                    {
                        gameManager.PromoteButton();
                    }
                    else
                    {
                        gameManager.NotPromoteButton();
                    }
                    break;
            }
        }
        CPUPos.Clear(); // List����ɂ���
    }
}

// ��̃I�u�W�F�N�g�Ɠ�������}�X�̍��W�����N���X
public class Member
{
    public GameObject Object { get; set; }
    public Vector2Int Position { get; set; }

    public Member(GameObject gameObject, Vector2Int position)
    {
        Object = gameObject;
        Position = position;
    }
}