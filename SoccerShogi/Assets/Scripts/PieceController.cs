using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 駒の種類
public enum PieceType
{
    king,
    rook,
    bishop,
    gold,
    silver,
    knight,
    lance,
    pawn,
}

public class PieceController : MonoBehaviour
{
    private int boardLeft;     // 左端
    private int boardRight;    // 右端
    private int boardBottom;   // 下端
    private int boardTop;      // 上端

    private readonly int playerCanPromotePos = 7;   // 先手駒が成れる位置
    private readonly int enemyCanPromotePos = 3;    // 後手駒が成れる位置

    [SerializeField]
    private PieceType pieceType;            // 駒の種類

    [SerializeField]
    private GameObject firstPlayerStand;    // 先手の駒台

    [SerializeField]
    private GameObject secondPlayerStand;     // 後手の駒台

    [SerializeField]
    private GameObject firstPlayerKingObj;       // 先手玉のオブジェクト

    [SerializeField]
    private GameObject secondPlayerKingObj;        // 後手玉のオブジェクト

    private GameObject ballObject;          // ボールのオブジェクト

    private GameManager gameManager;        // GameManagerの入れ物

    private Piece piece;                    // 駒のPieceクラス
    private GameObject[] pieces;            // すべての駒のゲームオブジェクト
    private Vector2 pieceScale;             // 駒のサイズ
    private Vector2 oldPos;                 // 移動前の座標

    private string pieceTag = "Piece";      // Pieceタグ
    private string firstPlayerLayer = "FirstPlayer";  // FirstPlayerレイヤー
    private string secondPlayerLayer = "SecondPlayer";    // SecondPlayerレイヤー
    private string thisLayer = "";          // この駒のレイヤー

    [System.NonSerialized]
    public List<Vector2Int> pointPosList = new List<Vector2Int>();  // Pointの座標

    private bool isNowUpdate = false;    // 更新中フラグ(要改善)



    private void Awake()
    {
        // 盤の初期化
        boardLeft = GameManager.boardLeft;
        boardRight = GameManager.boardRight;
        boardBottom = GameManager.boardBottom;
        boardTop = GameManager.boardTop;
    }

    private void Start()
    {
        // 変数の初期化
        // gameManagerを取得(あまり良くないかも．今のところはこれでいい)
        gameManager = GameManager.gameManager;
        piece = GetComponent<Piece>();
        pieceScale = transform.lossyScale;
        pieces = GameObject.FindGameObjectsWithTag(pieceTag);
        thisLayer = LayerMask.LayerToName(this.gameObject.layer);
        ballObject = BallController.ballObject;
        piece.Set(transform.position); // 駒の初期設定
        StartCoroutine(Coroutine()); // 全ての駒の初期設定が終わるのを待つために時間差で呼び出す
        oldPos = transform.position;
    }

    private IEnumerator Coroutine()
    {
        yield return null;
        // 先手駒の更新
        if (thisLayer == GameManager.nowPlayer)
        {
            CalculatePointPos();
        }
    }

    // Point(駒が動けるマス)の座標を計算する
    public void CalculatePointPos()
    {
        pointPosList.Clear(); // Listの初期化
        thisLayer = LayerMask.LayerToName(gameObject.layer);    // Layer変数の更新
        // ボールを保持している場合
        if (BallController.pieceHoldingBall == gameObject)
        {
            // ドリブルの範囲を格納
            pointPosList = new List<Vector2Int>(BallController.dribblePosList);
            RemoveNotMovablePos(pointPosList);     // 動かせないマスは削除
        }

        // 駒が盤上にある場合
        else if (!IsInHand())
        {
            piece.Set(transform.position);    // 座標をセット
            piece.CalculateMovePos(pointPosList);  // 駒の動きを計算
            RemoveNotMovablePos(pointPosList);     // 動かせないマスは削除
        }
        // 持ち駒にある場合
        else
        {
            // 王手のとき
            if (GameManager.isInCheck)
            {
                // 合駒ができるとき
                if (GameManager.intersectingPosList.Count > 0)
                {
                    // 合駒の座標を取得
                    foreach (Vector2Int intersectingPos in GameManager.intersectingPosList)
                    {
                        // 二歩はスキップ
                        if (pieceType == PieceType.pawn)
                        {
                            if (PawnExistsInLine(intersectingPos.x)) continue;
                        }
                        // 座標をリストに追加
                        pointPosList.Add(intersectingPos);
                    }
                }
            }
            // 王手でないとき
            // 置けないマスはスキップ
            else
            {
                // 駒がないマスの座標を取得
                for (int i = boardLeft; i <= boardRight; i++)
                {
                    // 二歩はスキップ
                    if (pieceType == PieceType.pawn)
                    {
                        if (PawnExistsInLine(i)) continue;
                    }

                    for (int j = boardBottom; j <= boardTop; j++)
                    {
                        Vector2Int v = new Vector2Int(i, j);
                        // すでに駒があればスキップ
                        if (piece.PieceExistsAtPos(v).Item1) continue;

                        // 置いたらずっと動けないマスはスキップ
                        if (thisLayer == firstPlayerLayer)
                        {
                            if (piece.GetStuckPosY() <= j) continue;
                        }
                        else if (thisLayer == secondPlayerLayer)
                        {
                            if (piece.GetStuckPosY() >= j) continue;
                        }

                        // ボールの位置には置けない
                        if (gameManager.BallExistsAtPos(v)) continue;

                        // 座標をリストに追加
                        pointPosList.Add(v);

                    }
                }
            }
        }
    }

