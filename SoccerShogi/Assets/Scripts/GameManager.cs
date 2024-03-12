using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static readonly Vector2Int centerPos = new Vector2Int(5, 5);  // 盤の中心の座標
    public static readonly int boardLeft = 1;     // 左端
    public static readonly int boardRight = 9;    // 右端
    public static readonly int boardBottom = 1;   // 下端
    public static readonly int boardTop = 9;      // 上端

    private static readonly float epsilon = 0.0001f; // 小さい数

    public static string gameState = "";    // ゲームの状態
    public static string nowPlayer = "";    // 今のプレイヤー

    public static bool isButtonClicked = false;         // Buttonを押したか
    public static bool isInCheck = false;               // 王手か
    public static bool isSelectingPromotion = false;    // 成るか選択中フラグ
    public static bool isPassing = false; // パス中か
    public static bool isDribbling = false; // ドリブル中か

    public static List<GameObject> pieceInCheckList = new List<GameObject>();   // 王手をしている駒
    public static List<Vector2Int> intersectingPosList = new List<Vector2Int>();// 合駒の座標

    public static string inCheckPlayer = "";    // 王手をしているプレーヤー
    public static string nowSceneName = "";     // 現在のシーン名

    [SerializeField]
    private GameObject promoteButtonPanel;  // 成るボタンパネル

    [SerializeField]
    private Image promotedPieceImage;       // 成るボタンに表示する画像

    [SerializeField]
    private Image notPromotedPieceImage;    // 成らないボタンに表示する画像

    [SerializeField]
    private GameObject actionButtonPanel;   // アクションボタンパネル

    [SerializeField]
    private GameObject firstPlayerKingObj;  // 先手玉のオブジェクト

    [SerializeField]
    private GameObject secondPlayerKingObj; // 後手玉のオブジェクト

    [SerializeField]
    private TMP_Text messageText;           // ゲーム終了後のメッセージのText

    private GameObject ballObject;          // サッカーボールのオブジェクト
    public GameObject pointPrefab;          // PointのPrefab

    private readonly string firstPlayerLayer = "FirstPlayer";   // FirstPlayerレイヤー
    private readonly string secondPlayerLayer = "SecondPlayer"; // SecondPlayerレイヤー
    private string pieceTag = "Piece";                          // Pieceタグ

    private GameObject[] pieces;            // すべての駒のゲームオブジェクト

    public static GameManager gameManager;  // GameManagerの入れ物

    public static List<Vector2Int> goalPosList = new List<Vector2Int>(); // ゴールの位置

    private void Awake()
    {
        gameManager = this;    // static変数に自分を保存する
    }

    private void Start()
    {
        pieces = GameObject.FindGameObjectsWithTag(pieceTag);
        ballObject = BallController.ballObject;
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
    public static void DisplayPieceCount(GameObject pieceObj)
    {
        Piece piece = pieceObj.GetComponent<Piece>();
        int count = piece.GetCount();                           // 持ち駒の数
        GameObject textObj = piece.GetCountText();              // 持ち駒の数のText
        textObj.transform.rotation = piece.transform.rotation;  // Textを駒と同じ向きにする

        // 持ち駒が2個以上あるとき表示
        if (count > 1)
        {
            Quaternion pieceRotation = piece.transform.rotation;
            Vector2 piecePos = RotateCoordinate(piece.GetPieceStandPos(), pieceRotation, centerPos);    // 駒の座標
            Vector2 pos = RotateCoordinate(new Vector2(0.35f, 0.3f), pieceRotation, new Vector2(0.0f, 0.0f)); // 駒からのずれ
            Vector2 textPos = piecePos + pos;   // Textの座標
            textObj.transform.position = textPos;
            textObj.GetComponent<TMP_Text>().text = count.ToString();
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
        isButtonClicked = true;                 // ボタンクリックフラグオン
    }
    
    // 成らずボタン
    public void NotPromotePieceButton()
    {
        // ボタンを消すだけ
        promoteButtonPanel.SetActive(false);    // ボタン非表示
        isSelectingPromotion = false;           // フラグを下ろす
        isButtonClicked = true;                 // ボタンクリックフラグオン
    }

    // 成るボタンを表示する
    public void ActivePromoteButton()
    {
        Piece piece = ClickObject.selectingPiece.GetComponent<Piece>(); // 選択中の駒のPieceを取得
        // ボタンに表示する画像を選択中の駒のものに差し替える
        promotedPieceImage.sprite = piece.promotedPieceSprite;
        notPromotedPieceImage.sprite = piece.pieceSprite;
        promoteButtonPanel.SetActive(true);
    }

    // パスボタン
    public void PassButton()
    {
        CancelAction(ClickObject.selectingPiece); // クリック中の駒のアクションを初期化
        isPassing = true;   // パス中
        BallController.CalculatePassPos();      // パスの範囲を更新

        // パスの範囲を表示
        foreach (Vector2Int passPos in BallController.passPosList)
        {
            Instantiate(pointPrefab, (Vector2)passPos, Quaternion.identity); // PointのPrefabを作成
        }
    }

    // ドリブルボタン
    public void DribbleButton()
    {
        CancelAction(ClickObject.selectingPiece); // クリック中の駒のアクションを初期化
        isDribbling = true; // ドリブル中
        PieceController pc = BallController.pieceHoldingBall.GetComponent<PieceController>();
        Debug.Log(BallController.pieceHoldingBall);
        // ドリブルの範囲を表示
        foreach (Vector2Int pointPos in pc.pointPosList)
        {
            Instantiate(pointPrefab, (Vector2)pointPos, Quaternion.identity); // PointのPrefabを作成
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
        if (BallController.pieceHoldingBall != null)
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
        actionButtonPanel.SetActive(true);  // ボタン表示
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

            // 持ち駒または別プレイヤーの駒のときスキップ
            if (pc.IsInHand() || pieceLayer != playerName) continue;

            foreach (Vector2Int pointPos in pc.pointPosList)
            {
                // Pointと相手の王の座標が一致したときtrueを返す
                if (TwoPositionsEquals(kingObj.transform.position, pointPos))
                {
                    inCheckPlayer = playerName; // 王手中のプレイヤーを保持
                    return true;
                }
            }
        }
        return false;
    }

    // 王手をしている駒を返す
    private List<GameObject> GetPiecesInCheck()
    {
        List<GameObject> objList = new List<GameObject>();

        // 王手されているプレイヤーの玉
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();

            // 持ち駒であるか，王手中のプレイヤーの駒でないときスキップ
            if (pc.IsInHand() || pieceLayer != inCheckPlayer) continue;
            
            foreach (Vector2 pointPos in pc.pointPosList)
            {
                // Pointと相手の王の座標が一致したとき
                if (TwoPositionsEquals(kingObj.transform.position, pointPos))
                {
                    // List追加
                    objList.Add(pc.gameObject);
                }
            }
        }
        return objList;
    }

    // 合駒の座標を計算する
    private List<Vector2Int> GetIntersectingPos()
    {
        List<Vector2Int> vec = new List<Vector2Int>();

        // 王手されているプレイヤーの玉
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);

        // 両王手の場合は合駒が利かないため
        // 王手をしている駒が1つの場合のみ合駒の座標を計算
        if (pieceInCheckList.Count == 1)
        {
            GameObject pieceInCheck = pieceInCheckList[0];  // 王手をしている駒

            // 王手中の駒と王の間に王手中の駒のPointがあれば合駒List追加
            PieceController pc = pieceInCheck.GetComponent<PieceController>();

            foreach (Vector2Int pointPos in pc.pointPosList)
            {
                // 王手中の駒のPointが王手中の駒と王の内分点かどうか
                if (IsInternalDivision(pieceInCheck.transform.position, kingObj.transform.position,pointPos))
                {
                    vec.Add(pointPos); // 合駒List追加
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

        // 王手をしている駒と王の位置関係によって処理を変える

        // 縦方向のとき 3点x座標が同じ
        if (Mathf.Approximately(v.x, v2.x) && Mathf.Approximately(v.x, v3.x))
        {
            return 0 < ty && ty < 1;    // 内分しているか
        }

        // 横方向のとき 3点y座標が同じ
        else if (Mathf.Approximately(v.y, v2.y) && Mathf.Approximately(v.y, v3.y))
        {
            return 0 < tx && tx < 1;    // 内分しているか
        }
        // 斜めのとき
        else
        {
            bool equal = Mathf.Approximately(tx, ty);   // v3がvとv2を結んだ直線上にあるか→tx=tyかどうか
            return equal && 0 < tx && tx < 1;           // 内分しているか(tyでもいい)
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
                if (pc.pointPosList.Count != 0)
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

        // 王手関係のListを初期化
        pieceInCheckList.Clear();
        intersectingPosList.Clear();

        BallController.CalculateDribblePos();   // ドリブルの範囲を更新

        // 自分の駒のPoint更新
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) == nowPlayer)
            {
                PieceController pc = piece.GetComponent<PieceController>();
                // 動ける位置を更新する
                pc.CalculatePointPos();
            }
        }

        isInCheck = IsCheck(nowPlayer); // 王手情報更新

        // 王手チェック
        if (isInCheck)
        {
            pieceInCheckList = GetPiecesInCheck();      // 王手中の駒
            intersectingPosList = GetIntersectingPos(); // 合駒の座標
        }

        // 相手の駒のPoint更新
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) != nowPlayer)
            {
                PieceController pc = piece.GetComponent<PieceController>();
                // 動ける位置を更新する
                pc.CalculatePointPos();
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
        string message = (winningPlayer == firstPlayerLayer) ? "あなたの勝ち！" : "あなたの負け!";
        
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
    public bool BallExistsAtPos(Vector2 v)
    {
        return TwoPositionsEquals(ballObject.transform.position, v);
    }

    // 受け取った座標がゴール位置か判定する
    public bool GoalExistsAtPos(Vector2Int v)
    {
        return goalPosList.Contains(v);
    }

    // 2つの座標が等しいか判定する
    public static bool TwoPositionsEquals(Vector2 v1, Vector2 v2)
    {
        return Vector2.SqrMagnitude(v1 - v2) < epsilon;
    }
    
}

    
