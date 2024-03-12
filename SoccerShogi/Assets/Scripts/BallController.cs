using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // �h���u���œ�������}�X
    public static List<Vector2Int> dribblePosList = new List<Vector2Int>();
    // �p�X���ł���}�X
    public static List<Vector2Int> passPosList = new List<Vector2Int>();

    public static readonly Vector2 ballLocalPos = new Vector2(0, 0.25f);// ��ɑ΂���{�[���̈ʒu
    public static Vector2 ballWorldPos;                                 // �{�[���̐�Έʒu

    public static GameObject ballObject;        // �{�[���̃I�u�W�F�N�g
    public static GameObject pieceHoldingBall;  // �{�[����ێ����Ă����̃I�u�W�F�N�g


    private void Awake()
    {
        ballObject = gameObject;
    }

    private void Start()
    {
        ballWorldPos = transform.position;
        pieceHoldingBall = null;
    }

    // �h���u���œ�������W���v�Z����
    public static void CalculateDribblePos()
    {
        dribblePosList.Clear(); // List��������

        if (pieceHoldingBall != null)
        {
            // �ێ�����Ă���ꍇ
            Piece piece = pieceHoldingBall.GetComponent<Piece>();
            piece.Set(pieceHoldingBall.transform.position);
            piece.CalculateDribblePos(dribblePosList);   // �h���u���ł̓���
        }
    }

    // �p�X�\�ȍ��W���v�Z����
    public static void CalculatePassPos()
    {
        passPosList.Clear();    // List��������

        if (pieceHoldingBall != null)
        {
            // �ێ�����Ă���ꍇ
            Piece piece = pieceHoldingBall.GetComponent<Piece>();
            PieceController pc = pieceHoldingBall.GetComponent<PieceController>();
            piece.Set(pieceHoldingBall.transform.position);
            piece.CalculateMovePos(passPosList);    // �p�X
            pc.RemoveNotMovablePos(passPosList);    // �������Ȃ��}�X�͍폜
        }
    }
}