    // 駒を動かす
    public void MovePiece(Vector2 newPos)
    {
        int newXInt = Mathf.RoundToInt(newPos.x);   // 動かした後の座標
        int newYInt = Mathf.RoundToInt(newPos.y);
        Vector2Int newPosInt = new Vector2Int(newXInt, newYInt);

        // 盤上にある駒を動かす場合
        if (!IsInHand())
        {
            thisLayer = LayerMask.LayerToName(gameObject.layer); // 動かす駒のレイヤー

            // 駒が王または金でないかつ，成った状態でないとき，成れるか判別する
            if (!(pieceType == PieceType.king || pieceType == PieceType.gold) && piece.isPromoted == false)
            {
                int oldY = Mathf.RoundToInt(oldPos.y);

                if (thisLayer == firstPlayerLayer) // 先手
                {
                    // 成れるか判別
                    if (newYInt >= piece.GetStuckPosY())
                    {
                        // 強制で成る
                        PromotePiece();
                    }
                    else if (oldY >= playerCanPromotePos || newYInt >= playerCanPromotePos)
                    {
                        // 成るか選択ボタンを表示
                        gameManager.ActivePromoteButton();
                        GameManager.isButtonClicked = false;    // フラグを下ろしておく
                    }
                }
                else if (thisLayer == secondPlayerLayer) // 後手
                {
                    // 成れるか判別
                    if (newYInt <= piece.GetStuckPosY())
                    {
                        // 強制で成る
                        PromotePiece();
                    }
                    else if (newYInt <= enemyCanPromotePos || oldY <= enemyCanPromotePos)
                    {
                        // 成るか選択ボタンを表示
                        gameManager.ActivePromoteButton();
                        GameManager.isButtonClicked = false;    // フラグを下ろしておく
                    }
                }
            }

            // 取った駒を持ち駒にする
            if (piece.PieceExistsAtPos(newPosInt).Item1)
            {
                GameObject obj = piece.PieceExistsAtPos(newPosInt).Item2;   // 取った駒のゲームオブジェクト
                Piece objPiece = obj.GetComponent<Piece>();                 // 取った駒のPieceクラス

                obj.transform.DetachChildren();                             // 子を削除
                obj.GetComponent<PieceController>().DemotePiece();          // 成状態を解除
                obj.transform.rotation *= Quaternion.Euler(0, 0, 180);      // 駒の向きを反転

                Quaternion pieceRotation = piece.transform.rotation;
                obj.transform.position = GameManager.RotateCoordinate(objPiece.GetPieceStandPos(), pieceRotation, GameManager.centerPos);   // 駒台に配置

                string objLayer = LayerMask.LayerToName(obj.layer);         // 取った駒のlayer
                if (objLayer == secondPlayerLayer)
                {
                    obj.layer = LayerMask.NameToLayer(firstPlayerLayer);    // レイヤーを変更
                    obj.transform.SetParent(firstPlayerStand.transform);    // 駒台の子にする
                }
                else if (objLayer == firstPlayerLayer)
                {
                    obj.layer = LayerMask.NameToLayer(secondPlayerLayer);   // レイヤーを変更
                    obj.transform.SetParent(secondPlayerStand.transform);   // 駒台の子にする
                }

                objPiece.SetPieceStand();           // 駒台のコンポーネントをセットし直す
                objPiece.CountUp();                 // 駒のカウントを増やす
                GameManager.DisplayPieceCount(obj); // 駒のカウントを表示

                // ボールの座標を丸める
                float posX = Mathf.Round(ballObject.transform.position.x);
                float posY = Mathf.Round(ballObject.transform.position.y);
                ballObject.transform.position = new Vector2(posX, posY);
            }

            // ボールがあれば保持
            if (gameManager.BallExistsAtPos(newPosInt))
            {
                ballObject.transform.SetParent(gameObject.transform);               // 駒の子にする
                ballObject.transform.localPosition = BallController.ballLocalPos;   // ボールの位置を駒に対して少しずらす
                BallController.pieceHoldingBall = gameObject;                       // ボール保持
            }
        }
        // 持ち駒を置いた場合
        else
        {
            piece.CountDown();                          // カウントを減らす
            transform.SetParent(null);                  // 親をnullにする
            GameManager.DisplayPieceCount(gameObject);  // 駒のカウントを表示

            // 打ち歩詰めか判定
            if (pieceType == PieceType.pawn)
            {
                transform.position = new Vector2(newXInt, newYInt); // 打った場所に移動

                if (gameManager.IsCheckmate())
                {
                    // 歩を打って詰んだら打った方の負け
                    Debug.Log("打ち歩詰めです！");
                    string winningPlayer;
                    if (GameManager.nowPlayer == firstPlayerLayer)
                    {
                        winningPlayer = secondPlayerLayer;
                    }
                    else
                    {
                        winningPlayer = firstPlayerLayer;
                    }
                    gameManager.GameOver(winningPlayer);    // ゲーム終了
                }
            }
        }

        SoundManager.soundManager.MakeMotionSound();    // 効果音を鳴らす

        // positionとscaleを整数に直す
        transform.position = (Vector3Int)newPosInt;
        transform.localScale = pieceScale;
        oldPos = transform.position;    // 次に動かす時のために更新
    }

