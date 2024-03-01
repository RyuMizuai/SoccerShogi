using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // �h���u���œ�������ʒu
    public static List<Vector2Int> dribblePosList = new List<Vector2Int>();

    public static List<Vector2Int> passPosList = new List<Vector2Int>();

    public static readonly Vector2 ballLocalPos = new Vector2(0, 0.25f);// ��ɑ΂���{�[���̈ʒu
    public static Vector2 ballWorldPos;                                 // �{�[���̐�Έʒu

    private void Start()
    {
        ballWorldPos = transform.position;
    }

    // �h���u���œ�������W���v�Z����
    public void CalculateDribblePos()
    {
        dribblePosList.Clear();
        if (transform.root.gameObject != gameObject)
        {
            // �e������ꍇ(�ێ�����Ă���ꍇ)
            GameObject parentPiece = transform.root.gameObject; // �e�̋�̃I�u�W�F�N�g
            Piece piece = parentPiece.GetComponent<Piece>();
            PieceController pc = parentPiece.GetComponent<PieceController>();
            piece.Set(parentPiece.transform.position);
            piece.CalculateDribblePos(dribblePosList);   // �h���u���ł̓���
            pc.RemoveNotMovablePos(dribblePosList);      // �������Ȃ��}�X�͍폜
        }
    }

    // �p�X�\�ȍ��W���v�Z����
    public void CalculatePassPos()
    {
        passPosList.Clear();
        if (transform.root.gameObject != gameObject)
        {
            // �e������ꍇ(�ێ�����Ă���ꍇ)
            GameObject parentPiece = transform.root.gameObject; // �e�̋�̃I�u�W�F�N�g
            Piece piece = parentPiece.GetComponent<Piece>();
            PieceController pc = parentPiece.GetComponent<PieceController>();
            piece.Set(parentPiece.transform.position);
            piece.CalculateMovePos(passPosList);   // �p�X
            pc.RemoveNotMovablePos(passPosList);      // �������Ȃ��}�X�͍폜
        }
    }

}
