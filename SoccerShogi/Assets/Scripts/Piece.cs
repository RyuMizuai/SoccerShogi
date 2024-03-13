using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    protected int boardLeft = 1;     // 左端
    protected int boardRight = 9;    // 右端
    protected int boardBottom = 1;   // 下端
    protected int boardTop = 9;      // 上端

    public Sprite pieceSprite;          // 駒の画像
    public Sprite promotedPieceSprite;  // 成駒の画像

    protected float posX;       // 駒のx座標を保管する
    protected float posY;       // 駒のy座標を保管する
    protected int stuckPosY;    // 動けなくなるマスの座標

    protected PieceStand ps;                // PieceStandの入れ物
    private GameObject[] pieces;          // すべての駒のゲームオブジェクト

    private string firstPlayerLayer = "FirstPlayer";  //First Playerレイヤー
    private string secondPlayerLayer = "SecondPlayer";    // SecondPlayerレイヤー
    private string thisLayer = "";          // 駒のレイヤー

    [System.NonSerialized]
    public bool isPromoted = false; // 成っているか




    abstract protected void Init(); // 初期化用メソッド

    // Setメソッド
    abstract public void Set(Vector2 pos);

    // 駒が動ける座標を計算する
    abstract public void CalculateMovePos(List<Vector2Int> pointPosList);

    // ドリブルで動ける座標を計算する
    abstract public void CalculateDribblePos(List<Vector2Int> pointPosList);

    // 駒台の座標を返す
    abstract public Vector2 GetPieceStandPos();

    // カウントを増やす
    abstract public void CountUp();
    // カウントを減らす
    abstract public void CountDown();

    // 持ち駒の数を受け取る
    abstract public int GetCount();
    // 持ち駒の数のTextを受け取る
    abstract public GameObject GetCountText();



    private void Awake()
    {
        // 初期化
        Init();
        
    }

    private void Start()
    {
        boardLeft = BoardManager.boardLeft;
        boardRight = BoardManager.boardRight;
        boardBottom = BoardManager.boardBottom;
        boardTop = BoardManager.boardTop;

        SetPieceStand();
        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        // GameManagerの初期化完了まで待つ
        yield return new WaitUntil(() => GameManager.isFinishInitialize);
        pieces = GameManager.gameManager.pieces;    // 駒を取得
    }


    // 指定の座標に他の駒があるかを判定する
    public (bool, GameObject) PieceExistsAtPos(Vector2Int v)
    {
        foreach (GameObject piece in pieces)
        {
            if (piece == this.gameObject) continue; // 自身と同じならスキップ

            // 座標が一致したらtrueと駒のオブジェクトを返す
            if (GameManager.TwoPositionsEquals(piece.transform.position, v))
            {
                return (true, piece);
            }
        }
        return (false, null);
    }

    // 入力された角度と距離をもとに，駒が動けるマスの座標を計算する
    protected void CalculateXY(float a, float d, List<Vector2Int> pointPosList)
    {
        float x = Mathf.Round(posX + d * Mathf.Cos(a * Mathf.Deg2Rad)); // x座標
        float y = Mathf.Round(posY + d * Mathf.Sin(a * Mathf.Deg2Rad)); // y座標
        // 動く範囲を向いている方向に合わせる
        Vector2 v = GameManager.RotateCoordinate(new Vector2(x, y), transform.rotation, transform.position);
        pointPosList.Add(new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)));
    }

    protected void CalculateXY(float a, List<Vector2Int> pointPosList)
    {
        float x = Mathf.Round(posX + Mathf.Cos(a * Mathf.Deg2Rad));
        float y = Mathf.Round(posY + Mathf.Sin(a * Mathf.Deg2Rad));
        Vector2 v = GameManager.RotateCoordinate(new Vector2(x, y), transform.rotation, transform.position);
        pointPosList.Add(new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)));
    }

    public void SetPieceStand()
    {
        // レイヤーでどっちのpsを呼び出すか判断
        thisLayer = LayerMask.LayerToName(gameObject.layer);
        if (thisLayer == firstPlayerLayer)
        {
            ps = GameManager.gameManager.firstPlayerStand.GetComponent<PieceStand>();
        }
        else if (thisLayer == secondPlayerLayer)
        {
            ps = GameManager.gameManager.secondPlayerStand.GetComponent<PieceStand>();
        }
    }

    // 動けなくなるマスの座標を返す
    public int GetStuckPosY()
    {
        Vector2 v = new Vector2(0, stuckPosY);
        // 駒の向きによって座標を反転
        Vector2 stuckPos = GameManager.RotateCoordinate(v, transform.rotation, BoardManager.centerPos);
        return Mathf.RoundToInt(stuckPos.y);
    }
}