    // 駒が持ち駒か判定する
    public bool IsInHand()
    {
        GameObject pieceStand;  // 駒台のオブジェクト
        if (thisLayer == firstPlayerLayer)
        {
            pieceStand = firstPlayerStand;
        }
        else
        {
            pieceStand = secondPlayerStand;
        }

        foreach (Transform child in pieceStand.transform)
        {
            // 自身のオブジェクトが駒台の子であればTrueを返す
            if (this.gameObject.transform == child)
            {
                return true;
            }
        }
        return false;
    }

    // 駒を成る
    private void PromotePiece()
    {
        piece.isPromoted = true;
        GetComponent<SpriteRenderer>().sprite = piece.promotedPieceSprite; // 画像を差し替える
    }

    // 成りを解除
    private void DemotePiece()
    {
        piece.isPromoted = false;
        GetComponent<SpriteRenderer>().sprite = piece.pieceSprite; // 画像を差し替える
    }

    // 入力された行に味方の歩があるか判定
    private bool PawnExistsInLine(int i)
    {
        thisLayer = LayerMask.LayerToName(gameObject.layer);

        for (int j = boardBottom; j <= boardTop; j++)
        {
            Vector2Int v = new Vector2Int(i, j);
            // 座標(i,j)に駒があるとき
            if (piece.PieceExistsAtPos(v).Item1)
            {
                GameObject obj = piece.PieceExistsAtPos(v).Item2; // (i,j)にある駒のオブジェクト
                PieceController objPc = obj.GetComponent<PieceController>();

                if (objPc.pieceType != PieceType.pawn || LayerMask.LayerToName(obj.layer) != thisLayer) continue;

                // その駒が味方の歩である
                if (!obj.GetComponent<Piece>().isPromoted)
                {
                    // かつ成っていないときtrueを返す
                    return true;
                }
            }
        }
        return false;
    }

