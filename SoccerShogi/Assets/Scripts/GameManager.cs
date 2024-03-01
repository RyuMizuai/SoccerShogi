using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static readonly Vector2Int centerPos = new Vector2Int(5, 5);  // 盤の中心の座標
    public static readonly int boardLeft = 1;     // 左端
    public static readonly int boardRight = 9;    // 右端
    public static readonly int boardBottom = 1;   // 下端
    public static readonly int boardTop = 9;      // 上端

    private readonly float epsilon = 0.0001f; // 小さい数

    public static string gameState = "";    // ゲームの状態
    public static string nowPlayer = "";    // 今のプレイヤー

    public static bool isButtonClicked = false;         // Buttonを押したか
    public static bool isInCheck = false;               // 王手か
    public static bool isSelectingPromotion = false;    // 成るか選択中フラグ
    public static bool isPassing = false; // パス中か
    public static bool isDribbling = false; // ドリブル中か

    public static List<GameObject> piecesInCheck = new List<GameObject>();      // 王手をしている駒
    public static List<Vector2Int> intersectingPos = new List<Vector2Int>();    // 合駒の座標

    public static string inCheckPlayer = "";    // 王手をしているプレーヤー
    public static string nowSceneName = "";     // 現在のシーン名

    [SerializeField]
    private GameObject promoteButtonPanel;  // 成るボタンパネル

    [SerializeField]
    private GameObject actionButtonPanel;   // アクションボタンパネル

    [SerializeField]
    private GameObject firstPlayerKingObj;       // 先手玉のオブジェクト

    [SerializeField]
    private GameObject secondPlayerKingObj;        // 後手玉のオブジェクト

    public GameObject ballObject;           // サッカーボールのオブジェクト
    public GameObject pointPrefab;          // PointのPrefab

    private readonly string firstPlayerLayer = "FirstPlayer";  // FirstPlayerレイヤー
    private readonly string secondPlayerLayer = "SecondPlayer";    // SecondPlayerレイヤー
    private string pieceTag = "Piece";      // Pieceタグ

    private GameObject[] pieces;            // すべての駒のゲームオブジェクト

    [System.NonSerialized]
    public bool isCalledFromInCheck = false;    // IsCheckから呼ばれたか(要改善)

    public static GameManager gameManager;  // GameManagerの入れ物

    public Text messageText;                // ゲーム終了後のメッセージのText

    public static List<Vector2Int> goalPosList = new List<Vector2Int>(); // ゴールの位置

    private void Awake()
    {
        gameManager = this;
    }

    private void Start()
    {
        pieces = GameObject.FindGameObjectsWithTag(pieceTag);
        promoteButtonPanel.SetActive(false);                // ボタン非表示
        actionButtonPanel.SetActive(false);                 // ボタン非表示
        messageText.gameObject.SetActive(false);            // Text非表示
        gameState = "Playing";                              // ゲーム状態
        nowPlayer = firstPlayerLayer;                       // 現在のプレイヤー名を先手にする
        nowSceneName = SceneManager.GetActiveScene().name;  // 現在のシーン名
        goalPosList.Add(new Vector2Int(5, 10));
    }
    
    // 現在の座標と回転させる向き，回転軸の座標を受け取って，回転後の座標を返す
    public static Vector2 RotateCoordinate(Vector2 objPosition, Quaternion objRotation, Vector2 axisPos)
    {
        Vector3 newPos = objPosition - axisPos;         // 平行移動後の座標
        Vector3 v = objRotation * newPos;               // 回転後の座標
        return new Vector2(v.x, v.y) + axisPos;         // もう一度平行移動

    }

    // 持ち駒の数を画面に表示する
    public static void DisplayPieceCount(Piece piece)
    {
        int count = piece.GetCount();                           // 持ち駒の数
        GameObject textObj = piece.GetCountText();              // 持ち駒の数のText
        textObj.transform.rotation = piece.transform.rotation;  // Textを駒と同じ向きにする

        // 持ち駒が2個以上あるとき表示
        if (count > 1)
        {
            Quaternion pieceRotation = piece.transform.rotation;
            Vector2 piecePos = RotateCoordinate(piece.GetPieceStandPos(), pieceRotation, centerPos);    // 駒の座標
            Vector2 pos = RotateCoordinate(new Vector2(0.25f, 0.2f), pieceRotation, new Vector2(0.0f, 0.0f)); // 駒からのずれ
            Vector2 textPos = piecePos + pos;   // Textの座標
            textObj.transform.position = textPos;
            textObj.GetComponent<Text>().text = count.ToString();
            textObj.SetActive(true);    // 表示
        }
        // 持ち駒が0個か1個のとき非表示
        else
        {
            textObj.SetActive(false);   // 非表示
        }
    }

    // 成るボタン
    public void PromotePieceButton()
    {
        Piece piece = ClickObject.selectingPiece.GetComponent<Piece>(); // 操作中の駒
        piece.isPromoted = true;
        piece.GetComponent<SpriteRenderer>().sprite = piece.promotedPieceSprite; // 画像を差し替える
        promoteButtonPanel.SetActive(false);    // ボタン非表示
        isSelectingPromotion = false;           // フラグを下ろす
        ClickObject.selectingPiece = null;      // 操作終了
        isButtonClicked = true;                 // ボタンクリックフラグオン
    }
    
    // 成らずボタン
    public void NotPromotePieceButton()
    {
       // ボタンを消すだけ
        promoteButtonPanel.SetActive(false);    // ボタン非表示
        isSelectingPromotion = false;           // フラグを下ろす
        ClickObject.selectingPiece = null;      // 操作終了
        isButtonClicked = true;                 // ボタンクリックフラグオン
    }

    // 成るボタンを表示する
    public void ActivePromoteButton()
    {
        promoteButtonPanel.SetActive(true);
    }

    // パスボタン
    public void PassButton()
    {
        CancelAction(ClickObject.selectingPiece); // クリック中の駒のアクションを初期化
        isPassing = true;   // パス中

        foreach (Vector2Int passPos in BallController.passPosList)
        {
            Instantiate(pointPrefab, new Vector3(passPos.x, passPos.y), Quaternion.identity); // PointのPrefabを作成
        }
    }

    // ドリブルボタン
    public void DribbleButton()
    {
        CancelAction(ClickObject.selectingPiece); // クリック中の駒のアクションを初期化
        isDribbling = true; // ドリブル中        

        foreach (Vector2Int dribblePos in BallController.dribblePosList)
        {
            Instantiate(pointPrefab, new Vector3(dribblePos.x, dribblePos.y), Quaternion.identity); // PointのPrefabを作成
        }
    }

    // アクションやめる
    public void CancelAction(GameObject obj)
    {
        // フラグを下ろす
        isPassing = false;
        isDribbling = false;
        // 駒とボールの座標を元の位置に戻す
        obj.transform.position = ClickObject.selectingPiecePos;
        // 保持されている場合
        if (ballObject.transform.root.gameObject != ballObject)
        {
            // ボールを駒の少し上に配置
            ballObject.transform.localPosition = BallController.ballLocalPos;
        }
        // 親がいない場合
        else
        {
            // 元の位置に配置
            ballObject.transform.position = BallController.ballWorldPos;
        }
        
        // Pointを全削除
        ClickObject.DeleteAllPoints();
    }

    // パスボタンとドリブルボタンを表示する
    public void ActiveActionButton()
    {
        actionButtonPanel.SetActive(true);
    }

    // パスボタンとドリブルボタンを非表示にする
    public void InactiveActionButton()
    {
        actionButtonPanel.SetActive(false);
    }

    // リトライボタン
    public void RetryButton()
    {
        SceneManager.LoadScene(nowSceneName);   // 現在のシーンを読み込む
    }


    // 入力されたプレーヤーが王手をしているかどうか判定する
    public bool IsCheck(string playerName)
    {
        inCheckPlayer = "Default"; // 王手中のプレイヤーを初期化
        // 相手玉のオブジェクト
        GameObject kingObj = GetOpponentKingObject(playerName);

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();
            // 入力されたプレイヤーの駒で盤上のものを取得
            if (!pc.IsInHand() && pieceLayer == playerName)
            {
                for (int i = 0; i < pc.pointList.Count; i++)
                {
                    Vector3 v = new Vector3(pc.pointList[i].x, pc.pointList[i].y);
                    // Pointの座標のいずれかが相手の王と一致したときtrueを返す
                    if (Vector3.SqrMagnitude(kingObj.transform.position - v) < epsilon)
                    {
                        inCheckPlayer = playerName; // 王手中のプレイヤー
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // 王手をしている駒を返す
    private List<GameObject> GetPiecesInCheck()
    {
        List<GameObject> obj = new List<GameObject>();
        // 王手されているプレイヤーの玉
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();
            // 王手中のプレイヤーの駒で盤上のものを取得
            if (!pc.IsInHand() && pieceLayer == inCheckPlayer)
            {
                for (int i = 0; i < pc.pointList.Count; i++)
                {
                    Vector3 v = new Vector3(pc.pointList[i].x, pc.pointList[i].y);
                    // Pointの座標のいずれかが相手の王と一致したとき
                    if (Vector3.SqrMagnitude(kingObj.transform.position - v) < epsilon)
                    {
                        // List追加
                        obj.Add(pc.gameObject);
                    }
                }
            }
        }
        return obj;
    }

    // 合駒の座標を計算する
    private List<Vector2Int> GetIntersectingPos()
    {
        List<Vector2Int> vec = new List<Vector2Int>();
        // 王手されているプレイヤーの玉
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);
        
        foreach (GameObject pieceInCheck in piecesInCheck)  //王手中の駒
        {
            // 王手中の駒と王の間に王手中の駒のPointがあれば合駒List追加
            PieceController pc = pieceInCheck.GetComponent<PieceController>();
            for (int i = 0; i < pc.pointList.Count; i++)
            {
                Vector2Int v = pc.pointList[i];

                // 王手中の駒のPointが王手中の駒と王の内分点かどうか
                if (IsInternalDivision(pieceInCheck.transform.position, kingObj.transform.position, v))
                {
                    vec.Add(v); // 合駒List追加
                }
            }
        }
        return vec;
    }

    // v3がvとv2の内分点かどうか
    private bool IsInternalDivision(Vector2 v, Vector2 v2, Vector2 v3)
    {
        // 0除算回避
        if (v == v2) return false;

        float tx = Mathf.InverseLerp(v.x, v2.x, v3.x);  // x方向の内分点
        float ty = Mathf.InverseLerp(v.y, v2.y, v3.y);  // y方向の内分点

        // 縦方向のとき
        if (Mathf.Approximately(v.x, v2.x))
        {
            bool equal = Mathf.Approximately(v.x, v3.x);    // 3点x座標が同じか
            return equal && 0 < ty && ty < 1;                  // 内分しているか
        }
        // 横方向のとき
        else if (Mathf.Approximately(v.y, v2.y))
        {
            bool equal = Mathf.Approximately(v.y, v3.y);    // 3点y座標が同じか
            return equal && 0 < tx && tx < 1;                  // 内分しているか
        }
        // 斜めのとき
        else
        {
            bool equal = Mathf.Approximately(tx, ty);   // v3がvとv2を結んだ直線上にあるか→tx=tyかどうか
            return equal && 0 < tx && tx < 1;              // 内分しているか(tyでもいい)
        }
    }

    // 相手玉を取得
    public GameObject GetOpponentKingObject(string playerName)
    {
        if (playerName == firstPlayerLayer)
        {
            return secondPlayerKingObj;
        }
        else if (playerName == secondPlayerLayer)
        {
            return firstPlayerKingObj;
        }
        else
        {
            Debug.Log("Player Name Error");
            return null;
        }
    }

    // 詰みを判定する
    public bool IsCheckmate()
    {
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) != nowPlayer)
            {
                // 相手のすべての駒が動けなかったら自分の勝ち
                PieceController pc = piece.GetComponent<PieceController>();
                if (pc.pointList.Count != 0)
                {
                    return false;   // Pointが１つでもあればfalse
                }
            }
        }
        return true;    // 1つもなければtrue
    }

    public IEnumerator PromoteButtonCoroutine()
    {
        // 成るボタンが押されるのを待つ
        yield return new WaitUntil(() => isButtonClicked);

        // 駒を解除
        ClickObject.selectingPiece = null;
        ClickObject.oldSelectingPiece = null;

        piecesInCheck.Clear();  // 王手中の駒を初期化

        // bcを取得
        BallController bc = ballObject.GetComponent<BallController>();
        bc.CalculateDribblePos();   // ドリブルの範囲を更新
        bc.CalculatePassPos();      // パスの範囲を更新

        // 王手チェック
        if (isInCheck = IsCheck(nowPlayer))
        {
            piecesInCheck = GetPiecesInCheck();     // 王手中の駒
            intersectingPos = GetIntersectingPos(); // 合駒の座標
        }

        // 相手駒のPointの更新
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) != nowPlayer)
            {
                PieceController pc = piece.GetComponent<PieceController>();
                // 動ける位置を更新する
                pc.CalculatePointPos(pc.pointList);
                
            }
        }
        

        // 詰みチェック
        if (isInCheck)
        {
            // 相手がどこも動かせなかったら詰み
            if (IsCheckmate())
            {
                StartCoroutine(GameOver(nowPlayer));    // 現在のプレイヤーの勝ち
            }
        }

        // 操作終了　相手のターンへ
        if (nowPlayer == firstPlayerLayer)
        {
            nowPlayer = secondPlayerLayer;
        }
        else if (nowPlayer == secondPlayerLayer)
        {
            nowPlayer = firstPlayerLayer;
        }
       
    }

    // ターン終了
    public void TurnEnd()
    {
        StartCoroutine(PromoteButtonCoroutine());   // ボタンコルーチンを呼ぶ
    }


    // ゲーム終了
    public IEnumerator GameOver(string winningPlayer)
    {
        gameState = "GameEnd";  // ゲーム終了

        // 勝ったプレイヤーに応じてメッセージをTextに出力
        string message;
        if (winningPlayer == firstPlayerLayer)
        {
            message = "You Win!";
        }
        else
        {
            message = "You Lose!";
        }
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);  // 3秒待つ
        TitleManager.LoadStartScene();          // スタートシーンを読み込む
    }

    // 投了ボタン
    public void GiveUpButton()
    {
        if (nowPlayer == firstPlayerLayer)
        {
            StartCoroutine(GameOver(secondPlayerLayer));
        }
        else
        {
            StartCoroutine(GameOver(firstPlayerLayer));
        }
    }

    // サッカーボールのオブジェクトと座標が一致したらtrueを返す
    public bool BallExistsAtPos(Vector2Int v)
    {
        return Vector3.SqrMagnitude(ballObject.transform.position - (Vector3Int)v) < epsilon;
    }

    // 受け取った座標がゴール位置か判定する
    public bool GoalExistsAtPos(Vector2Int v)
    {
        foreach(Vector2Int goalPos in goalPosList)
        {
            if (Vector3.SqrMagnitude(ballObject.transform.position - (Vector3Int)v) < epsilon)
            {
                return true;
            }
        }
        return false;
    }

    
}

    
