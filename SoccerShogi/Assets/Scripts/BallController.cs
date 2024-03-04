using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // ドリブルで動かせる位置
    public static List<Vector2Int> dribblePosList = new List<Vector2Int>();

    public static List<Vector2Int> passPosList = new List<Vector2Int>();

    public static readonly Vector2 ballLocalPos = new Vector2(0, 0.25f);// 駒に対するボールの位置
    public static Vector2 ballWorldPos;                                 // ボールの絶対位置
    static GameObject ballObject;

    private void Start()
    {
        ballWorldPos = transform.position;
        ballObject = this.gameObject;
    }

    // ドリブルで動ける座標を計算する
    public static void CalculateDribblePos()
    {
        dribblePosList.Clear(); // Listを初期化

        if (ballObject.transform.parent != null)
        {
            // 親がいる場合(保持されている場合)
            GameObject parentPiece = ballObject.transform.root.gameObject; // 親の駒のオブジェクト
            Piece piece = parentPiece.GetComponent<Piece>();
            PieceController pc = parentPiece.GetComponent<PieceController>();
            piece.Set(parentPiece.transform.position);
            piece.CalculateDribblePos(dribblePosList);   // ドリブルでの動き
            pc.RemoveNotMovablePos(dribblePosList);      // 動かせないマスは削除
        }
    }

    // パス可能な座標を計算する
    public static void CalculatePassPos()
    {
        passPosList.Clear();    // Listを初期化

        if (ballObject.transform.parent != null)
        {
            // 親がいる場合(保持されている場合)
            GameObject parentPiece = ballObject.transform.root.gameObject; // 親の駒のオブジェクト
            Piece piece = parentPiece.GetComponent<Piece>();
            PieceController pc = parentPiece.GetComponent<PieceController>();
            piece.Set(parentPiece.transform.position);
            piece.CalculateMovePos(passPosList);   // パス
            pc.RemoveNotMovablePos(passPosList);      // 動かせないマスは削除
        }
    }

}
