using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // ドリブルで動かせるマス
    public static List<Vector2Int> dribblePosList = new List<Vector2Int>();
    // パスができるマス
    public static List<Vector2Int> passPosList = new List<Vector2Int>();

    public static readonly Vector2 ballLocalPos = new Vector2(0, 0.25f);// 駒に対するボールの位置
    public static Vector2 ballWorldPos;                                 // ボールの絶対位置

    public static GameObject ballObject;        // ボールのオブジェクト
    public static GameObject pieceHoldingBall;  // ボールを保持している駒のオブジェクト


    private void Awake()
    {
        ballObject = gameObject;
    }

    private void Start()
    {
        ballWorldPos = transform.position;
        pieceHoldingBall = null;
    }

    // ドリブルで動ける座標を計算する
    public static void CalculateDribblePos()
    {
        dribblePosList.Clear(); // Listを初期化

        if (pieceHoldingBall != null)
        {
            // 保持されている場合
            Piece piece = pieceHoldingBall.GetComponent<Piece>();
            piece.Set(pieceHoldingBall.transform.position);
            piece.CalculateDribblePos(dribblePosList);   // ドリブルでの動き
        }
    }

    // パス可能な座標を計算する
    public static void CalculatePassPos()
    {
        passPosList.Clear();    // Listを初期化

        if (pieceHoldingBall != null)
        {
            // 保持されている場合
            Piece piece = pieceHoldingBall.GetComponent<Piece>();
            PieceController pc = pieceHoldingBall.GetComponent<PieceController>();
            piece.Set(pieceHoldingBall.transform.position);
            piece.CalculateMovePos(passPosList);    // パス
            pc.RemoveNotMovablePos(passPosList);    // 動かせないマスは削除
        }
    }
}