    // 移動不可能なマスを削除する
    public void RemoveNotMovablePos(List<Vector2Int> pointPosList)
    {
        List<Vector2Int> removePosList = new List<Vector2Int>();    // 削除するマスのList

        // 移動不可能なら座標を削除
        foreach (Vector2Int pointPos in pointPosList)
        {
            // 盤の外側のとき
            if (pointPos.x < boardLeft || pointPos.x > boardRight || pointPos.y < boardBottom || pointPos.y > boardTop)
            {
                // ゴールでないまたはパス中でない場合
                if (!gameManager.GoalExistsAtPos(pointPos) || !GameManager.isPassing)
                {
                    removePosList.Add(pointPos); // List追加
                    continue;
                }
            }

            // パスの場合
            if (GameManager.isPassing)
            {
                // 敵の駒の位置にはパスできない(無くてもいい)

                // 敵の駒と重なっているか
                /*if (piece.PieceExistsAtPos(pointPos).Item1)
                {
                    GameObject obj = piece.PieceExistsAtPos(pointPos).Item2;
                    // 敵の駒ならパスできない
                    if (LayerMask.LayerToName(obj.layer) != thisLayer)
                    {
                        removePosList.Add(pointPos); // List追加
                        continue;
                    }
                }*/

                // 王手中の場合のパス座標削除の処理
                if (GameManager.isInCheck)
                {
                    // 合駒ができる場合
                    if (GameManager.intersectingPosList.Count > 0)
                    {
                        // 合駒の座標にパスの座標が含まれていない場合
                        if (!GameManager.intersectingPosList.Contains(pointPos))
                        {
                            removePosList.Add(pointPos);// List追加
                        }
                    }
                    // 合駒ができない場合
                    else
                    {
                        removePosList.Add(pointPos);    // List追加
                    }

                }
            }
            // パス以外の場合
            else
            {
                // 味方の駒の位置には移動できない

                // 味方の駒と重なっているか
                if (piece.PieceExistsAtPos(pointPos).Item1)
                {
                    GameObject obj = piece.PieceExistsAtPos(pointPos).Item2;
                    // 味方の駒ならと同じ位置なら座標Listから削除
                    if (LayerMask.LayerToName(obj.layer) == thisLayer)
                    {
                        removePosList.Add(pointPos); // List追加
                        continue;
                    }
                }

                if (!isNowUpdate) // 更新中の場合は再帰回避のためスキップ
                {

                    // 動かすと詰みならその位置には動かせない

                    // 動かすと詰みかどうか
                    Vector2 nowPos = transform.position;        // 座標を保持
                    transform.position = (Vector2)pointPos;     // 仮に動かす
                    // 仮に取る駒を保持する変数
                    GameObject obj = null;
                    string objLayer = "";

                    if (piece.PieceExistsAtPos(pointPos).Item1)
                    {
                        // 動かした先で駒を取れるなら仮に取ったことにする
                        obj = piece.PieceExistsAtPos(pointPos).Item2;   // 取った駒のゲームオブジェクト
                        objLayer = LayerMask.LayerToName(obj.layer);    // 取った駒のレイヤー
                        if (objLayer != thisLayer)
                        {
                            // 仮に取った駒のレイヤーを一時的に変更
                            obj.layer = LayerMask.NameToLayer("Default");
                        }
                    }
                    // 動かすと詰みなら座標Listから削除
                    if (thisLayer == firstPlayerLayer)
                    {
                        UpdateMoving(secondPlayerLayer);
                        if (gameManager.IsCheck(secondPlayerLayer))
                        {
                            removePosList.Add(pointPos);    // List追加
                        }
                    }
                    else if (thisLayer == secondPlayerLayer)
                    {
                        UpdateMoving(firstPlayerLayer);
                        if (gameManager.IsCheck(firstPlayerLayer))
                        {
                            removePosList.Add(pointPos);    // List追加
                        }
                    }
                    // 仮に取った駒のレイヤーと自身の座標を元に戻す
                    if (obj != null)
                    {
                        obj.layer = LayerMask.NameToLayer(objLayer);
                    }
                    transform.position = nowPos;
                }
            }
        }

        foreach (Vector2Int removePos in removePosList)
        {
            pointPosList.Remove(removePos);
        }
    }

    // 入力されたプレーヤーの駒の動きを更新する
    public void UpdateMoving(string playerName)
    {
        BallController.CalculateDribblePos();   // ドリブルの範囲を更新

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();

            // 持ち駒または別のプレイヤーの駒ならスキップ
            if (pc.IsInHand() || pieceLayer != playerName) continue;

            // プレイヤーの駒のPointを更新
            pc.isNowUpdate = true;   // 改善点(更新中フラグ)
            pc.CalculatePointPos();
            pc.isNowUpdate = false;
        }
    }
}
