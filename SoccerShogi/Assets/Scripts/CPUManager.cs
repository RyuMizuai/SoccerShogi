using System.Collections.Generic;
using UnityEngine;

public class CPUManager : MonoBehaviour
{
    public static List<Member> CPUPos = new List<Member>(); // CPUの選択肢
    private static GameManager gameManager;

   
    private void Start()
    {
        gameManager = GameManager.gameManager;
        CPUPos.Clear(); // Listを空にする
    }

    public static void MovingCPU()
    {
        int count = CPUPos.Count;                   // 選択肢の数

        // 乱数を発生させる
        System.Random random = new System.Random();
        int rndIndex = random.Next(count);          // 0以上count未満の整数を発生させる

        // 乱数によって得た駒とマスの組み合わせで動かす
        GameObject pieceObj = CPUPos[rndIndex].Object;
        PieceController pc = pieceObj.GetComponent<PieceController>();
        ClickObject.selectingPiece = pieceObj;      // 選択中の駒にする
        pc.MovePiece(CPUPos[rndIndex].Position);    // 移動

        // 成るか選択する場合
        if (GameManager.isButtonClicked == false)
        {
            PieceType type = pc.pieceType;  // 駒の種類

            switch (type)
            {
                // 歩か角か飛車なら成る
                case PieceType.pawn:
                case PieceType.bishop:
                case PieceType.rook:
                    gameManager.PromoteButton();
                    break;
                // それ以外なら2分の1で成る
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
        CPUPos.Clear(); // Listを空にする
    }
}

// 駒のオブジェクトと動かせるマスの座標を持つクラス
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